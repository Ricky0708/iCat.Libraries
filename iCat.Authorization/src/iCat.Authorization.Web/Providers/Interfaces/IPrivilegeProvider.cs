using iCat.Authorization.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Web.Providers.Interfaces
{
    /// <summary>
    /// provider Privilege info
    /// </summary>
    public interface IPrivilegeProvider<T> where T : Enum
    {
        /// <summary>
        /// Get currently authenticated user privilege
        /// </summary>
        /// <returns></returns>
        IEnumerable<Privilege<T>> GetCurrentUserPrivileges();

        /// <summary>
        /// Get currently authenticated user privilege
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        IEnumerable<Privilege<T>> GetUserPrivileges(HttpContext httpContext);

        /// <summary>
        /// Get privileges required from router
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        List<Privilege<T>> GetRouterPrivilegesRequired(Endpoint endpoint);

        /// <summary>
        /// Generate privilege claim
        /// </summary>
        /// <param name="privilege"></param>
        /// <returns></returns>
        Claim GenerateClaim(Privilege<T> privilege);

        /// <summary>
        /// Generate privilege claim
        /// </summary>
        /// <typeparam name="TPermission"></typeparam>
        /// <param name="permissionEnum"></param>
        /// <returns></returns>
        Claim GenerateClaim<TPermission>(TPermission permissionEnum) where TPermission : Enum;

        /// <summary>
        /// Validate privilege
        /// </summary>
        /// <param name="userPrivilege"></param>
        /// <param name="routerPrivilege"></param>
        /// <returns></returns>
        bool ValidatePermission(IEnumerable<Privilege<T>> userPrivilege, Privilege<T> routerPrivilege);
    }
}
