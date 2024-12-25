using iCat.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Providers.Interfaces
{
    /// <summary>
    /// Claim Generator
    /// </summary>
    public interface IClaimProcessor<T> where T : Enum
    {
        /// <summary>
        /// Generate privilege claim
        /// </summary>
        /// <param name="privilege"></param>
        /// <returns></returns>
        Claim GeneratePrivilegeClaim(Privilege<T> privilege);

        /// <summary>
        /// Generate privilege claim
        /// </summary>
        /// <typeparam name="TPermission"></typeparam>
        /// <param name="permission"></param>
        /// <returns></returns>
        Claim GeneratePrivilegeClaim<TPermission>(TPermission permission) where TPermission : Enum;
    }
}
