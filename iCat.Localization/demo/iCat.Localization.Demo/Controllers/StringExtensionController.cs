using Microsoft.AspNetCore.Mvc;
using iCat.Localization.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace iCat.Localization.Demo.Controllers
{
    public class StringExtensionController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var resultA = "My name is {Name}".Localize(); // current cultureInfo
            var resultB = "My name is {Name}".Localize(CultureInfo.CurrentCulture.Name); // specify cultureName
            return Ok($"{resultA}_{resultB}");
        }
    }
}
