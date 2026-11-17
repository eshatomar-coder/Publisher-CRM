using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Publisher_CRM.Controllers
{
    public class ProjectManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Populate ViewBag.Accounts for the dropdown here
            ViewBag.Accounts = _context.Account.Select(a => new SelectListItem { Value = a.AccountName, Text = a.AccountName }).ToList();

            var model = new ProjectTitle
            {
                RecordId = Guid.NewGuid().ToString(),
                TitleId = "TTL-" + DateTime.Now.ToString("yyMMddHHmmss") + new Random().Next(10, 99),
                Stage = "Open"
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectTitle model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Accounts = _context.Account.Select(a => new SelectListItem { Value = a.AccountName, Text = a.AccountName }).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await _context.Set<ProjectTitle>().FindAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ProjectTitle model)
        {
            if (id != model.RecordId) return NotFound();
            if (ModelState.IsValid)
            {
                model.ModifiedTime = DateTime.Now;
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectTitles()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userProfile = User.Claims.FirstOrDefault(c => c.Type == "Profile")?.Value;

            var query = _context.Set<ProjectTitle>().AsQueryable();

            if (userProfile != "Administrator" && !string.IsNullOrEmpty(currentUserId))
            {
                query = query.Where(p => p.OwnerId == currentUserId);
            }

            var titles = await query
                .OrderByDescending(p => p.CreatedTime)
                .Select(p => new {
                    recordId = p.RecordId,
                    titleId = p.TitleId ?? "-",
                    isbn = p.Isbn ?? "-",
                    type = p.Type ?? "-",
                    primaryItemNo = p.PrimaryItemNo ?? "-",
                    primaryItemName = p.PrimaryItemName ?? "-",
                    titleName = p.TitleName ?? "-",
                    language = p.Language ?? "-",
                    customerId = p.CustomerId ?? "-",
                    customerName = p.CustomerName ?? "-",
                    status = p.Stage ?? "Open",
                    remarks = p.Remarks ?? "-",
                    dispatchDate = p.TentativeDispatchDate.HasValue ? p.TentativeDispatchDate.Value.ToString("yyyy-MM-dd") : "-"
                })
                .ToListAsync();

            return Json(new { data = titles });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWorkflowAction(string recordId, string targetStatus, string remarks, DateTime? fileRecdDate)
        {
            if (string.IsNullOrEmpty(recordId) || string.IsNullOrEmpty(targetStatus))
                return Json(new { success = false, message = "Invalid parameters." });

            var project = await _context.Set<ProjectTitle>().FirstOrDefaultAsync(p => p.RecordId == recordId);
            if (project == null)
                return Json(new { success = false, message = "Title not found." });

            // Enforce Mandatory Remarks for Rejections
            if (targetStatus.Contains("Rejected") && string.IsNullOrWhiteSpace(remarks))
            {
                return Json(new { success = false, message = "Remarks are mandatory for rejections." });
            }

            project.Stage = targetStatus;
            project.ModifiedTime = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                project.Remarks = $"{DateTime.Now:dd-MMM yy}: {remarks} | {project.Remarks ?? ""}";
            }

            if (fileRecdDate.HasValue)
            {
                project.FileRecdDate = fileRecdDate.Value;
            }

            _context.Update(project);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Status updated to {targetStatus}" });
        }

    }
}