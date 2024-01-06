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
    public class PermissionDetail
    {
        /// <summary>
        /// Permission name
        /// </summary>
        public string? PermissionName { get; set; }

        /// <summary>
        /// Permission
        /// </summary>
        public int Permission { get; set; }
    }
}
