using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Demo.Controllers
{
    public class MSStringLocalizerController : Controller
    {
        private readonly IStringLocalizer _stringLocalizer;

        public MSStringLocalizerController(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var result = _stringLocalizer["Name"].Value;
            return Ok(result);
        }
    }
}
