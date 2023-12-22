using iCat.Localization.Demo.Models;
using iCat.Localization.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using iCat.Localization.Extensions;

namespace iCat.Localization.Demo.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            var converted = "My name is {#Name}".AddParams(new { });
            var resultA = "{Name}".Localize(); // current cultureInfo
            var resultB = "{Name}".Localize("en-US"); // specify cultureName
            return View();
        }
    }
}
