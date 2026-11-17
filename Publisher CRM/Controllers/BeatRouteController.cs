using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher_CRM.Controllers
{
    public class BeatRouteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BeatRouteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.AllUsers = await _context.Users.OrderBy(u => u.FirstName).ToListAsync();
            ViewBag.Customers = await _context.Account.OrderBy(a => a.AccountName).ToListAsync();

            var routes = await _context.BeatRoutes
                .Include(r => r.SalesPerson)
                .Include(r => r.RouteCustomers)
                .ThenInclude(rc => rc.Customer)
                .OrderByDescending(r => r.BeatRouteId)
                .ToListAsync();

            // NEW: Fetch data specifically for the "Account View" toggle
            ViewBag.AccountViewData = await _context.BeatRouteCustomers
                .Include(rc => rc.Customer)
                .Include(rc => rc.BeatRoute)
                    .ThenInclude(br => br.SalesPerson)
                .OrderBy(rc => rc.Customer.AccountName)
                .ToListAsync();

            return View(routes);
        }

        [HttpPost]
        public async Task<IActionResult> SaveRoute(int routeId, string routeName, string salesPersonId, int[] customerIds)
        {
            try
            {
                BeatRoute route;

                if (routeId > 0)
                {
                    // Edit Mode
                    route = await _context.BeatRoutes
                        .Include(r => r.RouteCustomers)
                        .FirstOrDefaultAsync(r => r.BeatRouteId == routeId);

                    if (route == null) return Json(new { success = false, message = "Route not found." });

                    route.RouteName = routeName;
                    route.SalesPersonId = salesPersonId;

                    // Remove existing mappings
                    _context.BeatRouteCustomers.RemoveRange(route.RouteCustomers);
                }
                else
                {
                    // Create Mode
                    route = new BeatRoute { RouteName = routeName, SalesPersonId = salesPersonId };
                    _context.BeatRoutes.Add(route);
                }

                // Add new mappings
                if (customerIds != null && customerIds.Length > 0)
                {
                    foreach (var cid in customerIds)
                    {
                        route.RouteCustomers.Add(new BeatRouteCustomer { AccountId = cid });
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = routeId > 0 ? "Route updated successfully!" : "Beat Route created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 2. BEAT PLAN (Plan)
        // ==========================================
        public async Task<IActionResult> Plan()
        {
            ViewBag.Routes = await _context.BeatRoutes.Include(r => r.RouteCustomers).ThenInclude(rc => rc.Customer).ToListAsync();
            ViewBag.AllUsers = await _context.Users.OrderBy(u => u.FirstName).ToListAsync();

            // Get distinct cities from Postcodes
            ViewBag.Cities = await _context.Postcodes
                .Where(p => !string.IsNullOrEmpty(p.City))
                .Select(p => p.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var plans = await _context.BeatPlans
                .Include(p => p.BeatRoute)
                .Include(p => p.SalesPerson)
                .Include(p => p.PlanMeetings)
                .ThenInclude(m => m.Customer)
                .OrderByDescending(p => p.PlanDate)
                .ToListAsync();

            return View(plans);
        }

        [HttpGet]
        public async Task<IActionResult> GetRouteCustomers(int routeId)
        {
            var customers = await _context.BeatRouteCustomers
                .Where(rc => rc.BeatRouteId == routeId)
                .Select(rc => new { id = rc.AccountId, name = rc.Customer.AccountName })
                .ToListAsync();

            return Json(customers);
        }

        [HttpPost]
        public async Task<IActionResult> SavePlan([FromBody] BeatPlanRequestDto request)
        {
            try
            {
                var plan = new BeatPlan
                {
                    PlanDate = request.PlanDate,
                    BeatRouteId = request.BeatRouteId,
                    City = request.City,
                    SalesPersonId = request.SalesPersonId,
                    Associates = request.Associates // Saved as comma separated string
                };

                if (request.Meetings != null)
                {
                    foreach (var meeting in request.Meetings)
                    {
                        plan.PlanMeetings.Add(new PlanMeeting
                        {
                            AccountId = meeting.AccountId,
                            MeetingTime = TimeSpan.Parse(meeting.Time)
                        });
                    }
                }

                _context.BeatPlans.Add(plan);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Beat Plan scheduled successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class BeatPlanRequestDto
        {
            public DateTime PlanDate { get; set; }
            public int BeatRouteId { get; set; }
            public string? City { get; set; }
            public string SalesPersonId { get; set; }
            public string? Associates { get; set; }
            public MeetingDto[] Meetings { get; set; }
        }

        public class MeetingDto
        {
            public int AccountId { get; set; }
            public string Time { get; set; }
        }

        public async Task<IActionResult> DailyPlan()
        {
            ViewBag.AllUsers = await _context.Users.OrderBy(u => u.FirstName).ToListAsync();
            ViewBag.Customers = await _context.Account.OrderBy(a => a.AccountName).ToListAsync();

            // Get currently logged-in user ID to set as default
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            ViewBag.CurrentUserId = currentUserId;

            var plans = await _context.DailyPlans
                .Include(p => p.SalesPerson)
                .Include(p => p.CreatedBy)
                .Include(p => p.Customers).ThenInclude(c => c.Customer)
                .Include(p => p.Contacts).ThenInclude(c => c.Contact)
                .OrderByDescending(p => p.Date).ThenByDescending(p => p.Time)
                .ToListAsync();

            return View(plans);
        }

        [HttpGet]
        public async Task<IActionResult> GetContactsForCustomers(string customerIds)
        {
            if (string.IsNullOrEmpty(customerIds)) return Json(new List<object>());

            var ids = customerIds.Split(',').Select(int.Parse).ToList();

            // 1. Get Accounts to guarantee we have the correct names
            var accounts = await _context.Account
                .Where(a => ids.Contains(a.AccountId))
                .Select(a => new { a.AccountId, a.RecordId, a.AccountName })
                .ToListAsync();

            var accountNames = accounts.Select(a => a.AccountName).ToList();
            var recordIds = accounts.Where(a => !string.IsNullOrEmpty(a.RecordId)).Select(a => a.RecordId).ToList();
            var accountIdsString = ids.Select(id => id.ToString()).ToList();

            // 2. Fetch contacts
            var contactsDb = await _context.Contacts
                .Where(c =>
                    (c.AccountNameId != null && (recordIds.Contains(c.AccountNameId) || accountIdsString.Contains(c.AccountNameId))) ||
                    (c.AccountName != null && accountNames.Contains(c.AccountName))
                )
                .ToListAsync();

            // 3. Map the correct Account Name
            var result = contactsDb.Select(c => {
                var matchingAcc = accounts.FirstOrDefault(a =>
                    (c.AccountNameId != null && (a.RecordId == c.AccountNameId || a.AccountId.ToString() == c.AccountNameId)) ||
                    a.AccountName == c.AccountName);

                return new
                {
                    id = c.ContactId,
                    name = c.FirstName + " " + c.LastName,
                    accountName = matchingAcc?.AccountName ?? c.AccountName ?? "Unknown Account"
                };
            }).ToList();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> SearchAccounts(string searchTerm, int skip = 0)
        {
            var query = _context.Account.AsQueryable();

            // If a search term is provided, filter the query
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.AccountName.Contains(searchTerm));
            }

            // Load 50 accounts at a time based on the "skip" count
            var accounts = await query
                .OrderBy(a => a.AccountName)
                .Skip(skip)
                .Take(50)
                .Select(a => new {
                    accountId = a.AccountId,
                    accountName = a.AccountName
                })
                .ToListAsync();

            return Json(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> SaveDailyPlan([FromBody] DailyPlanRequestDto request)
        {
            try
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? request.SalesPersonId;

                DailyPlan plan;
                if (request.DailyPlanId > 0)
                {
                    plan = await _context.DailyPlans
                        .Include(p => p.Customers)
                        .Include(p => p.Contacts)
                        .FirstOrDefaultAsync(p => p.DailyPlanId == request.DailyPlanId);

                    _context.DailyPlanCustomers.RemoveRange(plan.Customers);
                    _context.DailyPlanContacts.RemoveRange(plan.Contacts);
                }
                else
                {
                    plan = new DailyPlan { CreatedById = currentUserId };
                    _context.DailyPlans.Add(plan);
                }

                plan.Date = request.Date;
                plan.Time = TimeSpan.Parse(request.Time);
                plan.CallType = request.CallType;
                plan.Place = request.Place;
                plan.SalesPersonId = request.SalesPersonId;
                plan.Associates = request.Associates;

                // Add Customers
                if (request.CustomerIds != null)
                {
                    foreach (var cId in request.CustomerIds)
                        plan.Customers.Add(new DailyPlanCustomer { AccountId = cId });
                }

                // Add Contacts
                if (request.ContactIds != null)
                {
                    foreach (var cId in request.ContactIds)
                        plan.Contacts.Add(new DailyPlanContact { ContactId = cId });
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Daily Plan saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class DailyPlanRequestDto
        {
            public int DailyPlanId { get; set; }
            public DateTime Date { get; set; }
            public string Time { get; set; }
            public string CallType { get; set; }
            public string Place { get; set; }
            public string SalesPersonId { get; set; }
            public string Associates { get; set; }
            public int[] CustomerIds { get; set; }
            public int[] ContactIds { get; set; }
        }
    } 
}
