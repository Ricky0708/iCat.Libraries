using iCat.Localization.Implements;
using iCat.Localization.Interfaces;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Extensions
{
    public static class iCatLocalizationStringExtension
    {

        private static Interfaces.LocalizerFactory? _factory;

        public static void SetLocalizationFactory(Interfaces.LocalizerFactory stringLocalizerFactory)
        {
            _factory = stringLocalizerFactory ?? throw new ArgumentNullException(nameof(stringLocalizerFactory));
        }

        /// <summary>
        /// Add daynamic parameter
        /// </summary>
        /// <param name="str"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static string AddParams(this string str, object paramData)
        {
            return _factory?.Create().AddParams(str, paramData) ?? throw new NotImplementedException("");
        }

        /// <summary>
        /// Localize string by CurrentCulture
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Localize(this string str)
        {
            return _factory?.Create().Localize(str, CultureInfo.CurrentCulture.Name) ?? throw new NotImplementedException("");
        }

        /// <summary>
        /// Localize string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static string Localize(this string str, string lang)
        {
            return _factory?.Create().Localize(str, lang) ?? throw new NotImplementedException("");
        }
    }
}
