using iCat.Localization.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Demo.Controllers
{
    public class iCatStringLocalizerController : Controller
    {
        private readonly IStringLocalizer _stringLocalizer;

        public iCatStringLocalizerController(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var resultA = _stringLocalizer["Name"].Value;
            var resultB = _stringLocalizer.Localize("{Name}", CultureInfo.CurrentCulture.Name);

            return Ok($"{resultA}_{resultB}");
        }
    }
}
