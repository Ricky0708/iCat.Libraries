using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Models
{
    /// <summary>
    /// Permit - Permission information
    /// </summary>
    public class Permit
    {
        /// <summary>
        /// Permit name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Permit value
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// permission detail
        /// </summary>
        public List<Permission> PermissionsData { get; set; } = new List<Permission>();

        /// <summary>
        /// Permissions
        /// </summary>
        public int Permissions => PermissionsData?.Sum(p => p.Value) ?? 0;
    }
}
