using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Publisher_CRM.Data;
using Publisher_CRM.Models;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Publisher_CRM.Controllers
{
    public class PostcodeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostcodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // INDEX: LIST ALL POSTCODES
        // ==========================================
        [HttpGet]
        public IActionResult Index()
        {
            // Retrieves all postcodes, sorted alphabetically by ZipCode
            var postcodes = _context.Postcodes
                                    .OrderBy(p => p.ZipCode)
                                    .ToList();

            return View(postcodes);
        }

        // ==========================================
        // CREATE: SINGLE ENTRY
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Postcode postcode)
        {
            if (ModelState.IsValid)
            {
                _context.Postcodes.Add(postcode);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redirecting to Index after creation is usually better UI flow
            }
            return View(postcode);
        }

        [HttpGet]
        public JsonResult GetPostcodesList()
        {
            // By selecting ONLY the fields needed for the table, you drastically reduce the download size and speed up the query.
            var postcodes = _context.Postcodes
                .Select(p => new {
                    id = p.Id,
                    zipCode = p.ZipCode,
                    city = p.City,
                    state_Code = p.State_Code,
                    recordId = p.RecordId,
                    email = p.Email,
                    recordStatus = p.RecordStatus
                })
                .ToList();

            return Json(postcodes);
        }

        // ==========================================
        // BULLETPROOF IMPORT FUNCTIONALITY
        // ==========================================
        [HttpPost]
        public IActionResult ImportCsv(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HeaderValidated = null,
                        MissingFieldFound = null,
                        // Skip completely empty "ghost" rows from Excel
                        ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrWhiteSpace)
                    }))
                    {
                        var allRecords = csv.GetRecords<Postcode>().ToList();

                        // Filter out records that are missing the ZipCode data
                        var validRecords = allRecords.Where(p => !string.IsNullOrWhiteSpace(p.ZipCode)).ToList();

                        int skippedCount = allRecords.Count - validRecords.Count;

                        if (validRecords.Any())
                        {
                            _context.Postcodes.AddRange(validRecords);
                            _context.SaveChanges();

                            string successMsg = $"{validRecords.Count} Postcodes imported successfully!";
                            if (skippedCount > 0)
                            {
                                successMsg += $" (Skipped {skippedCount} rows that were blank).";
                            }

                            TempData["SuccessMessage"] = successMsg;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Upload failed. No valid Zipcodes were found in the CSV file.";
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // Expose actual database inner exception
                    var innerError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData["ErrorMessage"] = $"Database Error during import: {innerError}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a valid CSV file.";
            }

            return RedirectToAction("Index"); // Redirecting to Index makes it easy to see the newly imported data
        }
    }
}