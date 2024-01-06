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
    public class FunctionPermissionData
    {
        /// <summary>
        /// Function name
        /// </summary>
        public string? FunctionName { get; set; }

        /// <summary>
        /// Function value
        /// </summary>
        public int? FunctionValue { get; set; }

        /// <summary>
        /// permission detail
        /// </summary>
        public List<PermissionDetail> PermissionDetails { get; set; } = new List<PermissionDetail>();

        /// <summary>
        /// Permissions
        /// </summary>
        public int Permissions => PermissionDetails?.Sum(p => p.Permission) ?? 0;
    }
}
