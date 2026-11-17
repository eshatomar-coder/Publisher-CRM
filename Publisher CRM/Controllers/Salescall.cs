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
    public class SalesCallController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _uploadBasePath = @"C:\CRM_Publisher\SalesCalls";
        private readonly IHierarchyService _hierarchyService;

        // Static HttpClient prevents ERR_HTTP2_PROTOCOL_ERROR port exhaustion
        private static readonly HttpClient _proxyClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            UseProxy = false
        })
        { Timeout = TimeSpan.FromSeconds(15) };

        public SalesCallController(ApplicationDbContext context, IHierarchyService hierarchyService)
        {
            _context = context;
            _hierarchyService = hierarchyService;
        }

        // ==========================================
        // 1. INDEX (LIST VIEW & JSON API)
        // ==========================================
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetSalesCallsList()
        {
            var usersList = await _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToListAsync();
            var query = _context.Set<Salescall>().AsQueryable();

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

                            var allowedRoleNames = allRoles
                                .Where(r => subordinateRoleIds.Contains(r.Id))
                                .Select(r => r.Name)
                                .ToList();

                            var subordinateUserIds = usersList
                                .Where(u => allowedRoleNames.Contains(u.Role))
                                .Select(u => u.RecordId)
                                .ToList();

                            allowedUserIds.AddRange(subordinateUserIds);
                        }
                    }

                    query = query.Where(s => allowedUserIds.Contains(s.Owner));
                }
            }

            var callsData = await query.ToListAsync();

            var calls = callsData.Select(s =>
            {
                var ownerUser = usersList.FirstOrDefault(u => !string.IsNullOrEmpty(s.Owner) && u.RecordId.Trim() == s.Owner.Trim());
                string mappedOwner = ownerUser != null ? $"{ownerUser.FirstName} {ownerUser.LastName}".Trim() : (s.Owner ?? "-");

                var teamUser = usersList.FirstOrDefault(u => !string.IsNullOrEmpty(s.TeamMember) && u.RecordId.Trim() == s.TeamMember.Trim());
                string mappedTeamMember = teamUser != null ? $"{teamUser.FirstName} {teamUser.LastName}".Trim() : (s.TeamMember ?? "-");

                return new
                {
                    id = s.Id,
                    accountName = s.AccountName ?? "New/Event Customer",
                    salesCallName = s.SalesCallName ?? "-",
                    callDateTime = s.CallDateTime.ToString("yyyy-MM-dd HH:mm"),
                    owner = mappedOwner,
                    teamMember = mappedTeamMember,
                    associates = s.Associates ?? "-",
                    contacts = s.Contacts ?? "-",
                    callType = s.CallType ?? "-",
                    callSubType = s.CallSubType ?? "-",
                    eventId = s.EventId,
                    pipeline = s.Pipeline ?? "-",
                    customerRelationship = s.CustomerRelationship ?? "-",
                    customerChannel = s.CustomerChannel ?? "-",
                    customerActivity = s.CustomerActivity ?? "-",
                    customerGeography = s.CustomerGeography ?? "-",
                    customerZone = s.CustomerZone ?? "-",
                    customerProductProfile = s.CustomerProductProfile ?? "-",
                    potentialOrder = s.PotentialOrder ? "Yes" : "No",
                    sampleRequired = s.SampleRequired ? "Yes" : "No",
                    hasOrder = s.HasOrder ? "Yes" : "No",
                    orderType = s.OrderType ?? "-",
                    followUpDate = s.FollowUpDate.HasValue ? s.FollowUpDate.Value.ToString("yyyy-MM-dd") : "-",
                    remarks = s.Remarks ?? "-"
                };
            }).ToList();

            return Json(calls);
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

        [HttpGet]
        public async Task<JsonResult> GetAccountDetails(string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) return Json(null);

            var account = await _context.Account.FirstOrDefaultAsync(a => a.AccountName.ToLower() == accountName.Trim().ToLower());
            if (account == null) return Json(null);

            return Json(new
            {
                customerChannel = account.CustomerChannel,
                customerActivity = account.CustomerActivity,
                customerGeography = account.CustomerGeography,
                customerZone = account.CustomerZone,
                customerProductProfile = account.CustomerProductProfile,
                customerRelationship = account.CustomerRelationship
            });
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

        // ==========================================
        // 2. CREATE (GET & POST)
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            var model = new Salescall();

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.Identity.IsAuthenticated)
            {
                model.Owner = currentUserId;
            }

            var currentUserInfo = _context.Users.FirstOrDefault(u => u.RecordId == currentUserId);
            ViewBag.IsWholeSalesCall = (currentUserInfo != null && currentUserInfo.SalesCallAssign == "Whole SalesCall");

            var usersFromDbList = _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToList();
            ViewBag.OwnerList = usersFromDbList.Select(u => new SelectListItem { Value = u.RecordId.Trim(), Text = $"{u.FirstName} {u.LastName}".Trim() }).ToList();
            ViewBag.Users = usersFromDbList.Select(u => new SelectListItem { Text = $"{u.FirstName} {u.LastName}".Trim(), Value = u.RecordId.Trim() }).ToList();

            ViewBag.Accounts = _context.Account.Select(a => a.AccountName).ToList();
            ViewBag.Events = _context.EventMaster.Select(e => new SelectListItem { Text = e.EventName, Value = e.Id.ToString() }).ToList();
            ViewBag.Contacts = new List<string>();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromForm] Salescall model,
            int? EventId,
            string? EnquiryDataJson,
            List<string> SelectedCustomers,
            List<string> SelectedContacts,
            List<string> SelectedAssociates)
        {
            model.AccountName = SelectedCustomers != null && SelectedCustomers.Any() ? string.Join(", ", SelectedCustomers) : null;
            model.Contacts = SelectedContacts != null && SelectedContacts.Any() ? string.Join(", ", SelectedContacts) : null;
            model.Associates = SelectedAssociates != null && SelectedAssociates.Any() ? string.Join(", ", SelectedAssociates) : null;

            if (model.CallType != "Physical" || !EventId.HasValue)
            {
                if (string.IsNullOrEmpty(model.AccountName))
                {
                    ModelState.AddModelError("AccountName", "Customer is mandatory unless linking to a Physical Event.");
                }
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(model.Owner)) model.Owner = currentUserId;
            string safeOwner = model.Owner ?? "1";

            ModelState.Remove("UploadedFiles");
            ModelState.Remove("SalesCallName");
            ModelState.Remove("AccountName");
            ModelState.Remove("EnquiryItems");

            if (ModelState.IsValid)
            {
                try
                {
                    string accountLabel = string.IsNullOrEmpty(model.AccountName) ? "New/Event Customer" : model.AccountName.Split(',')[0];
                    model.SalesCallName = $"{accountLabel} - {DateTime.Now:yyyy-MM-dd HH:mm}";
                    model.EventId = EventId;

                    if (model.CallSubType == "Order") model.HasOrder = true;
                    if (model.CallSubType == "Complaint") model.IsComplaint = true;
                    if (model.CallSubType == "Merchandising") model.MerchandisingCheck = true;
                    if (model.CallSubType == "Enquiry") model.PotentialOrder = true;

                    if (!model.HasOrder && !model.PotentialOrder) model.OrderType = null;
                    if (!model.PotentialOrder) model.SampleRequired = false;

                    _context.Salescall.Add(model);
                    await _context.SaveChangesAsync();

                    // A. ENQUIRY ITEMS
                    if (model.CallSubType == "Enquiry" && !string.IsNullOrWhiteSpace(EnquiryDataJson))
                    {
                        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        options.Converters.Add(new NumberToStringConverter());
                        var enquiryItems = System.Text.Json.JsonSerializer.Deserialize<List<EnquiryPayloadDto>>(EnquiryDataJson, options);

                        if (enquiryItems != null && enquiryItems.Any())
                        {
                            foreach (var item in enquiryItems)
                            {
                                if (item == null) continue;
                                var newEnquiryItem = new SalesCallEnquiryItem
                                {
                                    SalesCallId = model.Id,
                                    BrandName = item.brandName,
                                    SeriesName = item.seriesName,
                                    Title = item.title,
                                    Isbn = item.isbn,
                                    Category = item.category,
                                    Quantity = (int)(item.enquiryDetails?.quantity ?? 1),
                                    TargetPrice = item.enquiryDetails?.targetPrice ?? 0,
                                    OfferedPrice = item.enquiryDetails?.offeredPrice ?? 0,
                                    UnitCost = item.enquiryDetails?.unitCost ?? 0,
                                    Specification = item.enquiryDetails?.specification,
                                    InternalRemark = item.enquiryDetails?.internalRemark,
                                    ExternalRemark = item.enquiryDetails?.externalRemark
                                };
                                _context.Set<SalesCallEnquiryItem>().Add(newEnquiryItem);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }

                    // B. COMPLAINT
                    if (model.CallSubType == "Complaint" && SelectedCustomers != null && SelectedCustomers.Any())
                    {
                        foreach (var customer in SelectedCustomers)
                        {
                            var newComplaint = new Complaints
                            {
                                AccountName = customer,
                                ComplaintsOwner = safeOwner,
                                ComplaintRemark = model.Remarks,
                                SalesCall = model.SalesCallName
                            };
                            _context.Set<Complaints>().Add(newComplaint);
                        }
                        await _context.SaveChangesAsync();
                    }

                    // C. ORDER (Updated to new OrderManagementViewModel fields)
                    if (model.CallSubType == "Order" && SelectedCustomers != null && SelectedCustomers.Any())
                    {
                        foreach (var customer in SelectedCustomers)
                        {
                            var newOrder = new OrderManagementViewModel
                            {
                                RecordId = Guid.NewGuid().ToString(),
                                OrderId = "ORD-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                                AccountNameId = customer,
                                SalesCallId = model.Id,
                                OrderManagementOwnerId = safeOwner,
                                Stage = "Open",
                                TentativeDispatchDate = DateTime.Now.AddDays(7),
                                OrderValue = 0m,
                                Currency = "INR",
                                OrderType = model.OrderType,
                                CreatedTime = DateTime.Now,
                                ModifiedTime = DateTime.Now
                            };
                            _context.Set<OrderManagementViewModel>().Add(newOrder);
                        }
                        await _context.SaveChangesAsync();
                    }

                    // D. ENQUIRY / LEAD (Updated to new LeadViewModel fields)
                    if (model.PotentialOrder && SelectedCustomers != null && SelectedCustomers.Any())
                    {
                        var ownerUser = await _context.Users.FirstOrDefaultAsync(u => u.RecordId == safeOwner);
                        bool isWholeSalesCall = ownerUser != null && ownerUser.SalesCallAssign == "Whole SalesCall";

                        if (!isWholeSalesCall)
                        {
                            foreach (var customer in SelectedCustomers)
                            {
                                var existingLead = await _context.Set<LeadViewModel>()
                                    .FirstOrDefaultAsync(l => l.AccountNameId == customer && l.SalesCallId == model.Id);

                                if (existingLead == null)
                                {
                                    var newLead = new LeadViewModel
                                    {
                                        RecordId = Guid.NewGuid().ToString(),
                                        EnquiryId = "ENQ-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                                        AccountNameId = customer,
                                        SalesCallId = model.Id,
                                        LeadOwnerId = safeOwner,
                                        Stage = "Open",
                                        EnquiryType = "Sales Call",
                                        Currency = "INR",
                                        CreatedTime = DateTime.Now,
                                        ModifiedTime = DateTime.Now
                                    };
                                    _context.Set<LeadViewModel>().Add(newLead);
                                }
                            }
                            await _context.SaveChangesAsync();
                        }
                    }

                    // E. MERCHANDISING
                    if (model.CallSubType == "Merchandising" && model.UploadedFiles != null && model.UploadedFiles.Any())
                    {
                        if (!Directory.Exists(_uploadBasePath)) Directory.CreateDirectory(_uploadBasePath);

                        foreach (var file in model.UploadedFiles)
                        {
                            if (file != null && file.Length > 0)
                            {
                                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                                string fullPhysicalPath = Path.Combine(_uploadBasePath, uniqueFileName);

                                using (var stream = new FileStream(fullPhysicalPath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var fileRecord = new Files
                                {
                                    ModuleName = "SalesCall",
                                    RecordId = model.Id.ToString(),
                                    FileName = file.FileName,
                                    FilePath = fullPhysicalPath
                                };
                                _context.File.Add(fileRecord);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // EVENT
                    if (EventId.HasValue && model.CallType == "Physical")
                    {
                        var existingEvent = await _context.EventMaster.FindAsync(EventId.Value);
                        if (existingEvent != null)
                        {
                            existingEvent.NumberOfCustomerMet += SelectedCustomers?.Count ?? 1;
                            existingEvent.ModifiedBy = User.Identity?.Name ?? "System";
                            _context.EventMaster.Update(existingEvent);
                            await _context.SaveChangesAsync();
                        }
                    }

                    TempData["SuccessMessage"] = "Sales Call processed and routed successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database error occurred: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }

            var usersFromDbList = _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToList();
            ViewBag.OwnerList = usersFromDbList.Select(u => new SelectListItem { Value = u.RecordId.Trim(), Text = $"{u.FirstName} {u.LastName}".Trim() }).ToList();
            ViewBag.Users = usersFromDbList.Select(u => new SelectListItem { Text = $"{u.FirstName} {u.LastName}".Trim(), Value = u.RecordId.Trim() }).ToList();
            ViewBag.Accounts = _context.Account.Select(a => a.AccountName).ToList();
            ViewBag.Events = _context.EventMaster.Select(e => new SelectListItem { Text = e.EventName, Value = e.Id.ToString() }).ToList();

            return View(model);
        }

        // ==========================================
        // 3. EDIT (GET & POST)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var salesCall = await _context.Salescall.FindAsync(id);
            if (salesCall == null) return NotFound();

            ViewBag.IsWholeSalesCall = true;

            var usersFromDb = _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToList();

            ViewBag.OwnerName = usersFromDb.FirstOrDefault(u => u.RecordId.Trim() == salesCall.Owner?.Trim()) is Users ownerUser
                ? $"{ownerUser.FirstName} {ownerUser.LastName}".Trim()
                : salesCall.Owner;

            ViewBag.OwnerList = usersFromDb.Select(u => new SelectListItem
            {
                Text = $"{u.FirstName} {u.LastName}".Trim(),
                Value = u.RecordId.Trim(),
                Selected = (u.RecordId.Trim() == salesCall.Owner?.Trim())
            }).ToList();

            ViewBag.Users = usersFromDb.Select(u => new SelectListItem
            {
                Text = $"{u.FirstName} {u.LastName}".Trim(),
                Value = u.RecordId.Trim()
            }).ToList();

            ViewBag.Accounts = _context.Account.Select(a => a.AccountName).ToList();

            ViewBag.Events = _context.EventMaster.Select(e => new SelectListItem
            {
                Text = e.EventName,
                Value = e.Id.ToString(),
                Selected = (e.Id == salesCall.EventId)
            }).ToList();

            var savedEnquiries = await _context.Set<SalesCallEnquiryItem>()
                .Where(e => e.SalesCallId == salesCall.Id)
                .Select(e => new
                {
                    e.BrandName,
                    e.SeriesName,
                    e.Title,
                    e.Isbn,
                    e.Category,
                    e.Quantity,
                    e.TargetPrice,
                    e.OfferedPrice,
                    e.UnitCost,
                    e.Specification,
                    e.InternalRemark,
                    e.ExternalRemark
                })
                .ToListAsync();

            ViewBag.SavedEnquiriesJson = JsonSerializer.Serialize(savedEnquiries);
            ViewBag.Attachments = await _context.File.Where(f => f.ModuleName == "SalesCall" && f.RecordId == salesCall.Id.ToString()).ToListAsync();

            return View(salesCall);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [FromForm] Salescall model,
            int? EventId,
            string? EnquiryDataJson,
            List<string> SelectedCustomers,
            List<string> SelectedContacts,
            List<string> SelectedAssociates)
        {
            if (model == null || id != model.Id) return NotFound();

            model.AccountName = SelectedCustomers != null && SelectedCustomers.Any() ? string.Join(", ", SelectedCustomers) : null;
            model.Contacts = SelectedContacts != null && SelectedContacts.Any() ? string.Join(", ", SelectedContacts) : null;
            model.Associates = SelectedAssociates != null && SelectedAssociates.Any() ? string.Join(", ", SelectedAssociates) : null;

            if (model.CallType != "Physical" || !EventId.HasValue)
            {
                if (string.IsNullOrEmpty(model.AccountName))
                {
                    ModelState.AddModelError("AccountName", "Customer is mandatory unless linking to a Physical Event.");
                }
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(model.Owner)) model.Owner = currentUserId;
            string safeOwner = model.Owner ?? "1";

            ModelState.Remove("UploadedFiles");
            ModelState.Remove("SalesCallName");
            ModelState.Remove("AccountName");
            ModelState.Remove("EnquiryItems");

            if (ModelState.IsValid)
            {
                try
                {
                    model.EventId = EventId;

                    if (model.CallSubType == "Order") model.HasOrder = true;
                    if (model.CallSubType == "Complaint") model.IsComplaint = true;
                    if (model.CallSubType == "Merchandising") model.MerchandisingCheck = true;
                    if (model.CallSubType == "Enquiry") model.PotentialOrder = true;

                    if (!model.HasOrder && !model.PotentialOrder) model.OrderType = null;
                    if (!model.PotentialOrder) model.SampleRequired = false;

                    _context.Update(model);
                    await _context.SaveChangesAsync();

                    // Update Enquiries
                    if (model.CallSubType == "Enquiry" && !string.IsNullOrWhiteSpace(EnquiryDataJson))
                    {
                        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        options.Converters.Add(new NumberToStringConverter());
                        var enquiryItems = System.Text.Json.JsonSerializer.Deserialize<List<EnquiryPayloadDto>>(EnquiryDataJson, options);

                        if (enquiryItems != null)
                        {
                            var existingItems = _context.Set<SalesCallEnquiryItem>().Where(e => e.SalesCallId == model.Id);
                            _context.Set<SalesCallEnquiryItem>().RemoveRange(existingItems);

                            foreach (var item in enquiryItems)
                            {
                                if (item == null) continue;
                                var newEnquiryItem = new SalesCallEnquiryItem
                                {
                                    SalesCallId = model.Id,
                                    BrandName = item.brandName,
                                    SeriesName = item.seriesName,
                                    Title = item.title,
                                    Isbn = item.isbn,
                                    Category = item.category,
                                    Quantity = (int)(item.enquiryDetails?.quantity ?? 1),
                                    TargetPrice = item.enquiryDetails?.targetPrice ?? 0,
                                    OfferedPrice = item.enquiryDetails?.offeredPrice ?? 0,
                                    UnitCost = item.enquiryDetails?.unitCost ?? 0,
                                    Specification = item.enquiryDetails?.specification,
                                    InternalRemark = item.enquiryDetails?.internalRemark,
                                    ExternalRemark = item.enquiryDetails?.externalRemark
                                };
                                _context.Set<SalesCallEnquiryItem>().Add(newEnquiryItem);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }

                    // Complaints
                    if (model.CallSubType == "Complaint" && SelectedCustomers != null && SelectedCustomers.Any())
                    {
                        foreach (var customer in SelectedCustomers)
                        {
                            var existing = await _context.Set<Complaints>().FirstOrDefaultAsync(c => c.AccountName == customer && c.SalesCall == model.SalesCallName);
                            if (existing == null)
                            {
                                var newComplaint = new Complaints
                                {
                                    AccountName = customer,
                                    ComplaintsOwner = safeOwner,
                                    ComplaintRemark = model.Remarks,
                                    SalesCall = model.SalesCallName
                                };
                                _context.Set<Complaints>().Add(newComplaint);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Orders
                    if (model.CallSubType == "Order" && SelectedCustomers != null && SelectedCustomers.Any())
                    {
                        foreach (var customer in SelectedCustomers)
                        {
                            var existing = await _context.Set<OrderManagementViewModel>().FirstOrDefaultAsync(o => o.AccountNameId == customer && o.SalesCallId == model.Id);
                            if (existing == null)
                            {
                                var newOrder = new OrderManagementViewModel
                                {
                                    RecordId = Guid.NewGuid().ToString(),
                                    OrderId = "ORD-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                                    AccountNameId = customer,
                                    SalesCallId = model.Id,
                                    OrderManagementOwnerId = safeOwner,
                                    Stage = "Open",
                                    TentativeDispatchDate = DateTime.Now.AddDays(7),
                                    OrderValue = 0m,
                                    Currency = "INR",
                                    OrderType = model.OrderType,
                                    CreatedTime = DateTime.Now,
                                    ModifiedTime = DateTime.Now
                                };
                                _context.Set<OrderManagementViewModel>().Add(newOrder);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Lead Integration
                    if (model.PotentialOrder && SelectedCustomers != null && SelectedCustomers.Any())
                    {
                        var ownerUser = await _context.Users.FirstOrDefaultAsync(u => u.RecordId == safeOwner);
                        bool isWholeSalesCall = ownerUser != null && ownerUser.SalesCallAssign == "Whole SalesCall";

                        if (!isWholeSalesCall)
                        {
                            foreach (var customer in SelectedCustomers)
                            {
                                var existingLead = await _context.Set<LeadViewModel>().FirstOrDefaultAsync(l => l.AccountNameId == customer && l.SalesCallId == model.Id);
                                if (existingLead == null)
                                {
                                    var newLead = new LeadViewModel
                                    {
                                        RecordId = Guid.NewGuid().ToString(),
                                        EnquiryId = "ENQ-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                                        AccountNameId = customer,
                                        SalesCallId = model.Id,
                                        LeadOwnerId = safeOwner,
                                        Stage = "Open",
                                        EnquiryType = "Sales Call",
                                        Currency = "INR",
                                        CreatedTime = DateTime.Now,
                                        ModifiedTime = DateTime.Now
                                    };
                                    _context.Set<LeadViewModel>().Add(newLead);
                                }
                            }
                            await _context.SaveChangesAsync();
                        }
                    }

                    // Merchandising
                    if (model.CallSubType == "Merchandising" && model.UploadedFiles != null && model.UploadedFiles.Any())
                    {
                        if (!Directory.Exists(_uploadBasePath)) Directory.CreateDirectory(_uploadBasePath);
                        foreach (var file in model.UploadedFiles)
                        {
                            if (file != null && file.Length > 0)
                            {
                                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                                string fullPhysicalPath = Path.Combine(_uploadBasePath, uniqueFileName);

                                using (var stream = new FileStream(fullPhysicalPath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var fileRecord = new Files
                                {
                                    ModuleName = "SalesCall",
                                    RecordId = model.Id.ToString(),
                                    FileName = file.FileName,
                                    FilePath = fullPhysicalPath
                                };
                                _context.File.Add(fileRecord);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Sales Call updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database error occurred: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }

            var usersFromDbList = _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToList();
            ViewBag.OwnerList = usersFromDbList.Select(u => new SelectListItem { Value = u.RecordId.Trim(), Text = $"{u.FirstName} {u.LastName}".Trim() }).ToList();
            ViewBag.Users = usersFromDbList.Select(u => new SelectListItem { Text = $"{u.FirstName} {u.LastName}".Trim(), Value = u.RecordId.Trim() }).ToList();
            ViewBag.Accounts = _context.Account.Select(a => a.AccountName).ToList();
            ViewBag.Events = _context.EventMaster.Select(e => new SelectListItem { Text = e.EventName, Value = e.Id.ToString() }).ToList();

            ViewBag.SavedEnquiriesJson = "[]";
            ViewBag.Attachments = await _context.File.Where(f => f.ModuleName == "SalesCall" && f.RecordId == model.Id.ToString()).ToListAsync();

            return View(model);
        }

        // ==========================================
        // 4. ATTACHMENT MANAGEMENT & APIs
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var fileRecord = await _context.File.FindAsync(id);
            if (fileRecord == null || string.IsNullOrEmpty(fileRecord.FilePath) || !System.IO.File.Exists(fileRecord.FilePath))
            {
                return NotFound("File not found on server.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fileRecord.FilePath);
            return File(fileBytes, "application/octet-stream", fileRecord.FileName);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteFile(int id)
        {
            try
            {
                var fileRecord = await _context.File.FindAsync(id);
                if (fileRecord == null) return Json(new { success = false, message = "File record not found." });

                if (!string.IsNullOrEmpty(fileRecord.FilePath) && System.IO.File.Exists(fileRecord.FilePath))
                {
                    System.IO.File.Delete(fileRecord.FilePath);
                }

                _context.File.Remove(fileRecord);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "File deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting file: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMarketingData()
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    var response = await client.GetAsync("http://192.168.0.88:8080/api/marketing/series-and-titles");
                    response.EnsureSuccessStatusCode();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    return Content(jsonString, "application/json");
                }
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = "API Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEnquiryApiData()
        {
            try
            {
                using var client = new HttpClient();
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
        public async Task<IActionResult> GetExternalImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return NotFound();

            try
            {
                var response = await _proxyClient.GetAsync(imageUrl);

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
                    return File(fileBytes, contentType);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load image: {ex.Message}");
                return NotFound();
            }
        }

        // ==========================================
        // 5. HELPER QUICK-CREATE LOGIC
        // ==========================================
        [HttpPost]
        public async Task<JsonResult> QuickCreateEvent(string eventName, string accountName)
        {
            if (string.IsNullOrWhiteSpace(eventName)) return Json(new { success = false, message = "Event Name is required." });

            try
            {
                var existingEvent = await _context.EventMaster.FirstOrDefaultAsync(e => e.EventName.ToLower() == eventName.Trim().ToLower());
                if (existingEvent != null) return Json(new { success = true, eventId = existingEvent.Id, eventName = existingEvent.EventName, isDuplicate = true });

                var newEvent = new EventMaster
                {
                    EventName = eventName.Trim(),
                    EventDateFrom = DateTime.Now,
                    EventDateTo = DateTime.Now.AddHours(1),
                    EventLocation = "TBD",
                    SalespersonCode = "TBD",
                    NumberOfCustomerMet = 0,
                    EventStatus = EventStatus.Open,
                    EventMasterOwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedBy = User.Identity?.Name ?? "System",
                    ModifiedBy = User.Identity?.Name ?? "System"
                };

                _context.EventMaster.Add(newEvent);
                await _context.SaveChangesAsync();

                return Json(new { success = true, eventId = newEvent.Id, eventName = newEvent.EventName, isDuplicate = false });
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "DB Error: " + errorMessage });
            }
        }

        [HttpPost]
        public async Task<JsonResult> QuickCreateContact(string firstName, string lastName, string accountName, string email, string title, string department, string mobile)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName)) return Json(new { success = false, message = "First and Last Name are required." });

            try
            {
                string fullName = $"{firstName.Trim()} {lastName.Trim()}";

                var existingContact = await _context.Contacts
                    .FirstOrDefaultAsync(c => c.FirstName.ToLower() == firstName.Trim().ToLower() &&
                                              c.LastName.ToLower() == lastName.Trim().ToLower() &&
                                              c.AccountName.ToLower() == accountName.Trim().ToLower());

                if (existingContact != null) return Json(new { success = true, contactName = fullName, isDuplicate = true });

                var newContact = new Contact
                {
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    AccountName = accountName.Trim(),
                    Email = email?.Trim(),
                    Title = title?.Trim(),
                    Department = department?.Trim(),
                    Mobile = mobile?.Trim(),
                };

                _context.Contacts.Add(newContact);
                await _context.SaveChangesAsync();

                return Json(new { success = true, contactName = fullName, isDuplicate = false });
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "DB Error: " + errorMessage });
            }
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            var users = _context.Users
                .Where(u => !string.IsNullOrEmpty(u.RecordId))
                .ToList()
                .Select(u => new
                {
                    recordId = u.RecordId.Trim(),
                    fullName = $"{u.FirstName} {u.LastName}".Trim()
                })
                .ToList();

            return Json(users);
        }

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
                            ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrWhiteSpace)
                        };

                        using (var csv = new CsvReader(reader, csvConfig))
                        {
                            var records = csv.GetRecords<Salescall>().ToList();
                            foreach (var record in records)
                            {
                                record.SalesCallName = $"{record.AccountName} - {DateTime.Now:yyyy-MM-dd}";
                            }
                            _context.Salescall.AddRange(records);
                            _context.SaveChanges();
                            TempData["SuccessMessage"] = $"{records.Count} Sales Calls imported successfully!";
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
                TempData["ErrorMessage"] = "Please select a valid CSV file.";
            }

            return RedirectToAction(nameof(Index));
        }
    }

    public class EnquiryPayloadDto
    {
        public string id { get; set; }
        public string brandName { get; set; }
        public string seriesName { get; set; }
        public string title { get; set; }
        public string isbn { get; set; }
        public string category { get; set; }
        public EnquiryDetailsDto enquiryDetails { get; set; }
    }

    public class EnquiryDetailsDto
    {
        public decimal quantity { get; set; }
        public decimal targetPrice { get; set; }
        public decimal offeredPrice { get; set; }
        public decimal unitCost { get; set; }
        public string specification { get; set; }
        public string internalRemark { get; set; }
        public string externalRemark { get; set; }
    }

    public class NumberToStringConverter : System.Text.Json.Serialization.JsonConverter<string>
    {
        public override string Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType == System.Text.Json.JsonTokenType.Number)
                return reader.GetDecimal().ToString(System.Globalization.CultureInfo.InvariantCulture);

            if (reader.TokenType == System.Text.Json.JsonTokenType.String)
                return reader.GetString();

            return null;
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, string value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}