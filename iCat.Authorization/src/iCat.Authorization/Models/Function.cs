using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Models
{
    /// <summary>
    /// Function - Permission information
    /// </summary>
    public class Function
    {
        /// <summary>
        /// Function name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Function value
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
