using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Models
{
    /// <summary>
    /// Validation result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidationDataResult<T>
    {
        /// <summary>
        /// Is token validated
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Message for valid fail
        /// </summary>
        public string? ErrorMsg { get; set; }

        /// <summary>
        /// Valid success data
        /// </summary>
        public T? TokenData { get; set; }
    }
}
