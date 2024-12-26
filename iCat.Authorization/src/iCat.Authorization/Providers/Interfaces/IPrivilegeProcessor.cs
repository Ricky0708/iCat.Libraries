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
    public interface IPrivilegeProcessor<T> where T : Enum
    {
        /// <summary>
        /// Get privilege and permission mapping
        /// </summary>
        /// <returns></returns>
        List<Privilege<T>> GetDefinitions();

        /// <summary>
        /// Get privilege definition from permission
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        Privilege<T> GetPrivilegeDefinitionFromPermission<E>(E permissionEnum) where E : Enum;

        /// <summary>
        /// Get privilege definition from permission
        /// </summary>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        Privilege<T> GetPrivilegeDefinitionFromPermission(Type permissionType);

        /// <summary>
        /// Get permissions information on attribute
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        List<Privilege<T>> GetPrivilegeFromAttribute(params CustomAttributeData[] attributes);

        /// <summary>
        /// Build privilege
        /// </summary>
        /// <param name="privilegeValue"></param>
        /// <param name="permissionsValue"></param>
        /// <returns></returns>
        Privilege<T> BuildPrivilege(int privilegeValue, int permissionsValue);

        /// <summary>
        /// Build privilege from permissions
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        Privilege<T> BuildPrivilege<E>(E permissionEnum) where E : Enum;

        /// <summary>
        /// Validate privilege
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permissions"></param>
        /// <param name="permissionRequired"></param>
        /// <returns></returns>
        bool Validate(IEnumerable<Privilege<T>> permissions, Privilege<T> permissionRequired);
    }
}
