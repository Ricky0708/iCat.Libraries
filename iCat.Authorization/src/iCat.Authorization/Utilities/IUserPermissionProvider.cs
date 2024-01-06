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
    public interface IUserPermissionProvider
    {
        /// <summary>
        /// Get current user permissions
        /// </summary>
        /// <returns></returns>
        IEnumerable<FunctionData> GetUserPermission();

        /// <summary>
        /// Validate FunctionData
        /// </summary>
        /// <param name="ownPermissions"></param>
        /// <param name="permissionRequired"></param>
        /// <returns></returns>
        bool Validate(IEnumerable<FunctionData> ownPermissions, FunctionData permissionRequired);
    }
}
