using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Models
{
    public class ValidationDataResult<T>
    {
        public bool IsValid { get; set; }
        public string? ErrorMsg { get; set; }
        public T? TokenData { get; set; }
    }
}
