using iCat.Authorization.Models;
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
        IEnumerable<Permit> GetPermits();

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

    }
}
