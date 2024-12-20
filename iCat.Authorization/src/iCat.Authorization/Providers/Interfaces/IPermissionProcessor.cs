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
        /// Get privilege and permission mapping
        /// </summary>
        /// <returns></returns>
        List<Privilege> GetDefinitions();

        /// <summary>
        /// Get privilege definition from permission
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permission"></param>
        /// <returns></returns>
        Privilege GetPrivilegeDefinitionFromPermission<T>(T permission) where T : Enum;

        /// <summary>
        /// Get privilege definition from permission
        /// </summary>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        Privilege GetPrivilegeDefinitionFromPermission(Type permissionType);

        /// <summary>
        /// Get permissions information on attribute
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        List<Privilege> GetPrivilegeFromAttribute(params CustomAttributeData[] attributes);

        /// <summary>
        /// Build privilege from permissions
        /// </summary>
        /// <param name="privilegeValue"></param>
        /// <param name="permissionsValue"></param>
        /// <returns></returns>
        Privilege BuildPrivilege(int privilegeValue, int permissionsValue);

        /// <summary>
        /// Build privilege from permissions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permission"></param>
        /// <returns></returns>
        Privilege BuildPrivilege<T>(T permission) where T : Enum;

        /// <summary>
        /// Validate privilege
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permissions"></param>
        /// <param name="permissionRequired"></param>
        /// <returns></returns>
        bool ValidatePermission<T>(IEnumerable<IPrivilege<T>> permissions, IPrivilege<T> permissionRequired) where T : IPermission;
    }
}
