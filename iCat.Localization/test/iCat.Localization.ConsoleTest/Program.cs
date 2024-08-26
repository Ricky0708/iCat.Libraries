using iCat.Localization.Extensions;
using iCat.Localization.Implements;
using iCat.Localization.Models;

namespace iCat.Localization.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var mapping = new List<LocalizationMapping>() {
                new LocalizationMapping{ CultureName="en-US", LanguageData = [] },
                new LocalizationMapping{ CultureName="zh-TW", LanguageData = [] }
            };
            var provider = new DefaultiCatLocalizationDataProvider(mapping);
            var localizer = new StringLocalizer(provider);
            var factory = new StringLocalizerFactory(localizer);
            iCatLocalizationStringExtension.SetLocalizationFactory(factory);

            var obj = new
            {
                檔案 = "File",
                個人 = new
                {
                    名字 = "Name",
                    年紀 = 18,
                    生日 = new { Date = DateTime.Now }
                }
            };

            var result = localizer.AddParams("AA{#個人.名字}, BB{#個人.年紀}, CC{#個人.生日.Date}", obj);

            var a = obj.GetType().GetProperty("個人");

            Console.WriteLine(result);
            Console.WriteLine(result.Localize());

        }
    }
}
