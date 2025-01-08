using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Models
{
    /// <summary>
    /// Permission detail
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Permission name
        /// </summary>
        public string Name { get; internal protected set; } = default!;

        /// <summary>
        /// Permission
        /// </summary>
        public int Value { get; internal protected set; }
    }
}
