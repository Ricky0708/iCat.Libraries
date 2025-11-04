using iCat.Token.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Interfaces
{
    /// <summary>
    /// Integrate TokenGenerator and TokenValidator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITokenService<T>
    {
        /// <summary>
        /// Token category
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Generate token by claims
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        string GenerateToken(List<Claim> claims);

        /// <summary>
        /// Generate token by Model
        /// </summary>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        string GenerateToken(T dataModel);

        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ValidationResult ValidateToken(string token);

        /// <summary>
        /// Validate token and return model
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ValidationDataResult<T> ValidateWithReturnData(string token);
    }
}
