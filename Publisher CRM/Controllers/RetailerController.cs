using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Publisher_CRM.Controllers
{
    public class RetailerPointController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RetailerPointController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // HELPER METHOD: Safely Load Users
        // ==========================================
        private void LoadUserViewBag(string selectedOwnerId = null)
        {
            var userList = _context.Users
                .Where(u => !string.IsNullOrEmpty(u.RecordId))
                .Select(u => new SelectListItem
                {
                    Value = u.RecordId.Trim(),
                    Text = u.FirstName + " " + u.LastName
                })
                .ToList();

            // Setup the SelectList with the pre-selected ID
            ViewBag.OwnerList = new SelectList(userList, "Value", "Text", selectedOwnerId);
        }
        

        public IActionResult Index()
        {
            // 1. Fetch all retailers and users from the database
            var allRetailers = _context.RetailerPointviews.ToList();
            var usersList = _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToList();

            // 2. Temporarily swap the ID with the Actual Name for the visual table
            foreach (var retailer in allRetailers)
            {
                if (!string.IsNullOrEmpty(retailer.OwnerName))
                {
                    var user = usersList.FirstOrDefault(u => u.RecordId.Trim() == retailer.OwnerName.Trim());
                    if (user != null)
                    {
                        // Replace the ID with the readable name for the View
                        retailer.OwnerName = $"{user.FirstName} {user.LastName}".Trim();
                    }
                }
            }

            return View(allRetailers);
        }

        // ==========================================
        // CREATE - GET
        // ==========================================
        public IActionResult Create()
        {
            var model = new RetailerPointview();
            if (User.Identity.IsAuthenticated)
            {
                // We MUST use NameIdentifier (RecordId) because your database dropdowns map to IDs
                model.OwnerName = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

        // ==========================================
        // EDIT - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var retailer = await _context.RetailerPointviews.FindAsync(id);
            if (retailer == null) return NotFound();

            // 1. DEFAULT TO LOGGED-IN USER: If the record has no owner, assign the current user
            if (string.IsNullOrEmpty(retailer.OwnerName) && User.Identity.IsAuthenticated)
            {
                retailer.OwnerName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            // 2. FETCH USERS & PRE-SELECT EXACT OWNER
            var usersFromDbList = await _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToListAsync();

            ViewBag.OwnerList = usersFromDbList.Select(u => new SelectListItem
            {
                Value = u.RecordId.Trim(),
                Text = $"{u.FirstName} {u.LastName}".Trim(),
                // This explicitly tells the dropdown to select this exact user
                Selected = (u.RecordId.Trim() == retailer.OwnerName?.Trim())
            }).ToList();

            // 3. LINKED ACCOUNT LOGIC
            var linkedAccount = await _context.Account
                .FirstOrDefaultAsync(a => a.AccountName == retailer.RetailerName);

            if (linkedAccount != null)
            {
                ViewBag.LinkedAccountId = linkedAccount.AccountId;
                ViewBag.LinkedAccountName = linkedAccount.AccountName;
            }

            return View(retailer);
        }

        // ==========================================
        // EDIT - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RetailerPointview model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Retailer updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.RetailerPointviews.Any(e => e.Id == model.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            LoadUserViewBag();
            return View(model);
        }

        // ==========================================
        // CREATE - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RetailerPointview model)
        {
            if (ModelState.IsValid)
            {
                _context.RetailerPointviews.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Retailer Point saved successfully!";
                return RedirectToAction("Index");
            }

            var errorList = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            ).Where(m => m.Value.Any());

            foreach (var error in errorList)
            {
                System.Diagnostics.Debug.WriteLine($"Field: {error.Key} | Error: {string.Join(", ", error.Value)}");
            }

            LoadUserViewBag();
            return View(model);
        }

        [HttpPost]
        public IActionResult ImportCsv(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                TempData["SuccessMessage"] = "File uploaded successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Please select a valid file.";
            return RedirectToAction("Index");
        }

       // ==========================================
// BULK CONVERT TO ACCOUNT (AJAX)
// ==========================================
[HttpPost]
public async Task<IActionResult> BulkConvertToAccount([FromBody] List<int> retailerIds)
{
    if (retailerIds == null || !retailerIds.Any())
    {
        return Json(new { success = false, message = "No records selected." });
    }

    try
    {
        var retailers = await _context.RetailerPointviews
            .Where(r => retailerIds.Contains(r.Id))
            .ToListAsync();

        int count = 0;
        int duplicates = 0;

        foreach (var retailer in retailers)
        {
            if (string.IsNullOrWhiteSpace(retailer.RetailerName)) continue;

            // Check if an account with this exact name already exists
            bool exists = await _context.Account.AnyAsync(a => a.AccountName.ToLower() == retailer.RetailerName.ToLower());
            
            if (!exists)
            {
                var newAccount = new Account
                {
                    AccountName = retailer.RetailerName,
                    AccountOwner = retailer.OwnerName,
                    Phone = retailer.MobileNumber,
                    Email = retailer.EmailAddress,
                    Address = retailer.Address,
                    Location = retailer.GoogleMapLocation
                };
                
                _context.Account.Add(newAccount);
                count++;
            }
            else
            {
                // Track if it was skipped because it already exists
                duplicates++; 
            }
        }

        // Save all newly created accounts to the database
        await _context.SaveChangesAsync();

        // Give a highly detailed response back to the frontend
        if (count == 0 && duplicates > 0)
        {
            return Json(new { success = true, message = $"0 created. {duplicates} Account(s) already exist with these names." });
        }

        return Json(new { success = true, message = $"{count} Retailer(s) successfully converted! ({duplicates} skipped as duplicates)." });
    }
    catch (Exception ex)
    {
        // CRITICAL: If the database rejects the save (e.g., missing a required field), 
        // this will catch the hidden error and display it on your screen.
        return Json(new { success = false, message = $"Database Error: {ex.InnerException?.Message ?? ex.Message}" });
    }
}

        // ==========================================
        // CONVERT TO ACCOUNT - POST 
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConvertToAccount(RetailerPointview model)
        {
            if (string.IsNullOrWhiteSpace(model.RetailerName))
            {
                TempData["ErrorMessage"] = "Retailer Name is missing. Cannot convert to Account.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // 1. Save or Update the Retailer First
                if (model.Id == 0)
                {
                    _context.RetailerPointviews.Add(model);
                }
                else
                {
                    _context.RetailerPointviews.Update(model);
                }
                await _context.SaveChangesAsync();

                // 2. Prevent Duplicate Accounts
                bool accountExists = await _context.Account.AnyAsync(a => a.AccountName.ToLower() == model.RetailerName.ToLower());

                if (!accountExists)
                {
                    // 3. Map to new Account
                    var newAccount = new Account
                    {
                        AccountName = model.RetailerName,
                        AccountOwner = model.OwnerName,
                        Address = model.Address,
                        Email = model.EmailAddress,
                        Phone = model.MobileNumber,
                        Location = model.GoogleMapLocation
                    };

                    _context.Account.Add(newAccount);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Retailer saved and Account created successfully!";
                }
                else
                {
                    TempData["Success"] = "Retailer updated (Account already existed).";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // If the database blocks the creation (e.g., a missing required field in the Account table),
                // this will catch it and show you the exact error on your screen.
                TempData["ErrorMessage"] = $"Account Creation Failed: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
    }
    
