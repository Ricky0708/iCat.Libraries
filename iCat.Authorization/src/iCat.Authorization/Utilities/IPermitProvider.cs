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
        /// Get Permit
        /// </summary>
        /// <returns></returns>
        IEnumerable<Permit> GetPermit();

        /// <summary>
        /// Generate permit claim
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Claim GeneratePermitClaim(Permit permission);

        /// <summary>
        /// Validate FunctionData
        /// </summary>
        /// <param name="permits"></param>
        /// <param name="permissionRequired"></param>
        /// <returns></returns>
        bool Validate(IEnumerable<Permit> permits, Permit permissionRequired);
    }
}
