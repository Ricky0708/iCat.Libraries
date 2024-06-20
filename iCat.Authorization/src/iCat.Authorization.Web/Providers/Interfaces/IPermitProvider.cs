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
    /// provider permit info
    /// </summary>
    public interface IPermitProvider
    {
        /// <summary>
        /// Get currently authenticated user permit
        /// </summary>
        /// <returns></returns>
        IEnumerable<Permit> GetCurrentUserPermits();

        /// <summary>
        /// Get currently authenticated user permit
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        IEnumerable<Permit> GetUserPermits(HttpContext httpContext);

        /// <summary>
        /// Get permits required from router
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        List<Permit> GetRouterPermitsRequired(Endpoint endpoint);

        /// <summary>
        /// Generate permit claim
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Claim GenerateClaim<T>(IPermit<T> permission) where T : IPermission;

        /// <summary>
        /// Generate permit claim
        /// </summary>
        /// <typeparam name="TPermission"></typeparam>
        /// <param name="permission"></param>
        /// <returns></returns>
        Claim GenerateClaim<TPermission>(TPermission permission) where TPermission : Enum;

        /// <summary>
        /// Generate permit claim
        /// </summary>
        /// <param name="permit"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        Claim GenerateClaim(int permit, int permission);

        /// <summary>
        /// Validate Permit
        /// </summary>
        /// <param name="userPermit"></param>
        /// <param name="routerPermit"></param>
        /// <returns></returns>
        bool ValidatePermission(IEnumerable<Permit> userPermit, Permit routerPermit);
    }
}
