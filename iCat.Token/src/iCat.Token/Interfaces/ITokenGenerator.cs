using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Interfaces
{
    /// <summary>
    /// Token Generator
    /// </summary>
    public interface ITokenGenerator
    {
        /// <summary>
        /// Category
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Create token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        string GenerateToken(List<Claim> claims);
    }
}
