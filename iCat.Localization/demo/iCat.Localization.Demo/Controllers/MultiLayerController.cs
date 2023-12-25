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
    public class MultiLayerController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // {TestSentence} is "My name is {#Name}" in LocalizationMappingdata;
            var converted = "{TestSentence}, Age is {#Age}, my school is{#School}".AddParams(new { Name = "Test", School = "School", Age = "99" });
            var resultA = converted.Localize(); // current cultureInfo
            var resultB = converted.Localize(CultureInfo.CurrentCulture.Name); // specify cultureName
            return Ok($"{resultA}_{resultB}");
        }
    }
}
