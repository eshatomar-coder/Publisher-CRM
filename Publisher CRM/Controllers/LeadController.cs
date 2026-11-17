using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Publisher_CRM.Controllers
{
    public class LeadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHierarchyService _hierarchyService;

        public LeadController(ApplicationDbContext context, IHierarchyService hierarchyService)
        {
            _context = context;
            _hierarchyService = hierarchyService;
        }

        // ==========================================
        // 1. INDEX & GRID DATA API
        // ==========================================
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetLeadsList()
        {
            var usersList = await _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToListAsync();
            var leadsQuery = _context.Set<LeadViewModel>().AsQueryable();

            // SECURITY FILTER: Check for Administrator Profile OR Role Hierarchy
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userProfile = User.Claims.FirstOrDefault(c => c.Type == "Profile")?.Value;

                if (userProfile != "Administrator" && !string.IsNullOrEmpty(currentUserId))
                {
                    var currentUser = usersList.FirstOrDefault(u => u.RecordId == currentUserId);
                    List<string> allowedUserIds = new List<string> { currentUserId };

                    if (currentUser != null && !string.IsNullOrEmpty(currentUser.Role))
                    {
                        var currentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == currentUser.Role);
                        if (currentRole != null)
                        {
                            var allRoles = await _context.Roles.ToListAsync();
                            var subordinateRoleIds = GetSubordinateRoleIds(allRoles, currentRole.Id);
                            var allowedRoleNames = allRoles.Where(r => subordinateRoleIds.Contains(r.Id)).Select(r => r.Name).ToList();
                            var subordinateUserIds = usersList.Where(u => allowedRoleNames.Contains(u.Role)).Select(u => u.RecordId).ToList();

                            allowedUserIds.AddRange(subordinateUserIds);
                        }
                    }

                    // Apply the list to the query to restrict visibility
                    leadsQuery = leadsQuery.Where(l => allowedUserIds.Contains(l.LeadOwnerId));
                }
            }

            var leadsData = await leadsQuery.OrderByDescending(l => l.CreatedTime).ToListAsync();

            var leads = leadsData.Select(l => {
                var ownerUser = usersList.FirstOrDefault(u => !string.IsNullOrEmpty(l.LeadOwnerId) && u.RecordId.Trim() == l.LeadOwnerId.Trim());
                string mappedOwnerName = ownerUser != null ? $"{ownerUser.FirstName} {ownerUser.LastName}".Trim() : (l.LeadOwnerId ?? "-");

                return new
                {
                    recordId = l.RecordId,
                    enquiryId = l.EnquiryId,
                    salesCallId = l.SalesCallId,
                    accountNameId = l.AccountNameId ?? "-",
                    contactNameId = l.ContactNameId ?? "-",
                    enquiryType = l.EnquiryType ?? "-",
                    remarks = l.Remarks ?? "-",
                    followupDate = l.FollowupDate.HasValue ? l.FollowupDate.Value.ToString("yyyy-MM-dd") : "",
                    lastSalesCallDate = l.LastSalesCallDate.HasValue ? l.LastSalesCallDate.Value.ToString("yyyy-MM-dd") : "",
                    lastCallRemark = l.LastCallRemark ?? "-",
                    callType = l.CallType ?? "-",
                    nature = l.Nature ?? "-",
                    stage = l.Stage ?? "Open",
                    enquiryValue = l.EnquiryValue,
                    currency = l.Currency ?? "INR",
                    commercialTerms = l.CommercialTerms ?? "-",
                    leadOwnerId = mappedOwnerName,
                    createdTime = l.CreatedTime.HasValue ? l.CreatedTime.Value.ToString("yyyy-MM-dd HH:mm") : "",
                    modifiedTime = l.ModifiedTime.HasValue ? l.ModifiedTime.Value.ToString("yyyy-MM-dd HH:mm") : ""
                };
            }).ToList();

            // Wrapped in { data = ... } to match frontend DataTables configuration
            return Json(new { data = leads });
        }

        private List<int> GetSubordinateRoleIds(List<Role> allRoles, int parentId)
        {
            var subordinateIds = new List<int>();
            var children = allRoles.Where(r => r.ParentRoleId == parentId).ToList();

            foreach (var child in children)
            {
                subordinateIds.Add(child.Id);
                subordinateIds.AddRange(GetSubordinateRoleIds(allRoles, child.Id));
            }

            return subordinateIds;
        }

        // ==========================================
        // 2. CREATE (GET & POST)
        // ==========================================
        [HttpGet]
        public IActionResult Create(int? salesCallId)
        {
            var model = new LeadViewModel
            {
                EnquiryId = "ENQ-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                SalesCallId = salesCallId,
                Stage = "Open",
                Currency = "INR"
            };

            if (User.Identity.IsAuthenticated)
            {
                model.LeadOwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            PopulateDropdowns(model);

            // Pre-fill from Sales Call if ID is passed
            if (salesCallId.HasValue)
            {
                var salesCall = _context.Salescall.FirstOrDefault(s => s.Id == salesCallId.Value);
                if (salesCall != null)
                {
                    model.AccountNameId = salesCall.AccountName;
                    model.ContactNameId = salesCall.Contacts?.Split(',').FirstOrDefault();
                    model.CallType = salesCall.CallType;
                    model.LastSalesCallDate = salesCall.CallDateTime;
                    model.LastCallRemark = salesCall.Remarks;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeadViewModel model)
        {
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(model.LeadOwnerId))
            {
                model.LeadOwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedTime = DateTime.Now;
                    model.ModifiedTime = DateTime.Now;

                    _context.Set<LeadViewModel>().Add(model);
                    await _context.SaveChangesAsync();

                    // If you have a separate child table for Titles, you process EnquiryTitlesJson here
                    // e.g. 
                    // if (!string.IsNullOrWhiteSpace(model.EnquiryTitlesJson)) {
                    //     var titles = JsonSerializer.Deserialize<List<EnquiryTitleItem>>(model.EnquiryTitlesJson);
                    //     ... save to child table ...
                    // }

                    TempData["SuccessMessage"] = "Enquiry created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database error occurred: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }

            PopulateDropdowns(model);
            return View(model);
        }

        // ==========================================
        // 3. EDIT (GET & POST)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var lead = await _context.Set<LeadViewModel>().FirstOrDefaultAsync(l => l.RecordId == id);
            if (lead == null) return NotFound();

            PopulateDropdowns(lead);

            // If titles are stored in a separate table in your DB, fetch and serialize them into lead.EnquiryTitlesJson here
            // lead.EnquiryTitlesJson = ...;

            return View(lead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, LeadViewModel model)
        {
            if (id != model.RecordId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    model.ModifiedTime = DateTime.Now;

                    _context.Update(model);
                    await _context.SaveChangesAsync();

                    // Handle updating of EnquiryTitlesJson child records here if applicable

                    TempData["SuccessMessage"] = "Enquiry updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database error occurred: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }

            PopulateDropdowns(model);
            return View(model);
        }

        // ==========================================
        // 4. DATA ENDPOINTS (API & JSON)
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetEnquiryApiData()
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                // Calling external API
                var response = await client.GetAsync("http://192.168.0.88:8080/api/marketing/series-and-titles");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return Content(jsonString, "application/json");
                }
                return BadRequest(new { message = "Failed to fetch data from the marketing API." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetContactsByAccount(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName)) return Json(new List<string>());

            var account = _context.Account.FirstOrDefault(a => a.AccountName.ToLower() == accountName.Trim().ToLower());
            if (account == null) return Json(new List<string>());

            string accountIdString = account.AccountId.ToString();
            string accountRecordId = account.RecordId ?? "";

            var contacts = _context.Contacts
                .Where(c => c.AccountNameId == accountIdString ||
                            (!string.IsNullOrEmpty(accountRecordId) && c.AccountNameId == accountRecordId) ||
                            c.AccountName == account.AccountName)
                .Select(c => (c.FirstName + " " + c.LastName).Trim())
                .Distinct()
                .ToList();

            return Json(contacts);
        }

        [HttpPost]
        public IActionResult ParseTitlesFile(IFormFile uploadFile)
        {
            if (uploadFile == null || uploadFile.Length == 0)
                return Json(new { success = false, message = "Please select a valid file." });

            try
            {
                var titles = new List<EnquiryTitleItem>();
                using (var reader = new StreamReader(uploadFile.OpenReadStream()))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true,
                        MissingFieldFound = null,
                        HeaderValidated = null
                    };

                    using (var csv = new CsvReader(reader, config))
                    {
                        var records = csv.GetRecords<dynamic>().ToList();
                        foreach (var row in records)
                        {
                            var rowDict = (IDictionary<string, object>)row;

                            string isbn = GetDictValue(rowDict, "ISBN", "Isbn", "Barcode");
                            string title = GetDictValue(rowDict, "Title", "Title Name", "Book");
                            int.TryParse(GetDictValue(rowDict, "Qty", "Quantity"), out int qty);
                            decimal.TryParse(GetDictValue(rowDict, "Net Price", "Price", "NetPrice"), out decimal price);

                            if (!string.IsNullOrWhiteSpace(isbn) || !string.IsNullOrWhiteSpace(title))
                            {
                                titles.Add(new EnquiryTitleItem
                                {
                                    Isbn = isbn ?? "",
                                    TitleName = title ?? "",
                                    Qty = qty > 0 ? qty : 1,
                                    NetPrice = price
                                });
                            }
                        }
                    }
                }
                return Json(new { success = true, data = titles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error parsing file: " + ex.Message });
            }
        }

        private string GetDictValue(IDictionary<string, object> dict, params string[] keys)
        {
            foreach (var key in keys)
            {
                var match = dict.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (match != null && dict[match] != null) return dict[match].ToString();
            }
            return "";
        }

        // ==========================================
        // 5. CSV LEAD EXPORT IMPORTER (Adapted for new Architecture)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> ImportCsv(IFormFile uploadFile)
        {
            if (uploadFile != null && uploadFile.Length > 0)
            {
                try
                {
                    using (var reader = new StreamReader(uploadFile.OpenReadStream()))
                    {
                        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            HeaderValidated = null,
                            MissingFieldFound = null,
                            Delimiter = ",",
                            IgnoreBlankLines = true
                        };

                        using (var csv = new CsvReader(reader, csvConfig))
                        {
                            csv.Context.RegisterClassMap<CsvLeadImportMap>();
                            var importedRecords = csv.GetRecords<CsvLeadImport>().ToList();
                            var validLeadsToInsert = new List<LeadViewModel>();
                            int skippedRows = 0;

                            foreach (var record in importedRecords)
                            {
                                string contactName = $"{record.FirstName} {record.LastName}".Trim();
                                string accountName = record.Company;

                                if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(record.LeadOwnerId))
                                {
                                    skippedRows++;
                                    continue;
                                }

                                var newEnquiry = new LeadViewModel
                                {
                                    RecordId = string.IsNullOrWhiteSpace(record.RecordId) ? Guid.NewGuid().ToString() : record.RecordId,
                                    EnquiryId = "ENQ-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                                    LeadOwnerId = record.LeadOwnerId,
                                    AccountNameId = accountName,
                                    ContactNameId = contactName,
                                    EnquiryType = record.LeadSource, // Mapping LeadSource to EnquiryType for backwards compat
                                    Stage = "Open",
                                    Currency = "INR",
                                    CreatedTime = DateTime.Now,
                                    ModifiedTime = DateTime.Now
                                };

                                validLeadsToInsert.Add(newEnquiry);
                            }

                            if (validLeadsToInsert.Any())
                            {
                                _context.AddRange(validLeadsToInsert);
                                await _context.SaveChangesAsync();

                                string msg = $"Successfully imported {validLeadsToInsert.Count} enquiries into the CRM!";
                                if (skippedRows > 0) msg += $" (Skipped {skippedRows} rows missing Company or Owner ID).";
                                TempData["SuccessMessage"] = msg;
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Upload failed. No valid rows were found in the CSV.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Import Error: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a valid CSV file to upload.";
            }

            return RedirectToAction(nameof(Index));
        }

        private class CsvLeadImport
        {
            public string RecordId { get; set; }
            public string LeadOwnerId { get; set; }
            public string Company { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string LeadSource { get; set; }
        }

        private sealed class CsvLeadImportMap : ClassMap<CsvLeadImport>
        {
            public CsvLeadImportMap()
            {
                Map(m => m.RecordId).Name("Record Id").Optional();
                Map(m => m.LeadOwnerId).Name("Lead Owner.id").Optional();
                Map(m => m.Company).Name("Company").Optional();
                Map(m => m.FirstName).Name("First Name").Optional();
                Map(m => m.LastName).Name("Last Name").Optional();
                Map(m => m.LeadSource).Name("Lead Source").Optional();
            }
        }

        // ==========================================
        // 6. VIEW HELPERS
        // ==========================================
        private void PopulateDropdowns(LeadViewModel model)
        {
            model.AccountsList = GetRealAccounts();

            model.EnquiryTypesList = new List<SelectListItem> {
                new SelectListItem { Value = "OEM", Text = "OEM" },
                new SelectListItem { Value = "White Labeling", Text = "White Labeling" },
                new SelectListItem { Value = "Distribution", Text = "Distribution" }
            };

            model.NaturesList = new List<SelectListItem> {
                new SelectListItem { Value = "Hot", Text = "Hot" },
                new SelectListItem { Value = "Warm", Text = "Warm" },
                new SelectListItem { Value = "Cold", Text = "Cold" }
            };

            model.StatusList = new List<SelectListItem> {
                new SelectListItem { Value = "Open", Text = "Open" },
                new SelectListItem { Value = "In Working", Text = "In Working" },
                new SelectListItem { Value = "Closed - Won", Text = "Closed - Won" },
                new SelectListItem { Value = "Closed - Lost", Text = "Closed - Lost" }
            };

            model.CurrencyList = new List<SelectListItem> {
                new SelectListItem { Value = "INR", Text = "INR (₹)" },
                new SelectListItem { Value = "USD", Text = "USD ($)" },
                new SelectListItem { Value = "EUR", Text = "EUR (€)" },
                new SelectListItem { Value = "GBP", Text = "GBP (£)" }
            };

            ViewBag.OwnerList = GetRealUsers();
        }

        private IEnumerable<SelectListItem> GetRealUsers()
        {
            try
            {
                return _context.Users.Select(u => new SelectListItem
                {
                    Value = u.RecordId.ToString(),
                    Text = u.FirstName + " " + u.LastName
                }).ToList();
            }
            catch
            {
                return new List<SelectListItem> { new SelectListItem { Value = "1", Text = "System Admin" } };
            }
        }

        private IEnumerable<SelectListItem> GetRealAccounts()
        {
            try
            {
                return _context.Account.Select(a => new SelectListItem
                {
                    Value = a.AccountName,
                    Text = a.AccountName
                }).ToList();
            }
            catch
            {
                return new List<SelectListItem> { new SelectListItem { Value = "Default Account", Text = "Default Account" } };
            }
        }
    }
}