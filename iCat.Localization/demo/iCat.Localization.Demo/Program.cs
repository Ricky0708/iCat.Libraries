using iCat.Localization.Models;
using iCat.Localization.Extensions;
using iCat.Localization.Extension.Web;
namespace iCat.Localization.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddControllersWithViews()
                .AddViewLocalization() // Localiztion in Razor View
                .AddDataAnnotationsLocalization(); // Localization in Model Validation

            // Configure cultureInfo from request and support list
            builder.Services.AddRequestLocalizationOptions(new System.Globalization.CultureInfo[] {
                new System.Globalization.CultureInfo("en-US"),
                new System.Globalization.CultureInfo("zh-TW"),
            }, "LangCode");  // key in Route/QueryString/Cookie

            // Configure localization data
            builder.Services.AddiCatLocalizationeService(new List<LocalizationMapping> {
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


            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Configure string extension for iCat.Localization
            app.UseiCatLocalizationExtension();

            app.Run();
        }
    }
}
