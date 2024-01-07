using iCat.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    /// <summary>
    /// Provide user's permission
    /// </summary>
    public interface IFunctionPermissionProvider
    {
        /// <summary>
        /// Get current user permissions
        /// </summary>
        /// <returns></returns>
        IEnumerable<FunctionPermissionData> GetUserPermission();

        /// <summary>
        /// Validate FunctionData
        /// </summary>
        /// <param name="ownPermissions"></param>
        /// <param name="permissionRequired"></param>
        /// <returns></returns>
        bool Validate(IEnumerable<FunctionPermissionData> ownPermissions, FunctionPermissionData permissionRequired);
    }
}
