using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Publisher_CRM.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _uploadBasePath = @"C:\CRM_Publisher\Orders";

        public OrderManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. INDEX & API
        // ==========================================
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersList()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userProfile = User.Claims.FirstOrDefault(c => c.Type == "Profile")?.Value;
            var usersList = await _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId)).ToListAsync();

            var query = _context.Set<OrderManagementViewModel>().AsQueryable();

            if (userProfile != "Administrator" && !string.IsNullOrEmpty(currentUserId))
            {
                query = query.Where(o => o.OrderManagementOwnerId == currentUserId);
            }

            // Fetch to memory first to avoid EF translation errors on string manipulation
            var rawOrders = await query.OrderByDescending(o => o.CreatedTime).ToListAsync();

            var result = rawOrders.Select(o =>
            {
                var ownerUser = usersList.FirstOrDefault(u => u.RecordId == o.OrderManagementOwnerId);
                string mappedOwner = ownerUser != null ? $"{ownerUser.FirstName} {ownerUser.LastName}" : (o.OrderManagementOwnerId ?? "-");

                return new
                {
                    recordId = o.RecordId,
                    orderId = o.OrderId ?? "-",
                    accountName = o.AccountNameId ?? "-",
                    orderType = o.OrderType ?? "-",
                    status = o.Stage ?? "-",
                    dispatchDate = o.TentativeDispatchDate.HasValue ? o.TentativeDispatchDate.Value.ToString("yyyy-MM-dd") : "-",
                    orderValue = (o.Currency ?? "INR") + " " + (o.OrderValue ?? 0).ToString("0.00"),
                    owner = mappedOwner
                };
            }).ToList();

            return Json(new { data = result });
        }

        [HttpGet]
        public async Task<JsonResult> GetSalesCallsByAccount(string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) return Json(new List<object>());
            var salesCalls = await _context.Set<Salescall>()
                .Where(s => s.AccountName != null && s.AccountName.Contains(accountName))
                .Select(s => new { value = s.Id.ToString(), text = s.SalesCallName })
                .ToListAsync();
            return Json(salesCalls);
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
        // 2. CREATE
        // ==========================================
        public IActionResult Create(string accountName = null, int? salesCallId = null)
        {
            var model = new OrderManagementViewModel
            {
                OrderId = "ORD-" + DateTime.Now.ToString("yyyyMMddHHmm"),
                AccountNameId = accountName,
                SalesCallId = salesCallId,
                Stage = "Open",
                TitleStatus = "Open",
                Currency = "INR"
            };

            if (User.Identity.IsAuthenticated)
            {
                model.OrderManagementOwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderManagementViewModel model)
        {
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(model.OrderManagementOwnerId))
                model.OrderManagementOwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedTime = DateTime.Now;
                    model.ModifiedTime = DateTime.Now;
                    model.AgreementPoFilePath = await SaveFileAsync(model.AgreementPoFile, null);

                    _context.Add(model);
                    await _context.SaveChangesAsync();

                    // Push Titles into the new Project Management Workflow engine
                    if (!string.IsNullOrWhiteSpace(model.OrderTitlesJson))
                    {
                        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var titles = System.Text.Json.JsonSerializer.Deserialize<List<OrderTitleItem>>(model.OrderTitlesJson, options);

                        if (titles != null && titles.Any())
                        {
                            foreach (var t in titles)
                            {
                                var projectTitle = new ProjectTitle
                                {
                                    RecordId = Guid.NewGuid().ToString(),
                                    TitleId = "TTL-" + DateTime.Now.ToString("yyMMddHHmmss") + new Random().Next(10, 99),
                                    OrderRecordId = model.RecordId,
                                    CustomerId = model.AccountNameId,
                                    CustomerName = model.AccountNameId, // Mapping to AccountName for now
                                    Isbn = t.Isbn,
                                    Type = t.Type,
                                    PrimaryItemNo = t.PrimaryItemNo,
                                    PrimaryItemName = t.PrimaryItemName,
                                    TitleName = t.TitleName,
                                    Language = t.Language,
                                    Stage = "Open",
                                    TentativeDispatchDate = model.TentativeDispatchDate,
                                    OwnerId = model.OrderManagementOwnerId
                                };
                                _context.Set<ProjectTitle>().Add(projectTitle);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }

                    TempData["SuccessMessage"] = "Order created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database error: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }

            PopulateDropdowns(model);
            return View(model);
        }

        // ==========================================
        // 3. EDIT
        // ==========================================
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var order = await _context.Set<OrderManagementViewModel>().FirstOrDefaultAsync(o => o.RecordId == id);
            if (order == null) return NotFound();

            PopulateDropdowns(order);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, OrderManagementViewModel model)
        {
            if (id != model.RecordId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    model.ModifiedTime = DateTime.Now;
                    model.AgreementPoFilePath = await SaveFileAsync(model.AgreementPoFile, model.AgreementPoFilePath);

                    _context.Update(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Order updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database error: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }

            PopulateDropdowns(model);
            return View(model);
        }

        // ==========================================
        // HELPERS
        // ==========================================
        private async Task<string> SaveFileAsync(IFormFile file, string existingPath)
        {
            if (file != null && file.Length > 0)
            {
                if (!Directory.Exists(_uploadBasePath)) Directory.CreateDirectory(_uploadBasePath);
                var fileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_uploadBasePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return filePath;
            }
            return existingPath;
        }

        private void PopulateDropdowns(OrderManagementViewModel model)
        {
            model.AccountsList = _context.Account.Select(a => new SelectListItem { Value = a.AccountName, Text = a.AccountName }).OrderBy(a => a.Text).ToList();

            try
            {
                model.OwnersList = _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId))
                    .Select(u => new SelectListItem { Value = u.RecordId, Text = u.FirstName + " " + u.LastName }).ToList();
            }
            catch { model.OwnersList = new List<SelectListItem>(); }

            model.OrderTypesList = new List<SelectListItem> {
                new SelectListItem { Value = "OEM", Text = "OEM" },
                new SelectListItem { Value = "White Labeling", Text = "White Labeling" },
                new SelectListItem { Value = "Distribution", Text = "Distribution" }
            };

            model.StatusList = new List<SelectListItem> {
                new SelectListItem { Value = "Open", Text = "Open" },
                new SelectListItem { Value = "In Working", Text = "In Working" },
                new SelectListItem { Value = "Closed - Won", Text = "Closed - Won" },
                new SelectListItem { Value = "Closed - Lost", Text = "Closed - Lost" }
            };

            model.TitleStatusList = new List<SelectListItem> {
                new SelectListItem { Value = "Open", Text = "Open" },
                new SelectListItem { Value = "Created", Text = "Created" }
            };

            model.CurrencyList = new List<SelectListItem> {
                new SelectListItem { Value = "INR", Text = "INR (₹)" },
                new SelectListItem { Value = "USD", Text = "USD ($)" },
                new SelectListItem { Value = "EUR", Text = "EUR (€)" },
                new SelectListItem { Value = "GBP", Text = "GBP (£)" }
            };
        }

        [HttpPost]
        public async Task<IActionResult> SyncBCOrders()
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    var authString = "bjain\\bjainapi:Pass@2025";
                    var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);

                    var url = "https://bjain-bc.bjain.com:8443/BJPL-BC250/ODataV4/Company('B.%20Jain%20Publishers%20Pvt.%20Ltd.')/Sales_Order_API";

                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = $"API Error: {response.StatusCode}" });
                    }

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var bcData = JsonSerializer.Deserialize<BcODataResponse>(jsonString, options);

                    if (bcData?.value != null && bcData.value.Any())
                    {
                        int ordersAdded = 0;
                        int ordersUpdated = 0;
                        string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                        // Pre-fetch Users to map Salesperson_Code efficiently
                        var crmUsers = await _context.Users.Where(u => !string.IsNullOrEmpty(u.RecordId) && !string.IsNullOrEmpty(u.SalespersonCode)).ToListAsync();

                        foreach (var bcOrder in bcData.value)
                        {
                            // -----------------------------------------------------
                            // 0. DETERMINE OWNER
                            // -----------------------------------------------------
                            string resolvedOwnerId = loggedInUserId; // Fallback to logged-in user

                            if (!string.IsNullOrWhiteSpace(bcOrder.Salesperson_Code))
                            {
                                var matchedUser = crmUsers.FirstOrDefault(u => u.SalespersonCode.Trim().Equals(bcOrder.Salesperson_Code.Trim(), StringComparison.OrdinalIgnoreCase));
                                if (matchedUser != null)
                                {
                                    resolvedOwnerId = matchedUser.RecordId;
                                }
                            }

                            // -----------------------------------------------------
                            // 1. SYNC ACCOUNT (Create or Update)
                            // -----------------------------------------------------
                            var account = await _context.Account.FirstOrDefaultAsync(a => a.BCAccountNo == bcOrder.Sell_to_Customer_No || a.ERPCustomerCode == bcOrder.Sell_to_Customer_No);

                            if (account != null)
                            {
                                // Update existing Account
                                account.AccountOwner = resolvedOwnerId;
                                _context.Update(account);
                            }
                            else if (!string.IsNullOrWhiteSpace(bcOrder.Sell_to_Customer_No))
                            {
                                // Create new Account
                                account = new Account
                                {
                                    RecordId = Guid.NewGuid().ToString(),
                                    BCAccountNo = bcOrder.Sell_to_Customer_No,
                                    ERPCustomerCode = bcOrder.Sell_to_Customer_No,
                                    AccountName = string.IsNullOrWhiteSpace(bcOrder.Sell_to_Customer_Name) ? "Unknown BC Account" : bcOrder.Sell_to_Customer_Name,
                                    AccountOwner = resolvedOwnerId
                                };
                                _context.Account.Add(account);
                                await _context.SaveChangesAsync(); // Save immediately so we can link it to the Contact
                            }

                            // -----------------------------------------------------
                            // 2. SYNC CONTACT (Create or Update)
                            // -----------------------------------------------------
                            Contact contact = null;
                            if (account != null && !string.IsNullOrWhiteSpace(bcOrder.Sell_to_Contact))
                            {
                                contact = await _context.Contacts.FirstOrDefaultAsync(c =>
                                    c.AccountNameId == account.RecordId &&
                                    c.ContactName == bcOrder.Sell_to_Contact);

                                if (contact != null)
                                {
                                    // Update existing Contact
                                    contact.ContactOwnerId = resolvedOwnerId;
                                    _context.Update(contact);
                                }
                                else
                                {
                                    // Create new Contact
                                    var nameParts = bcOrder.Sell_to_Contact.Trim().Split(' ', 2);
                                    string firstName = nameParts.Length > 1 ? nameParts[0] : "";
                                    string lastName = nameParts.Length > 1 ? nameParts[1] : nameParts[0];

                                    contact = new Contact
                                    {
                                        RecordId = Guid.NewGuid().ToString(),
                                        AccountNameId = account.RecordId,
                                        AccountName = account.AccountName,
                                        ContactName = bcOrder.Sell_to_Contact,
                                        FirstName = firstName,
                                        LastName = string.IsNullOrWhiteSpace(lastName) ? "-" : lastName,
                                        ContactOwnerId = resolvedOwnerId,
                                        CreatedTime = DateTime.Now
                                    };
                                    _context.Contacts.Add(contact);
                                    await _context.SaveChangesAsync();
                                }
                            }

                            // -----------------------------------------------------
                            // 3. SYNC ORDER (Create or Update)
                            // -----------------------------------------------------
                            var existingOrder = await _context.Set<OrderManagementViewModel>().FirstOrDefaultAsync(o => o.BcSoNo == bcOrder.No);

                            if (existingOrder != null)
                            {
                                // Update existing Order
                                existingOrder.OrderValue = bcOrder.Amount;
                                existingOrder.Stage = bcOrder.Status;
                                existingOrder.Currency = string.IsNullOrWhiteSpace(bcOrder.Currency_Code) ? "INR" : bcOrder.Currency_Code;
                                existingOrder.OrderManagementOwnerId = resolvedOwnerId;
                                existingOrder.ModifiedTime = DateTime.Now;

                                _context.Update(existingOrder);
                                ordersUpdated++;
                            }
                            else
                            {
                                // Create new Order
                                var newOrder = new OrderManagementViewModel
                                {
                                    RecordId = Guid.NewGuid().ToString(),
                                    OrderId = "ORD-BC-" + DateTime.Now.ToString("yyMMddHHmm") + new Random().Next(10, 99),
                                    BcSoNo = bcOrder.No,
                                    AccountNameId = account != null ? account.AccountName : bcOrder.Sell_to_Customer_Name,
                                    ContactNameId = contact != null ? contact.ContactName : bcOrder.Sell_to_Contact,
                                    BcSoDate = bcOrder.Document_Date,
                                    OrderValue = bcOrder.Amount,
                                    Currency = string.IsNullOrWhiteSpace(bcOrder.Currency_Code) ? "INR" : bcOrder.Currency_Code,
                                    Stage = bcOrder.Status,
                                    CreatedTime = DateTime.Now,
                                    ModifiedTime = DateTime.Now,
                                    OrderManagementOwnerId = resolvedOwnerId
                                };

                                _context.Add(newOrder);
                                ordersAdded++;
                            }
                        }

                        // Final save for all batched Order and Account/Contact updates
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = $"Sync complete: {ordersAdded} orders added, {ordersUpdated} orders updated." });
                    }

                    return Json(new { success = true, message = "No orders found to sync." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Sync failed: {ex.Message}" });
            }
        }

    }
}