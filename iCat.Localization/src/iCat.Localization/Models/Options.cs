using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Models
{
    /// <summary>
    /// Parser options
    /// </summary>
    public class Options
    {
        /// <summary>
        /// When false, key not found exception return key, else throw new KeyNotFoundException
        /// default is false
        /// </summary>
        public bool EnableKeyNotFoundException { get; set; } = false;

        /// <summary>
        /// Prefix for parser keywords
        /// </summary>
        public char KeywordPrefix { get; set; } = '{';

        /// <summary>
        /// suffix for parser keywords
        /// </summary>
        public char KeywordSuffix { get; set; } = '}';
    }
}
