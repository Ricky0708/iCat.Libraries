using iCat.Localization.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Demo.Controllers
{
    public class ParametersController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var converted = "My name is {Name}, Age is {#Age}, my school is {#School}".AddParams(new { School = "School", Age = "99" });
            var resultA = converted.Localize(); // current cultureInfo
            var resultB = converted.Localize(CultureInfo.CurrentCulture.Name); // specify cultureName
            return Ok($"{resultA}_{resultB}");
        }
    }
}
