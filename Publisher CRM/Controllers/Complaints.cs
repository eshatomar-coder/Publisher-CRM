using Microsoft.AspNetCore.Mvc;
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
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComplaintsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // HELPER METHOD: Safely Load Users for Owner Dropdown
        // ==========================================
        private void LoadUserViewBag(string selectedOwnerId = null)
        {
            var userList = _context.Users
                .Where(u => !string.IsNullOrEmpty(u.RecordId))
                .Select(u => new {
                    RecordId = u.RecordId.Trim(),
                    FullName = u.FirstName + " " + u.LastName
                })
                .ToList();

            if (userList.Any())
            {
                // CRITICAL FIX: Bind the Value to RecordId and pre-select the correct owner
                ViewBag.UserList = new SelectList(userList, "RecordId", "FullName", selectedOwnerId);
            }
            else
            {
                ViewBag.UserList = new SelectList(new List<SelectListItem>());
            }
        }

        // ==========================================
        // INDEX - GET
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var complaints = await _context.Complaint.ToListAsync();

            // Safely create a dictionary to map RecordId to Full Name
            var users = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.RecordId))
                .Select(u => new {
                    Id = u.RecordId.Trim(),
                    Name = u.FirstName + " " + u.LastName
                })
                .ToListAsync();

            var ownerDict = new Dictionary<string, string>();
            foreach (var u in users)
            {
                ownerDict[u.Id] = u.Name; // Safely handles any duplicate IDs in your DB
            }

            ViewBag.OwnerNames = ownerDict;

            return View(complaints);
        }

        // ==========================================
        // CREATE - GET & POST
        // ==========================================
        public IActionResult Create()
        {
            LoadUserViewBag();
            return View(new Complaints());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Complaints model)
        {
            if (ModelState.IsValid)
            {
                _context.Complaint.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Complaint created successfully!";
                return RedirectToAction(nameof(Index));
            }

            LoadUserViewBag();
            return View(model);
        }

        // ==========================================
        // EDIT - GET & POST
        // ==========================================
        [HttpGet]
        // CHANGE THIS: int? id -> string id (if your DB uses strings for primary keys)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var complaint = await _context.Complaint.FindAsync(id);
            if (complaint == null) return NotFound();

            if (string.IsNullOrEmpty(complaint.ComplaintsOwner) && User.Identity.IsAuthenticated)
            {
                complaint.ComplaintsOwner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            LoadUserViewBag(complaint.ComplaintsOwner?.Trim());

            ViewBag.StatusList = new List<SelectListItem>
    {
        new SelectListItem { Text = "Open", Value = "Open" },
        new SelectListItem { Text = "Closed", Value = "Closed" }
    };

            return View(complaint);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Complaints model) // Change 'int' to 'string' here too if needed
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Complaint updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Complaint.Any(e => e.Id == model.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            // FIX: Pass the model's owner back to the ViewBag so the dropdown stays selected
            LoadUserViewBag(model.ComplaintsOwner?.Trim());

            // Remember to also reload your StatusList here or the View will crash!
            ViewBag.StatusList = new List<SelectListItem>
    {
        new SelectListItem { Text = "Open", Value = "Open" },
        new SelectListItem { Text = "Closed", Value = "Closed" }
    };

            return View(model);
        }
    }
}