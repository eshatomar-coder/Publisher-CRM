using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using Publisher_CRM.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Publisher_CRM.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHierarchyService _hierarchyService; // 1. Add this

        public AccountController(ApplicationDbContext context, IHierarchyService hierarchyService)
        {
            _context = context;
            _hierarchyService = hierarchyService;
        }

        [HttpPost]
        public async Task<IActionResult> TriggerBcSync([FromServices] IBusinessCentralSyncService bcSyncService)
        {
            try
            {
                // Triggers the exact same logic as your background worker
                await bcSyncService.SyncAccountsFromApiAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logger configured
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new Account();

            if (User.Identity.IsAuthenticated)
            {
                // We MUST use NameIdentifier (RecordId) because your database dropdowns map to IDs
                model.AccountOwner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            // 1. Fetch all users and format them for a dropdown list
            var usersList = _context.Users
                .Where(u => !string.IsNullOrEmpty(u.RecordId))
                .Select(u => new SelectListItem
                {
                    Value = u.RecordId.Trim(),
                    Text = u.FirstName + " " + u.LastName
                })
                .ToList();

            // 2. Send the list to the View
            ViewBag.OwnerList = usersList;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Account account)
        {
            // CRITICAL FIX: Remove navigational and unmapped properties from validation
            ModelState.Remove("PostCode");
            ModelState.Remove("CsvPostcodeId");
            var model = new Account();

            if (User.Identity.IsAuthenticated)
            {
                model.AccountOwner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            if (ModelState.IsValid)
            {
                // Look up the PostCodeId from the database based on the zip string
                if (!string.IsNullOrEmpty(account.Postcode))
                {
                    var matchedPostcode = await _context.Postcodes.FirstOrDefaultAsync(p => p.ZipCode == account.Postcode);
                    if (matchedPostcode != null)
                    {
                        account.PostCodeId = matchedPostcode.Id;
                    }
                }

                _context.Account.Add(account);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Account created successfully!";
                return RedirectToAction("Index");
            }

            // If it fails, return the validation errors to the view
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["ErrorMessage"] = "Validation failed: " + string.Join(" | ", errors);

            return View(account);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usersList = await _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToListAsync();
            var accountsQuery = _context.Account.AsQueryable();

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
                    accountsQuery = accountsQuery.Where(a => allowedUserIds.Contains(a.AccountOwner));
                }
            }

            var accountsData = await accountsQuery.ToListAsync();

            // Map data for rapid JS rendering
            var accounts = accountsData.Select(a => {
                var ownerUser = usersList.FirstOrDefault(u => !string.IsNullOrEmpty(a.AccountOwner) && u.RecordId.Trim() == a.AccountOwner.Trim());
                string mappedOwnerName = ownerUser != null ? $"{ownerUser.FirstName} {ownerUser.LastName}" : (a.AccountOwner ?? "");

                return new
                {
                    accountId = a.AccountId,
                    accountName = !string.IsNullOrWhiteSpace(a.AccountName) ? a.AccountName.Trim() : "-",
                    accountOwner = !string.IsNullOrWhiteSpace(mappedOwnerName) ? mappedOwnerName : "-",
                    country = !string.IsNullOrWhiteSpace(a.Country) ? a.Country.Trim() : "-",
                    state = !string.IsNullOrWhiteSpace(a.State) ? a.State.Trim() : "-",
                    city = !string.IsNullOrWhiteSpace(a.City) ? a.City.Trim() : "-",
                    customerChannel = !string.IsNullOrWhiteSpace(a.CustomerChannel) ? a.CustomerChannel : "-",
                    customerActivity = !string.IsNullOrWhiteSpace(a.CustomerActivity) ? a.CustomerActivity : "-",
                    customerRelationship = !string.IsNullOrWhiteSpace(a.CustomerRelationship) ? a.CustomerRelationship : "-",
                    balance = a.Balance,
                    phone = !string.IsNullOrWhiteSpace(a.Phone) ? a.Phone : "-",
                    email = !string.IsNullOrWhiteSpace(a.Email) ? a.Email : "-",
                    customerGeography = !string.IsNullOrWhiteSpace(a.CustomerGeography) ? a.CustomerGeography : "-",
                    description = !string.IsNullOrWhiteSpace(a.Description) ? a.Description : "-",
                    website = !string.IsNullOrWhiteSpace(a.Website) ? a.Website : "-",
                    gstRegistrationNo = !string.IsNullOrWhiteSpace(a.GSTRegistrationNo) ? a.GSTRegistrationNo : "-",
                    erpCustomerCode = !string.IsNullOrWhiteSpace(a.ERPCustomerCode) ? a.ERPCustomerCode : "-",

                    // Dynamic Tabs based on Activity Status
                    tab = a.CustomerActivity == "Active" ? "Active" :
                          a.CustomerActivity == "Inactive" ? "Inactive" : "Other"
                };
            }).ToList();

            ViewBag.AccountsJson = System.Text.Json.JsonSerializer.Serialize(accounts);
            return View();
        }

        // NOTE: You can completely delete the old [HttpGet] public async Task<JsonResult> GetAccountsList() method now.

        [HttpPost]
        public IActionResult ImportCsv(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            HeaderValidated = null,
                            MissingFieldFound = null,
                            Delimiter = ",",
                            ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrWhiteSpace),
                            PrepareHeaderForMatch = args => args.Header.ToLower().Replace(" ", "").Replace(".", "")
                        };

                        using (var csv = new CsvReader(reader, csvConfig))
                        {
                            var dateOptions = new CsvHelper.TypeConversion.TypeConverterOptions
                            {
                                Formats = new[] {
                                    "dd-MM-yyyy HH:mm", "dd-MM-yyyy", "MM/dd/yyyy HH:mm",
                                    "yyyy-MM-dd", "M/d/yyyy", "d-M-yyyy HH:mm"
                                }
                            };
                            csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(dateOptions);
                            csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(dateOptions);

                            var allRecords = csv.GetRecords<Account>().ToList();

                            var validRecords = allRecords.Where(a =>
                                !string.IsNullOrWhiteSpace(a.AccountName)
                            ).ToList();

                            int skippedCount = allRecords.Count - validRecords.Count;

                            if (validRecords.Any())
                            {
                                var csvPostcodeIds = validRecords
                                    .Where(a => !string.IsNullOrWhiteSpace(a.CsvPostcodeId))
                                    .Select(a => a.CsvPostcodeId.Trim())
                                    .Distinct()
                                    .ToList();

                                var postcodeMap = _context.Postcodes
                                    .Where(p => csvPostcodeIds.Contains(p.RecordId))
                                    .ToDictionary(p => p.RecordId, p => p.Id);

                                foreach (var acc in validRecords)
                                {
                                    if (!string.IsNullOrWhiteSpace(acc.CsvPostcodeId))
                                    {
                                        string cleanCsvId = acc.CsvPostcodeId.Trim();
                                        if (postcodeMap.TryGetValue(cleanCsvId, out int realDbId))
                                        {
                                            acc.PostCodeId = realDbId;
                                        }
                                    }
                                    acc.PostCode = null;
                                }

                                _context.Account.AddRange(validRecords);
                                _context.SaveChanges();

                                string successMsg = $"{validRecords.Count} Accounts imported successfully!";
                                if (skippedCount > 0)
                                {
                                    successMsg += $" (Skipped {skippedCount} rows missing the Account Name).";
                                }
                                TempData["SuccessMessage"] = successMsg;
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Upload failed. None of the rows in the CSV had an Account Name filled out.";
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    var innerError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData["ErrorMessage"] = $"Database Error during import: {innerError}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a valid CSV file.";
            }

            return RedirectToAction("Index");
        }

        // ---------------------------------------------------
        // EDIT ACTIONS
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var account = await _context.Account.FindAsync(id);
            if (account == null) return NotFound();

            // Fetch the actual Full Name of the Owner to display as read-only
            string ownerName = account.AccountOwner; // Fallback to ID
            if (!string.IsNullOrEmpty(account.AccountOwner))
            {
                var ownerUser = await _context.Users.FirstOrDefaultAsync(u => u.RecordId == account.AccountOwner);
                if (ownerUser != null)
                {
                    ownerName = $"{ownerUser.FirstName} {ownerUser.LastName}";
                }
            }
            ViewBag.AccountOwnerName = ownerName;

            // --- NEW LOGIC: FETCH RELATED LIST COUNTS ---
            ViewBag.ContactCount = await _context.Contacts.CountAsync(c => c.AccountName == account.AccountName);
            ViewBag.SalesCallCount = await _context.Salescall.CountAsync(s => s.AccountName == account.AccountName);

            // Assuming Lead uses AccountNameId based on standard CRM mappings
            ViewBag.LeadCount = await _context.Set<LeadViewModel>().CountAsync(l => l.AccountNameId == account.AccountName);

            // Defaulting to 0 for tables that might not be fully mapped yet
            ViewBag.ComplaintCount = 0;
            // --------------------------------------------

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Account account)
        {
            if (id != account.AccountId) return NotFound();

            ModelState.Remove("PostCode");
            ModelState.Remove("CsvPostcodeId");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAccount = await _context.Account.FindAsync(id);
                    if (existingAccount == null) return NotFound();

                    if (!string.IsNullOrEmpty(account.Postcode) && account.Postcode != existingAccount.Postcode)
                    {
                        var matchedPostcode = await _context.Postcodes.FirstOrDefaultAsync(p => p.ZipCode == account.Postcode);
                        if (matchedPostcode != null)
                        {
                            account.PostCodeId = matchedPostcode.Id;
                        }
                    }
                    else if (string.IsNullOrEmpty(account.Postcode))
                    {
                        account.PostCodeId = null;
                    }
                    else
                    {
                        account.PostCodeId = existingAccount.PostCodeId;
                    }

                    _context.Entry(existingAccount).CurrentValues.SetValues(account);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Account updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!AccountExists(account.AccountId)) return NotFound();
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred: " + ex.Message;
                        throw;
                    }
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["ErrorMessage"] = "Validation failed: " + string.Join(" | ", errors);

            return View(account);
        }

        private bool AccountExists(int id)
        {
            return _context.Account.Any(e => e.AccountId == id);
        }

        // ---------------------------------------------------
        // API ENDPOINTS FOR DYNAMIC UI & DATATABLES
        // ---------------------------------------------------
        [HttpGet]
        public JsonResult GetUsers()
        {
            // FIX: Trim the Record ID to ensure the frontend JS dropdown binds correctly
            var users = _context.Users
                .Where(u => !string.IsNullOrEmpty(u.RecordId))
                .Select(u => new {
                    recordId = u.RecordId.Trim(),
                    fullName = u.FirstName + " " + u.LastName
                })
                .ToList();

            return Json(users);
        }

        [HttpGet]
        public JsonResult GetStates(string country)
        {
            // FIX: Trim state names and exclude nulls to fix the state filter
            var states = _context.Postcodes
                .Where(p => !string.IsNullOrEmpty(p.State_Name))
                .Select(p => p.State_Name.Trim())
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            return Json(states);
        }

        [HttpGet]
        public JsonResult GetPostcodes(string state)
        {
            if (string.IsNullOrEmpty(state)) return Json(null);
            string searchCode = state.Trim();

            if (state.Contains("(") && state.Contains(")"))
            {
                int startIndex = state.IndexOf("(") + 1;
                int endIndex = state.IndexOf(")");
                searchCode = state.Substring(startIndex, endIndex - startIndex).Trim();

                var postcodesByCode = _context.Postcodes
                    .Where(p => p.State_Code == searchCode)
                    .Select(p => p.ZipCode.Trim())
                    .Distinct().ToList();
                return Json(postcodesByCode);
            }
            else
            {
                var postcodesByName = _context.Postcodes
                    .Where(p => p.State_Name == searchCode || p.State_Code == searchCode)
                    .Select(p => p.ZipCode.Trim())
                    .Distinct().ToList();
                return Json(postcodesByName);
            }
        }

        [HttpGet]
        public JsonResult GetCity(string postcode)
        {
            var city = _context.Postcodes
                .Where(p => p.ZipCode == postcode)
                .Select(p => p.City)
                .FirstOrDefault();
            return Json(city);
        }



        [HttpGet]
        public async Task<JsonResult> GetAccountsList()
        {
            var usersList = await _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToListAsync();
            var accountsQuery = _context.Account.AsQueryable();

            // SECURITY FILTER: Check for Administrator Profile OR Role Hierarchy
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userProfile = User.Claims.FirstOrDefault(c => c.Type == "Profile")?.Value;

                if (userProfile != "Administrator" && !string.IsNullOrEmpty(currentUserId))
                {
                    var currentUser = usersList.FirstOrDefault(u => u.RecordId == currentUserId);
                    List<string> allowedUserIds = new List<string> { currentUserId }; // User can always see their own records

                    if (currentUser != null && !string.IsNullOrEmpty(currentUser.Role))
                    {
                        var currentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == currentUser.Role);

                        if (currentRole != null)
                        {
                            // 1. Fetch the whole tree and find all subordinate role IDs
                            var allRoles = await _context.Roles.ToListAsync();
                            var subordinateRoleIds = GetSubordinateRoleIds(allRoles, currentRole.Id);

                            // 2. Get the string names of all those subordinate roles
                            var allowedRoleNames = allRoles
                                .Where(r => subordinateRoleIds.Contains(r.Id))
                                .Select(r => r.Name)
                                .ToList();

                            // 3. Find all users who have those subordinate roles
                            var subordinateUserIds = usersList
                                .Where(u => allowedRoleNames.Contains(u.Role))
                                .Select(u => u.RecordId)
                                .ToList();

                            allowedUserIds.AddRange(subordinateUserIds);
                        }
                    }

                    // FILTER: Only show accounts where the owner is in the allowed list
                    accountsQuery = accountsQuery.Where(a => allowedUserIds.Contains(a.AccountOwner));
                }
            }

            // Execute the filtered query and map the names for the frontend
            var accountsData = await accountsQuery.ToListAsync();

            var accounts = accountsData.Select(a => {
                var ownerUser = usersList.FirstOrDefault(u => !string.IsNullOrEmpty(a.AccountOwner) && u.RecordId.Trim() == a.AccountOwner.Trim());
                string mappedOwnerName = ownerUser != null ? $"{ownerUser.FirstName} {ownerUser.LastName}" : (a.AccountOwner ?? "");

                return new
                {
                    accountId = a.AccountId,
                    accountName = a.AccountName?.Trim() ?? "",
                    accountOwner = mappedOwnerName,
                    country = a.Country?.Trim() ?? "",
                    state = a.State?.Trim() ?? "",
                    city = a.City?.Trim() ?? "",
                    customerChannel = a.CustomerChannel ?? "",
                    customerActivity = a.CustomerActivity ?? "",
                    customerRelationship = a.CustomerRelationship ?? "",
                    balance = a.Balance,
                    phone = a.Phone ?? "",
                    email = a.Email ?? "",
                    customerGeography = a.CustomerGeography ?? "",
                    description = a.Description ?? "",
                    website = a.Website ?? "",
                    gstRegistrationNo = a.GSTRegistrationNo ?? "",
                    erpCustomerCode = a.ERPCustomerCode ?? ""
                };
            }).ToList();

            return Json(accounts);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteBulk([FromBody] List<int> accountIds)
        {
            if (accountIds == null || !accountIds.Any())
            {
                return Json(new { success = false, message = "No accounts selected." });
            }

            try
            {
                var accountsToDelete = await _context.Account
                    .Where(a => accountIds.Contains(a.AccountId))
                    .ToListAsync();

                if (accountsToDelete.Any())
                {
                    _context.Account.RemoveRange(accountsToDelete);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = $"{accountsToDelete.Count} accounts deleted successfully." });
                }

                return Json(new { success = false, message = "Selected accounts not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting accounts: " + ex.Message });
            }
        }

        // --- HELPER METHOD ---
        // This recursively digs through your Roles table to find everyone below the current user
        private List<int> GetSubordinateRoleIds(List<Role> allRoles, int parentId)
        {
            var subordinateIds = new List<int>();
            var children = allRoles.Where(r => r.ParentRoleId == parentId).ToList();

            foreach (var child in children)
            {
                subordinateIds.Add(child.Id);
                // Recursively get the children of the children
                subordinateIds.AddRange(GetSubordinateRoleIds(allRoles, child.Id));
            }

            return subordinateIds;
        }
    }
}