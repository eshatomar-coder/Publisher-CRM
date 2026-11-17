using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Publisher_CRM.Controllers
{
    [Authorize]
    public class CatalogueController : Controller
    {
        // Catches the "My Catalogues" menu link
        public IActionResult Index()
        {
            return View();
        }

        // Catches the "Catalogue Maker" menu link
        public IActionResult Create()
        {
            return View();
        }
    }
}