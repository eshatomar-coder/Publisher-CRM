using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace Publisher_CRM.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Fetch roles for the dropdown
            ViewBag.RolesList = _context.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList();

            var user = new Users
            {
                UserStatus = "Active"
            };
            return View(user);
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Users user)
        {
            user.RecordId = Guid.NewGuid().ToString();
            user.Zuid = Guid.NewGuid().ToString();
            user.AddedTime = DateTime.Now;

            ModelState.Remove("RecordId");
            ModelState.Remove("Zuid");
            ModelState.Remove("Roles"); // Ignore the navigation property

            if (ModelState.IsValid)
            {
                // Sync the string Role name based on the selected RoleId
                if (user.RoleId.HasValue)
                {
                    var selectedRole = _context.Roles.Find(user.RoleId.Value);
                    if (selectedRole != null)
                    {
                        user.Role = selectedRole.Name;
                    }
                }

                _context.Users.Add(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction("Index");
            }

            // Repopulate dropdown if validation fails
            ViewBag.RolesList = _context.Roles
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["ErrorMessage"] = "Validation failed: " + string.Join(", ", errors);

            return View(user);
        }
        // GET: User/Edit/{id}
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Invalid User ID.";
                return RedirectToAction("Index");
            }

            var user = _context.Users.FirstOrDefault(u => u.RecordId == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Fetch roles for the dropdown
            ViewBag.RolesList = _context.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList();

            return View(user);
        }

        // GET: User/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Pass an empty Users model to the view. 
            // We removed the authentication check so this page ALWAYS loads first!
            return View(new Users());
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Users loginAttempt)
        {
            // 1. Tell ASP.NET to ignore the automatic validation errors (like Profile and RecordId)
            ModelState.Clear();

            if (!string.IsNullOrEmpty(loginAttempt.Email) && !string.IsNullOrEmpty(loginAttempt.Password))
            {
                // 2. Check if the Email exists and the user is Active (ignoring capitalization)
                var user = _context.Users.FirstOrDefault(u =>
                    u.Email == loginAttempt.Email &&
                    u.UserStatus != null && u.UserStatus.ToLower() == "active");

                if (user != null)
                {
                    // 3. Email found! Save the password they just typed into the database
                    user.Password = loginAttempt.Password;
                    _context.Users.Update(user);
                    _context.SaveChanges();

                    // 4. Build the user's session data
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.RecordId),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim("Profile", user.Profile ?? "General")
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // 5. Issue the login cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // 6. Redirect to your Home page after successful login
                    return RedirectToAction("Index", "Home");
                }

                // If user is null, the email didn't exist or wasn't active
                ModelState.AddModelError(string.Empty, "Email address not found in the system.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email and Password are required.");
            }

            // Return the view with the attempted email so they don't have to re-type it
            return View(loginAttempt);
        }

        [HttpPost]
        [AllowAnonymous] // Crucial: Allows the HRMS to bypass the authorization block
        [IgnoreAntiforgeryToken] // Crucial: Allows the cross-site form submission
        public async Task<IActionResult> SsoLogin(string username, string password, string returnUrl)
        {
            // 1. Tell ASP.NET to ignore automatic validation errors
            ModelState.Clear();

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // 2. Use Synchronous FirstOrDefault (exactly like your working CRM Login)
                // Check BOTH Email and RecordId to catch whatever HRMS sends
                var user = _context.Users.FirstOrDefault(u =>
                    (u.Email == username || u.RecordId == username) &&
                    u.UserStatus != null &&
                    u.UserStatus.ToLower() == "active");

                if (user != null)
                {
                    // 3. Update the password and SaveChanges synchronously
                    user.Password = password;
                    _context.Users.Update(user);
                    _context.SaveChanges();

                    // 4. Exact copy of the claims from your working CRM Login
                    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.RecordId),
        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Role, user.Role ?? "User"),
        new Claim("Profile", user.Profile ?? "General")
    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // 5. Issue the login cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // 6. Redirect to target path if valid, otherwise go to Home
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            // If login fails (wrong credentials or inactive), send them to the manual CRM login page
            return RedirectToAction("Login");
        }

        // POST: User/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }
        // POST: User/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Users updatedUser)
        {
            if (id != updatedUser.RecordId)
            {
                TempData["ErrorMessage"] = "Record ID mismatch.";
                return RedirectToAction("Index");
            }

            ModelState.Remove("Zuid");
            ModelState.Remove("Roles"); // Ignore the navigation property

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = _context.Users.FirstOrDefault(u => u.RecordId == id);
                    if (existingUser == null) return NotFound();

                    existingUser.FirstName = updatedUser.FirstName;
                    existingUser.LastName = updatedUser.LastName;
                    existingUser.Email = updatedUser.Email;

                    // Sync RoleId and the string Role name
                    existingUser.RoleId = updatedUser.RoleId;
                    if (existingUser.RoleId.HasValue)
                    {
                        var selectedRole = _context.Roles.Find(existingUser.RoleId.Value);
                        if (selectedRole != null)
                        {
                            existingUser.Role = selectedRole.Name;
                        }
                    }
                    else
                    {
                        existingUser.Role = null;
                    }

                    existingUser.Profile = updatedUser.Profile;
                    existingUser.Type = updatedUser.Type;
                    existingUser.Phone = updatedUser.Phone;
                    existingUser.Mobile = updatedUser.Mobile;
                    existingUser.UserStatus = updatedUser.UserStatus;
                    existingUser.SalesCallAssign = updatedUser.SalesCallAssign;

                    existingUser.Street = updatedUser.Street;
                    existingUser.City = updatedUser.City;
                    existingUser.State = updatedUser.State;
                    existingUser.ZipCode = updatedUser.ZipCode;
                    existingUser.Country = updatedUser.Country;
                    existingUser.TimeZone = updatedUser.TimeZone;
                    existingUser.Language = updatedUser.Language;
                    existingUser.CountryLocale = updatedUser.CountryLocale;

                    existingUser.TimeFormat = updatedUser.TimeFormat;
                    existingUser.DateFormat = updatedUser.DateFormat;
                    existingUser.Currency = updatedUser.Currency;
                    existingUser.SalespersonCode = updatedUser.SalespersonCode;

                    existingUser.ModifiedTime = DateTime.Now;

                    _context.Users.Update(existingUser);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Database Error: {ex.InnerException?.Message ?? ex.Message}";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Validation failed: " + string.Join(", ", errors);
            }

            // Repopulate dropdown if validation fails
            ViewBag.RolesList = _context.Roles
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();

            return View(updatedUser);
        }
        // ==========================================
        // IMPORT FUNCTIONALITY
        // ==========================================
        [HttpPost]
        public IActionResult ImportCsv(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HeaderValidated = null,
                        MissingFieldFound = null,
                        PrepareHeaderForMatch = args => args.Header.ToLower().Replace(" ", "").Replace("_", "")
                    };

                    using (var reader = new StreamReader(file.OpenReadStream()))
                    using (var csv = new CsvReader(reader, config))
                    {
                        var records = csv.GetRecords<Users>()
                                         .Where(u => !string.IsNullOrWhiteSpace(u.RecordId))
                                         .ToList();

                        foreach (var user in records)
                        {
                            if (string.IsNullOrWhiteSpace(user.AddedById)) user.AddedById = "SYSTEM_IMPORT";
                            if (string.IsNullOrWhiteSpace(user.ModifiedById)) user.ModifiedById = "SYSTEM_IMPORT";

                            foreach (PropertyInfo prop in typeof(Users).GetProperties())
                            {
                                if (prop.PropertyType == typeof(string))
                                {
                                    var currentValue = prop.GetValue(user) as string;
                                    if (currentValue == null)
                                    {
                                        prop.SetValue(user, "");
                                    }
                                }
                            }
                        }

                        if (records.Any())
                        {
                            _context.Users.AddRange(records);
                            _context.SaveChanges();
                            TempData["SuccessMessage"] = $"{records.Count} Users imported successfully!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "No valid records were found. Check your CSV headers.";
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    var actualError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData["ErrorMessage"] = $"Error importing file: {actualError}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a valid CSV file.";
            }

            return RedirectToAction("Create");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userProfile = User.Claims.FirstOrDefault(c => c.Type == "Profile")?.Value;

            var query = _context.Users.AsQueryable();

            if (userProfile != "Administrator")
            {
                // Regular users can only see their own user record
                query = query.Where(u => u.RecordId == currentUserId);
            }

            var allUsers = query.ToList();
            return View(allUsers);
        }
    }
}