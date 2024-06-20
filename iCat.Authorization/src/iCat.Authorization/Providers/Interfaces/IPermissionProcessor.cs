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
    public interface IPermissionProcessor
    {
        /// <summary>
        /// Get permit and permission mapping
        /// </summary>
        /// <returns></returns>
        List<Permit> GetDefinitions();

        /// <summary>
        /// Get permit definition from permission
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permission"></param>
        /// <returns></returns>
        Permit GetPermitDefinitionFromPermission<T>(T permission) where T : Enum;

        /// <summary>
        /// Get permit definition from permission
        /// </summary>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        Permit GetPermitDefinitionFromPermission(Type permissionType);

        /// <summary>
        /// Get permissions information on attribute
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        List<Permit> GetPermitFromAttribute(params CustomAttributeData[] attributes);

        /// <summary>
        /// Build permit from permissions
        /// </summary>
        /// <param name="permitValue"></param>
        /// <param name="permissionsValue"></param>
        /// <returns></returns>
        Permit BuildPermit(int permitValue, int permissionsValue);

        /// <summary>
        /// Build permit from permissions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permission"></param>
        /// <returns></returns>
        Permit BuildPermit<T>(T permission) where T : Enum;

        /// <summary>
        /// Validate Permit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permissions"></param>
        /// <param name="permissionRequired"></param>
        /// <returns></returns>
        bool ValidatePermission<T>(IEnumerable<IPermit<T>> permissions, IPermit<T> permissionRequired) where T : IPermission;
    }
}
