using iCat.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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
        /// Get AuthorizationPermissin attribute information
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        List<FunctionPermissionData> GetAuthorizationPermissionsData(params CustomAttributeData[] attributes);

        /// <summary>
        /// Get function and permission mapping
        /// </summary>
        /// <returns></returns>
        List<FunctionPermissionData>? GetFunctionPermissionDefinitions();

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

        /// <summary>
        /// Get claim from function permission data
        /// </summary>
        /// <param name="functionPermissionData"></param>
        /// <returns></returns>
        Claim GetClaimFromFunctionPermissionData(FunctionPermissionData functionPermissionData);
    }
}
