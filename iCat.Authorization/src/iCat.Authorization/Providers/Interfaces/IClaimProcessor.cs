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
    public interface IClaimProcessor
    {
        /// <summary>
        /// Generate privilege claim
        /// </summary>
        /// <param name="privilege"></param>
        /// <returns></returns>
        Claim GeneratePrivilegeClaim<T>(IPrivilege<T> privilege) where T : IPermission;

        /// <summary>
        /// Generate privilege claim
        /// </summary>
        /// <typeparam name="TPermission"></typeparam>
        /// <param name="permission"></param>
        /// <returns></returns>
        Claim GeneratePrivilegeClaim<TPermission>(TPermission permission) where TPermission : Enum;
    }
}
