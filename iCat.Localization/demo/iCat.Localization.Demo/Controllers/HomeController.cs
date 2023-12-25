using iCat.Localization.Demo.Models;
using iCat.Localization.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using iCat.Localization.Extensions;

namespace iCat.Localization.Demo.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IStringLocalizer stringLocalizer)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
