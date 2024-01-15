using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Models
{
    /// <summary>
    /// Validation result
    /// </summary>
    public class ValidationResult
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
        /// Valid success claim principal
        /// </summary>
        public ClaimsPrincipal? Principal { get; set; }
    }
}
