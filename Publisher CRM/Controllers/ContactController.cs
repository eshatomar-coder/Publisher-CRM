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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Publisher_CRM.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHierarchyService _hierarchyService; // 1. Add this

        public ContactController(ApplicationDbContext context, IHierarchyService hierarchyService)
        {
            _context = context;
            _hierarchyService = hierarchyService;
        }

        // GET: Contact/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Contact/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new Contact();

            if (User.Identity.IsAuthenticated)
            {
                // We MUST use NameIdentifier (RecordId) because your database dropdowns map to IDs
                model.ContactOwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            // 1. Fetch all valid users from DB
            var usersFromDbList = _context.Users
                .Where(u => !string.IsNullOrEmpty(u.RecordId))
                .ToList();

            // 2. Populate the Record Owner dropdown list
            ViewBag.OwnerList = usersFromDbList.Select(u => new SelectListItem
            {
                Value = u.RecordId.Trim(),
                Text = $"{u.FirstName} {u.LastName}".Trim()
            }).ToList();

            // 3. THE FIX: Fetch the full Account objects so the frontend can read the AccountId!
            ViewBag.AccountsList = _context.Account
     .Select(a => new SelectListItem
     {
         Value = a.AccountId.ToString(), // The ID goes to the backend
         Text = a.AccountName            // The Name shows on the screen
     })
     .ToList();

            // (Optional) If your Contact Create page has a Lead Source dropdown, you can pass it like this:
            // ViewBag.LeadSources = new List<string> { "Advertisement", "Cold Call", "Employee Referral", "Partner", "Trade Show" };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contact contact)
        {
            // 1. CRITICAL FIX: Ensure the ContactOwner field gets populated BEFORE validation
            if (string.IsNullOrEmpty(contact.ContactOwnerId) && User.Identity.IsAuthenticated)
            {
                contact.ContactOwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            contact.ContactOwner = contact.ContactOwnerId;

            // 2. ROBUST LOOKUP: Check both AccountId (int) and RecordId (string)
            Account relatedAccount = null;
            if (!string.IsNullOrEmpty(contact.AccountNameId))
            {
                if (int.TryParse(contact.AccountNameId, out int accId))
                {
                    relatedAccount = _context.Account.FirstOrDefault(a => a.AccountId == accId);
                }

                if (relatedAccount == null)
                {
                    relatedAccount = _context.Account.FirstOrDefault(a => a.RecordId == contact.AccountNameId);
                }
            }

            if (relatedAccount == null && !string.IsNullOrEmpty(contact.AccountNameId))
            {
                ModelState.AddModelError("AccountNameId", "The specified Account does not exist.");
            }

            // 3. FIX: Clear validation errors for fields we populate manually or EF navigation properties
            ModelState.Remove("ContactOwner");
            ModelState.Remove("ContactOwnerId");
            ModelState.Remove("Account");
            ModelState.Remove("User");
            ModelState.Remove("ContactName");
            ModelState.Remove("AccountName");

            if (ModelState.IsValid)
            {
                // 4. ASSIGN THE NAME BEFORE SAVING
                if (relatedAccount != null)
                {
                    contact.AccountName = relatedAccount.AccountName;
                }

                if (string.IsNullOrWhiteSpace(contact.Email)) contact.Email = null;
                if (string.IsNullOrWhiteSpace(contact.Mobile)) contact.Mobile = null;

                try
                {
                    _context.Contacts.Add(contact);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Contact created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    Exception innerException = ex;
                    while (innerException.InnerException != null)
                    {
                        innerException = innerException.InnerException;
                    }
                    ModelState.AddModelError("", "Database save failed: " + innerException.Message);
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            if (errors.Any())
            {
                TempData["ErrorMessage"] = "Validation failed: " + string.Join(" | ", errors);
            }

            ViewBag.AccountsList = _context.Account.Select(a => new SelectListItem { Value = a.AccountId.ToString(), Text = a.AccountName }).ToList();
            var usersFromDbList = _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToList();
            ViewBag.OwnerList = usersFromDbList.Select(u => new SelectListItem { Value = u.RecordId.Trim(), Text = $"{u.FirstName} {u.LastName}".Trim() }).ToList();

            return View(contact);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return NotFound();

            // FIX: If the database is missing the Account Name, fetch it on the fly using the AccountNameId
            if (string.IsNullOrEmpty(contact.AccountName) && !string.IsNullOrEmpty(contact.AccountNameId))
            {
                Account relatedAccount = null;

                // First try matching it as an integer AccountId
                if (int.TryParse(contact.AccountNameId, out int accId))
                {
                    relatedAccount = await _context.Account.FirstOrDefaultAsync(a => a.AccountId == accId);
                }

                // If not found, try matching it as the string RecordId (e.g. "ACC-001")
                if (relatedAccount == null)
                {
                    relatedAccount = await _context.Account.FirstOrDefaultAsync(a => a.RecordId == contact.AccountNameId);
                }

                // If we found the account, attach the name so the Edit page can display it
                if (relatedAccount != null)
                {
                    contact.AccountName = relatedAccount.AccountName;
                }
            }

            ViewBag.Accounts = _context.Account.OrderBy(a => a.AccountName).ToList();

            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Text = u.FirstName + " " + u.LastName,
                    Value = u.FirstName + " " + u.LastName
                })
                .ToList();

            return View(contact);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contact contact)
        {
            if (id != contact.ContactId) return NotFound();

            // FIX: Clear validation errors for fields we populate manually or EF navigation properties
            ModelState.Remove("ContactOwner");
            ModelState.Remove("ContactOwnerId");
            ModelState.Remove("Account");
            ModelState.Remove("User");
            ModelState.Remove("ContactName");
            ModelState.Remove("AccountName");

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. ROBUST LOOKUP: Catch changes during editing
                    if (!string.IsNullOrEmpty(contact.AccountNameId))
                    {
                        Account relatedAccount = null;
                        if (int.TryParse(contact.AccountNameId, out int accId))
                        {
                            relatedAccount = await _context.Account.FirstOrDefaultAsync(a => a.AccountId == accId);
                        }

                        if (relatedAccount == null)
                        {
                            relatedAccount = await _context.Account.FirstOrDefaultAsync(a => a.RecordId == contact.AccountNameId);
                        }

                        if (relatedAccount != null)
                        {
                            contact.AccountName = relatedAccount.AccountName;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(contact.Email)) contact.Email = null;

                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Contact updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Contacts.Any(e => e.ContactId == contact.ContactId))
                        return NotFound();
                    else
                        throw;
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            if (errors.Any())
            {
                TempData["ErrorMessage"] = "Validation failed: " + string.Join(" | ", errors);
            }

            ViewBag.Accounts = _context.Account.OrderBy(a => a.AccountName).ToList();
            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem { Text = u.FirstName + " " + u.LastName, Value = u.FirstName + " " + u.LastName })
                .ToList();

            return View(contact);
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            var users = _context.Users
                .Select(u => new { recordId = u.RecordId, fullName = u.FirstName + " " + u.LastName })
                .ToList();
            return Json(users);
        }

        [HttpPost]
        public IActionResult ImportCsv(IFormFile file)
        {
            // ... (Your existing ImportCsv code remains exactly the same here) ...
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
                            var dateOptions = new CsvHelper.TypeConversion.TypeConverterOptions { Formats = new[] { "dd-MM-yyyy HH:mm", "dd-MM-yyyy", "dd/MM/yyyy HH:mm", "yyyy-MM-dd HH:mm", "M/d/yyyy", "yyyy-MM-dd" } };
                            csv.Context.TypeConverterOptionsCache.AddOptions<System.DateTime>(dateOptions);
                            csv.Context.TypeConverterOptionsCache.AddOptions<System.DateTime?>(dateOptions);

                            var allRecords = csv.GetRecords<Contact>().ToList();
                            var validAccountIds = _context.Account.Where(a => !string.IsNullOrEmpty(a.RecordId)).Select(a => a.RecordId.Trim().ToLower()).ToHashSet();
                            var validContactsToInsert = new List<Contact>();
                            int missingAccountCount = 0; int missingRequiredFieldsCount = 0;
                            var seenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            int duplicateEmailCountInFile = 0;

                            foreach (var record in allRecords)
                            {
                                string csvAccountId = record.AccountNameId?.Trim();
                                string csvLastName = record.LastName?.Trim();

                                if (string.IsNullOrWhiteSpace(csvLastName)) csvLastName = "Unknown";
                                if (string.IsNullOrWhiteSpace(csvAccountId)) { missingRequiredFieldsCount++; continue; }
                                if (!validAccountIds.Contains(csvAccountId.ToLower())) { missingAccountCount++; continue; }

                                if (string.IsNullOrWhiteSpace(record.Email)) record.Email = null;
                                else
                                {
                                    if (seenEmails.Contains(record.Email.Trim())) { duplicateEmailCountInFile++; continue; }
                                    seenEmails.Add(record.Email.Trim());
                                }

                                record.AccountNameId = csvAccountId;
                                record.LastName = csvLastName;
                                validContactsToInsert.Add(record);
                            }

                            if (validContactsToInsert.Any())
                            {
                                _context.Contacts.AddRange(validContactsToInsert);
                                _context.SaveChanges();
                                string msg = $"{validContactsToInsert.Count} Contacts imported successfully!";
                                if (missingAccountCount > 0) msg += $" Skipped {missingAccountCount} missing accounts.";
                                if (missingRequiredFieldsCount > 0) msg += $" Skipped {missingRequiredFieldsCount} missing IDs.";
                                if (duplicateEmailCountInFile > 0) msg += $" Skipped {duplicateEmailCountInFile} duplicate emails in the file.";
                                TempData["SuccessMessage"] = msg;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    string errorMsg = ex.Message;
                    if (ex.InnerException != null) errorMsg += " | Database Details: " + ex.InnerException.Message;
                    TempData["ErrorMessage"] = $"Import Error: {errorMsg}";
                }
            }
            else TempData["ErrorMessage"] = "Please select a valid CSV file.";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<JsonResult> GetContactsList()
        {
            // 1. Fetch accounts AND users into memory
            var accountsList = await _context.Account.ToListAsync();
            var usersList = await _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToListAsync();

            var contactsQuery = _context.Contacts.AsQueryable();

            // SECURITY FILTER: Check for Administrator Profile OR Role Hierarchy
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userProfile = User.Claims.FirstOrDefault(c => c.Type == "Profile")?.Value;

                // If NOT an Administrator, apply the role hierarchy filter
                if (userProfile != "Administrator" && !string.IsNullOrEmpty(currentUserId))
                {
                    var currentUser = usersList.FirstOrDefault(u => u.RecordId == currentUserId);

                    // 1. Every user can always see their own records
                    List<string> allowedUserIds = new List<string> { currentUserId };

                    if (currentUser != null && !string.IsNullOrEmpty(currentUser.Role))
                    {
                        var currentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == currentUser.Role);

                        if (currentRole != null)
                        {
                            // 2. Fetch the whole tree and find all subordinate role IDs
                            var allRoles = await _context.Roles.ToListAsync();
                            var subordinateRoleIds = GetSubordinateRoleIds(allRoles, currentRole.Id);

                            // 3. Get the string names of all those subordinate roles
                            var allowedRoleNames = allRoles
                                .Where(r => subordinateRoleIds.Contains(r.Id))
                                .Select(r => r.Name)
                                .ToList();

                            // 4. Find all users who are assigned to those subordinate roles
                            var subordinateUserIds = usersList
                                .Where(u => allowedRoleNames.Contains(u.Role))
                                .Select(u => u.RecordId)
                                .ToList();

                            allowedUserIds.AddRange(subordinateUserIds);
                        }
                    }

                    // 5. FILTER: Apply the list to the query to restrict visibility
                    contactsQuery = contactsQuery.Where(c => allowedUserIds.Contains(c.ContactOwner));
                }
            }

            // Execute the filtered query
            var contactsData = await contactsQuery.ToListAsync();

            var contacts = contactsData.Select(c =>
            {
                string finalAccountName = c.AccountName ?? "";

                // -- ACCOUNT NAME MAPPING --
                if (string.IsNullOrEmpty(finalAccountName) && !string.IsNullOrEmpty(c.AccountNameId))
                {
                    Account matchingAcc = null;
                    if (int.TryParse(c.AccountNameId, out int parsedId))
                    {
                        matchingAcc = accountsList.FirstOrDefault(a => a.AccountId == parsedId);
                    }

                    if (matchingAcc == null)
                    {
                        matchingAcc = accountsList.FirstOrDefault(a => a.RecordId == c.AccountNameId);
                    }

                    if (matchingAcc != null)
                    {
                        finalAccountName = matchingAcc.AccountName;
                    }
                }

                // -- CRITICAL FIX: OWNER NAME MAPPING --
                string mappedOwnerName = c.ContactOwner ?? "";
                if (!string.IsNullOrEmpty(mappedOwnerName))
                {
                    var ownerUser = usersList.FirstOrDefault(u => u.RecordId.Trim() == mappedOwnerName.Trim());
                    if (ownerUser != null)
                    {
                        mappedOwnerName = $"{ownerUser.FirstName} {ownerUser.LastName}".Trim();
                    }
                }

                return new
                {
                    contactId = c.ContactId,
                    contactName = !string.IsNullOrEmpty(c.ContactName) ? c.ContactName : (c.FirstName + " " + c.LastName),
                    accountName = finalAccountName,
                    salutation = c.Salutation ?? "",
                    title = c.Title ?? "",
                    department = c.Department ?? "",

                    email = c.Email ?? "",
                    phone = c.Phone ?? "",
                    otherPhone = c.OtherPhone ?? "",
                    mobile = c.Mobile ?? "",

                    salesCall = c.SalesCall,
                    dateOfBirth = c.DateOfBirth,
                    salespersonCode = c.SalespersonCode ?? "",
                    createdBy = c.CreatedBy ?? "",
                    modifiedBy = c.ModifiedBy ?? "",
                    connectedTo = c.ConnectedTo ?? "",
                    description = c.Description ?? "",
                    joiningDate = c.JoiningDate,

                    // Replaced the raw ID with the beautifully mapped First & Last Name
                    contactOwner = mappedOwnerName,

                    pipeline = c.Pipeline ?? "",
                    customerErpCode = c.CustomerErpCode ?? "",

                    createdTime = c.CreatedTime,
                    modifiedTime = c.ModifiedTime,
                    lastActivityTime = c.LastActivityTime,
                    userModifiedTime = c.UserModifiedTime,
                    userRelatedActivityTime = c.UserRelatedActivityTime,
                    systemModifiedTime = c.SystemModifiedTime,

                    mostRecentVisit = c.MostRecentVisit,
                    firstPageVisited = c.FirstPageVisited ?? "",
                    averageTimeSpentMinutes = c.AverageTimeSpentMinutes,
                    numberOfChats = c.NumberOfChats,
                    referrer = c.Referrer ?? "",
                    visitorScore = c.VisitorScore,
                    firstVisit = c.FirstVisit,
                    daysVisited = c.DaysVisited
                };
            }).ToList();

            // We must wrap this in { data = ... } so your DataTables UI fix continues to work!
            return Json(new { data = contacts });
        }

        // --- HELPER METHOD ---
        // Recursively digs through the Roles table to find everyone below the current user
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