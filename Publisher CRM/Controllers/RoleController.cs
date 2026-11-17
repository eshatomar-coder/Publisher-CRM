using Microsoft.AspNetCore.Mvc;
using Publisher_CRM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher_CRM.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Roles
        public IActionResult Index()
        {
            var allRoles = _context.Roles.ToList();

            // Build the tree for the display
            var hierarchy = BuildHierarchy(allRoles, null);

            // Provide data for the "Reports To" dropdown in the modal
            ViewBag.ParentRoles = GetHierarchicalRolesList(allRoles);

            return View(hierarchy);
        }

        // POST: Roles/CreateAjax
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromForm] Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                // Return success to the Javascript handler
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Please fill in all required fields." });
        }

        // ADD THIS TO YOUR ROLES CONTROLLER

        // POST: Roles/DeleteAjax
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return Json(new { success = false, message = "Role not found." });

            // Prevent deleting a role if it has subordinates attached to it
            bool hasChildren = _context.Roles.Any(r => r.ParentRoleId == id);
            if (hasChildren)
                return Json(new { success = false, message = "Cannot delete this role. Reassign its subordinates first." });

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // GET: Roles/GetUsersByRole
        [HttpGet]
        public IActionResult GetUsersByRole(string roleName)
        {
            // Fetches users that have this exact role assigned to them
            var users = _context.Users
                .Where(u => u.Role == roleName)
                .Select(u => new
                {
                    fullName = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    status = u.UserStatus
                })
                .ToList();

            return Json(users);
        }

        // POST: Roles/EditAjax
        [HttpPost]
        public async Task<IActionResult> EditAjax([FromForm] Role role)
        {
            var existingRole = await _context.Roles.FindAsync(role.Id);
            if (existingRole == null)
            {
                return Json(new { success = false, message = "Role not found." });
            }

            // Prevent a role from reporting to itself
            if (role.Id == role.ParentRoleId)
            {
                return Json(new { success = false, message = "A role cannot report to itself." });
            }

            existingRole.Name = role.Name;
            existingRole.ParentRoleId = role.ParentRoleId;
            existingRole.Description = role.Description;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // --- Helper Methods ---

        private List<RoleHierarchyNode> BuildHierarchy(List<Role> allRoles, int? parentId)
        {
            return allRoles.Where(r => r.ParentRoleId == parentId)
                .Select(r => new RoleHierarchyNode
                {
                    Id = r.Id,
                    Name = r.Name,
                    Children = BuildHierarchy(allRoles, r.Id)
                }).ToList();
        }

        private List<SelectListItem> GetHierarchicalRolesList(List<Role> allRoles)
        {
            var selectList = new List<SelectListItem>();
            var rootRoles = allRoles.Where(r => r.ParentRoleId == null).ToList();

            foreach (var rootRole in rootRoles)
            {
                AddRoleToDropdown(rootRole, allRoles, selectList, 0);
            }
            return selectList;
        }

        private void AddRoleToDropdown(Role currentRole, List<Role> allRoles, List<SelectListItem> selectList, int level)
        {
            string indent = new string('-', level * 2);
            string prefix = level > 0 ? " " : "";

            selectList.Add(new SelectListItem
            {
                Value = currentRole.Id.ToString(),
                Text = $"{indent}{prefix}{currentRole.Name}"
            });

            var children = allRoles.Where(r => r.ParentRoleId == currentRole.Id).ToList();
            foreach (var child in children)
            {
                AddRoleToDropdown(child, allRoles, selectList, level + 1);
            }
        }
    }
}