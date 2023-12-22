# iCat.Localization

iCat.Localization is seamlessly integrated to the `Microsoft.Extensions.Localization` package and compatible with the [Microsoft's localization documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization). It can handle sentences, as well as multi-layer positioning and dynamic parameters.

## Installation
```bash
dotnet add package iCat.Localization
dotnet add package iCat.Localization.Extension.Web
```

## Configuration

### Using
````C#
    using iCat.Localization.Models;
    using iCat.Localization.Extensions;
    using iCat.Localization.Extension.Web;
````
---
### Razor View, Model Validation

The configuration is used to use localization in razor view pages and model validation

````C#
    builder.Services
      .AddControllersWithViews()
      .AddViewLocalization() // Localiztion in Razor View
      .AddDataAnnotationsLocalization(); // Localization in Model Validation
````
---
### CultureInfo supports list from request and key

Localizaion determines the current language through culture info in the current thread.
.net provide Request Middleware to handle culture from route/query string/cookie, the setting configure the key in them and supportes list

````C#
    // Configure cultureInfo from request and support list
    builder.Services.AddRequestLocalizationOptions(new Syst Globalization.  CultureInfo[] {
        new System.Globalization.CultureInfo("en-US"),
        new System.Globalization.CultureInfo("zh-TW"),
    }, "LangCode");  // key in Route/QueryString/Cookie
````
---
### Localization Resource

````C#
    // Configure localization data
    builder.Services.AddiCatLocalizationeService(neList<LocalizationMapping> {
        new LocalizationMapping {
            CultureName = "en-US",
            LanguageData = new Dictionary<string, string>{
                {"Error.Required", "Can't be null {0}"},
                {"Error.MaxLength", "{0} Length Over Than {1}"},
                {"Name", "Eric"},
                {"TestSentence","My name is {#Name}" }
            }
        },
        new LocalizationMapping {
            CultureName = "zh-TW",
            LanguageData = new Dictionary<string, string>{
                {"Error.Required", "{0} 不能為空"},
                {"Error.MaxLength", "超過 {1}，欄位{0}"},
                {"Name", "艾瑞克"},
                {"TestSentence","{#Name} 是我的名字" }
            }
        }
    }, new Options { EnableKeyNotFoundException = false });
````
---
### String Extension

 Initial string extension to make it easier to obtain localized strings

````C#
    // Configure string extension for iCat.Localization
    app.UseiCatLocalizationExtension();
````
---

## Reserved word
There are reserved words when localizer parser strings, so don't use these words in string for localization.

````
{
{#
}
^
##
@@
````

## Sample

### Microsoft IStringLocalizer

Sentence parser does not work with Microsoft IStringLocalizer, It can only use generic scenarios

````C#
using Microsoft.Extensions.Localization;

public class HomeController : Controller
{
    private readonly IStringLocalizer _stringLocalizer;

    public HomeController(IStringLocalizer stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public IActionResult Index()
    {
        var result = _stringLocalizer["Name"];
        return View();
    }
}

````
---
### IiCatStringLocalizer

````C#
using iCat.Localization.Interfaces;

public class HomeController : Controller
{
    private readonly IiCatStringLocalizer _iCatStringLocalizer;
    public HomeController(IiCatStringLocalizer iCatStringLocalizer)
    {
        _iCatStringLocalizer = iCatStringLocalizer;
    }
    public IActionResult Index()
    {
        var result = _iCatStringLocalizer.Localize("My name is {Name}", "en-US");
        return View();
    }
}
````
---
### String Extension

#### Generic usage

````C#
using iCat.Localization.Extensions;

public class HomeController : Controller
{
    public HomeController()
    {
    }
    public IActionResult Index()
    {
        var resultA = "My name is {Name}".Localize(); // current cultureInfo
        var resultB = "My name is {Name}".Localize("en-US"); // specify cultureName
        return View();
    }
}
````

#### Parameters

Reserved word "{# //property }" will get string from dynamic parameter.
The variable "converted" could be stored in database, and read then localized.

````C#
using iCat.Localization.Extensions;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var converted = "My name is {Name}, Age is {#Age}, my school is {#School}".AddParams(new { School = "School", Age = "99" });
        var resultA = converted.Localize(); // current cultureInfo
        var resultB = converted.Localize("en-US"); // specify cultureName
        return View();
    }
}
````
---

#### Multi-layer

````C#
using iCat.Localization.Extensions;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // {TestSentence} is "My name is {#Name}" in LocalizationMappingdata;
        var converted = "{TestSentence}, Age is {#Age}, my school is{#School}".AddParams(new { Name = "Test", School = "School", Age ="99" });
        var resultA = converted.Localize(); // current cultureInfo
        var resultB = converted.Localize("en-US"); // specify cultureName
        return View();
    }
}
````