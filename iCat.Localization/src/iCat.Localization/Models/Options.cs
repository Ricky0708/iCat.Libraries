using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Models
{
    public class Options
    {
        /// <summary>
        /// when false, key not found exception return key, else throw new KeyNotFoundException
        /// default is false
        /// </summary>
        public bool EnableKeyNotFoundException { get; set; } = false;
    }
}
