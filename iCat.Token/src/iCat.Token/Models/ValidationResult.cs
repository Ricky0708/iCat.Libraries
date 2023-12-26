using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMsg { get; set; }
        public ClaimsPrincipal? Principal { get; set; }
    }
}
