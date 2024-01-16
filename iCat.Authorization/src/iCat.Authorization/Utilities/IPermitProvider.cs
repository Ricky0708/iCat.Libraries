using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using iCat.Authorization.Models;

namespace iCat.Authorization.Utilities
{
    /// <summary>
    /// Permit Provider
    /// </summary>
    public interface IPermitProvider
    {
        /// <summary>
        /// Get currently authenticated user permit
        /// </summary>
        /// <returns></returns>
        IEnumerable<Permit> GetPermit();

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
        /// Validate Permit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="permits"></param>
        /// <param name="permissionRequired"></param>
        /// <returns></returns>
        bool Validate<T>(IEnumerable<IPermit<T>> permits, IPermit<T> permissionRequired) where T : IPermission;
    }
}
