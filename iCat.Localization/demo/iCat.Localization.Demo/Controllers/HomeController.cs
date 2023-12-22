using iCat.Localization.Extensions;
using iCat.Localization.Interfaces;
using iCat.Localization.WebApplicationTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace iCat.Localization.WebApplicationTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IiCatStringLocalizer _rncLocalizaionProcessor;

        public HomeController(ILogger<HomeController> logger,
            IStringLocalizer stringLocalizer,
            IiCatStringLocalizer rncLocalizaionProcessor)
        {
            _logger = logger;
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            _rncLocalizaionProcessor = rncLocalizaionProcessor ?? throw new ArgumentNullException(nameof(rncLocalizaionProcessor));
        }

        [HttpGet]
        public IActionResult Index()
        {
            var a = _stringLocalizer.GetString("Name");
            var b = _rncLocalizaionProcessor.Localize("{Name}", "zh-TW");
            var c = "{Name}".Localize();
            return View();
        }

        [HttpPost]
        public IActionResult Post([FromBody] TestModel testModel)
        {
            if (ModelState.IsValid)
            {
                var a = _stringLocalizer.GetString("Name");
                var b = _rncLocalizaionProcessor.Localize("{Name}", "zh-TW");
                var c = "{Name}".Localize();
                var d = _stringLocalizer.GetString("TestSentence", testModel);
                var e = _rncLocalizaionProcessor.LangAddParams("{TestSentence}", testModel).Localize("zh-TW");
                var f = "{TestSentence}".LangAddParams(testModel).Localize();
                var g = _stringLocalizer.GetString("TestSentence", testModel);
                var h = _rncLocalizaionProcessor.LangAddParams("{TestSentence}", testModel).Localize("zh-TW");
                var i = "{TestSentence}".LangAddParams(testModel).Localize();
            }
            else
            {
                var n = 1;
            }

            return BadRequest(ModelState);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
