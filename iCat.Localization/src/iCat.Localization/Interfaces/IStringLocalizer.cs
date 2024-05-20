using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Interfaces
{
    /// <summary>
    /// Localizer
    /// </summary>
    public interface IStringLocalizer : Microsoft.Extensions.Localization.IStringLocalizer
    {
        /// <summary>
        /// Language Dictionary
        /// </summary>
        Dictionary<string, Dictionary<string, string>> Languages { get; }

        /// <summary>
        /// Add dynamic parameters
        /// </summary>
        /// <param name="str"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        string AddParams(string str, object paramData);

        /// <summary>
        /// Localize string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        string Localize(string str, string lang);

        /// <summary>
        /// Set language mapping
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="langCode"></param>
        ///
        void SetLanguageCollection(Dictionary<string, string> dic, string langCode);
    }
}
