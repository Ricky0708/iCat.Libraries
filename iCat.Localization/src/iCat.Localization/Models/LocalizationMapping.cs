using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Models
{
    /// <summary>
    /// Language Mapping Data
    /// </summary>
    public class LocalizationMapping
    {
        /// <summary>
        /// Language Code
        /// </summary>
        public string? CultureName { get; set; }

        /// <summary>
        /// Code Data
        /// </summary>
        public Dictionary<string, string>? LanguageData { get; set; }
    }
}
