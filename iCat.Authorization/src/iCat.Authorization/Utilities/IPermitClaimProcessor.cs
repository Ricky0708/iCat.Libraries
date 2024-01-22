using iCat.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    /// <summary>
    /// Claim Generator
    /// </summary>
    public interface IPermitClaimProcessor
    {
        /// <summary>
        /// Generate permit claim
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Claim GeneratePermitClaim<T>(IPermit<T> permission) where T : IPermission;

        /// <summary>
        /// Generate permit claim
        /// </summary>
        /// <typeparam name="TPermission"></typeparam>
        /// <param name="permission"></param>
        /// <returns></returns>
        Claim GeneratePermitClaim<TPermission>(TPermission permission) where TPermission : Enum;

        /// <summary>
        /// Generate permit claim
        /// </summary>
        /// <param name="permit"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        Claim GeneratePermitClaim(int permit, int permission);

        /// <summary>
        /// Get currently authenticated user permit
        /// </summary>
        /// <returns></returns>
        IEnumerable<Permit> GetPermit();
    }
}
