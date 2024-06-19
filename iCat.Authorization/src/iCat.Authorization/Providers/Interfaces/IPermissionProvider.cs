using iCat.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Providers.Interfaces
{
    /// <summary>
    /// Provide user's permission
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// Get AuthorizationPermissin attribute information
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        List<Permit> GetPermissionRequired(params CustomAttributeData[] attributes);

        /// <summary>
        /// Get permit definition from permission
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permission"></param>
        /// <returns></returns>
        Permit GetPermitFromPermission<T>(T permission) where T : Enum;

        /// <summary>
        /// Get permit and permission mapping
        /// </summary>
        /// <returns></returns>
        List<Permit> GetDefinitions();

        /// <summary>
        /// Validate Permit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permissions"></param>
        /// <param name="permissionRequired"></param>
        /// <returns></returns>
        bool Validate<T>(IEnumerable<IPermit<T>> permissions, IPermit<T> permissionRequired) where T : IPermission;
    }
}
