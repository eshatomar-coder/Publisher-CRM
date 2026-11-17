using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Publisher_CRM.Controllers
{
    // FIX 1: Added ": Controller" to inherit MVC properties like ViewBag, ModelState, etc.
    public class EventMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EventMaster/Create
        public IActionResult Create()
        {
            // FIX 2: Using ViewBag.Users and merging FirstName and LastName
            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Text = u.FirstName + " " + u.LastName,
                    Value = u.FirstName + " " + u.LastName
                })
                .ToList();

            return View();
        }



        // GET: EventMaster/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // 1. Fetch the specific event data from the database
            var eventMaster = await _context.EventMaster.FindAsync(id);
            if (eventMaster == null) return NotFound();

            // 2. CRITICAL FIX FOR BLANK PAGE: 
            // The .Where clause prevents null RecordIds from crashing the dropdown
            ViewBag.Users = _context.Users
                .Where(u => !string.IsNullOrEmpty(u.RecordId))
                .Select(u => new SelectListItem
                {
                    Text = u.FirstName + " " + u.LastName,
                    Value = u.RecordId.Trim() // Matches the Owner ID
                })
                .ToList();

            // 3. Pass the fetched data directly into the view to pre-fill the inputs
            return View(eventMaster);
        }

        // ==========================================
        // 1. INDEX (VIEW & JSON API)
        // ==========================================

        [HttpGet]
        public IActionResult Index()
        {
            // Simply returns the Index.cshtml view
            return View();
        }

        // Add this to your EventMasterController
        [HttpGet]
        public JsonResult GetEventsList()
        {
            try
            {
                // 1. Get the current logged-in user ID safely
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

                // 2. Safely load ONLY necessary user fields to prevent memory overload
                var usersList = _context.Users
                    .Where(u => !string.IsNullOrEmpty(u.RecordId))
                    .Select(u => new { u.RecordId, u.FirstName, u.LastName })
                    .ToList();

                var events = _context.EventMaster
                    .Where(e => e.EventMasterOwnerId == currentUserId) // Filters for logged-in user
                    .ToList() // 3. Execute DB query first
                    .Select(e => {
                        // 4. Find the matching user safely
                        var ownerUser = usersList.FirstOrDefault(u =>
                            !string.IsNullOrEmpty(e.EventMasterOwnerId) &&
                            u.RecordId.Trim() == e.EventMasterOwnerId.Trim());

                        string mappedOwnerName = ownerUser != null
                            ? $"{ownerUser.FirstName} {ownerUser.LastName}".Trim()
                            : (e.EventMasterOwnerId ?? "-");

                        // 5. Return mapped fields with safe type conversions
                        return new
                        {
                            id = e.Id,
                            eventName = e.EventName ?? "-",
                            eventMasterOwnerId = mappedOwnerName,

                            // CRITICAL FIX: Convert dates and enums to strings to prevent JSON serialization crashes
                            eventDateFrom = e.EventDateFrom?.ToString("yyyy-MM-dd") ?? "-",
                            eventDateTo = e.EventDateTo?.ToString("yyyy-MM-dd") ?? "-",
                            eventLocation = e.EventLocation ?? "-",
                            eventStatus = e.EventStatus.ToString(),

                            numberOfCustomerMet = e.NumberOfCustomerMet,
                            salespersonCode = e.SalespersonCode ?? "-",
                            rentalCharges = e.RentalCharges,
                            travelCosts = e.TravelCosts,
                            designingAndSetupCharges = e.DesigningAndSetupCharges,
                            otherCosts = e.OtherCosts
                        };
                    })
                    .ToList();

                return Json(new { data = events });
            }
            catch (Exception ex)
            {
                // If it fails, DataTables will read this 'error' property and tell you exactly why
                return Json(new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }
        // POST: EventMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventMaster eventMaster)
        {
            if (id != eventMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Update the Modified By field to whoever is saving the edit
                    string currentUser = User.Identity?.Name ?? "System";
                    eventMaster.ModifiedBy = currentUser;

                    // 2. Logic for Event Close Date
                    if (eventMaster.EventStatus == EventStatus.Closed && eventMaster.EventCloseDate == null)
                    {
                        // If just closed, stamp the current time
                        eventMaster.EventCloseDate = DateTime.Now;
                    }
                    else if (eventMaster.EventStatus != EventStatus.Closed)
                    {
                        // If they change it back to Open/None, remove the close date
                        eventMaster.EventCloseDate = null;
                    }

                    _context.Update(eventMaster);
                    await _context.SaveChangesAsync();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
                {
                    if (!_context.EventMaster.Any(e => e.Id == eventMaster.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, repopulate the dropdown so the user can try again
            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Text = u.FirstName + " " + u.LastName,
                    Value = u.FirstName + " " + u.LastName
                })
                .ToList();

            return View(eventMaster);
        }



        // POST: EventMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventMaster eventMaster)
        {
            if (ModelState.IsValid)
            {
                // 1. Auto-fill the hidden fields
                // Used User.Identity?.Name to prevent null reference errors
                string currentUser = User.Identity?.Name ?? "System";

                eventMaster.CreatedBy = currentUser;
                eventMaster.ModifiedBy = currentUser;

                // 2. Logic for Event Close Date
                if (eventMaster.EventStatus == EventStatus.Closed)
                {
                    eventMaster.EventCloseDate = DateTime.Now;
                }

                _context.Add(eventMaster);
                await _context.SaveChangesAsync();

                // Redirect to list page after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, repopulate the dropdown so the user can try again
            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Text = u.FirstName + " " + u.LastName,
                    Value = u.FirstName + " " + u.LastName
                })
                .ToList();

            return View(eventMaster);
        }
    }
}