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
            // {TestSentence} is "My name is {#Name}" in LocalizationMapping data;
            var converted = "{TestSentence}, Age is {#Age}, my school is {#School}".AddParams(new { Name = "Test", School = "School", Age = "99" });
            var resultA = converted.Localize(); // current cultureInfo
            var resultB = converted.Localize("en-US"); // specify cultureName
            return View();
        }
    }
}
