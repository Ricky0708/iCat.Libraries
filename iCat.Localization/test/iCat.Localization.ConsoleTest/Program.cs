using iCat.Localization.Extensions;
using iCat.Localization.Implements;
using iCat.Localization.Models;
using System.Linq.Expressions;
using System.Reflection;

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

            var names = new List<string>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                ParseNames(prop, ref names, "");
            }

            var a = obj.GetType().GetProperty("個人");

            Console.WriteLine(result);
            Console.WriteLine(result.Localize());

        }

        private static void ParseNames(PropertyInfo prop, ref List<string> lst, string parentName)
        {
            parentName = string.IsNullOrEmpty(parentName) ? prop.Name : $"{parentName}.{prop.Name}";
            var propName = prop.Name;
            if (prop.PropertyType == typeof(string))
            {
                lst.Add(parentName);
            }
            else if (prop.PropertyType.IsPrimitive &&
               (prop.PropertyType == typeof(byte) ||
                prop.PropertyType == typeof(sbyte) ||
                prop.PropertyType == typeof(short) ||
                prop.PropertyType == typeof(ushort) ||
                prop.PropertyType == typeof(int) ||
                prop.PropertyType == typeof(uint) ||
                prop.PropertyType == typeof(long) ||
                prop.PropertyType == typeof(ulong) ||
                prop.PropertyType == typeof(float) ||
                prop.PropertyType == typeof(double) ||
                prop.PropertyType == typeof(decimal)))
            {
                lst.Add(parentName);
            }
            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTimeOffset) || prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(DateTimeOffset?))
            {
                lst.Add(parentName);
            }
            else
            {
                foreach (var p in prop.PropertyType.GetProperties())
                {
                    ParseNames(p, ref lst, parentName);
                }
            }
        }
    }

}
