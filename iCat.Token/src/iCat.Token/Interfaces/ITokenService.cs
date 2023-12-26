using iCat.Token.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Interfaces
{
    public interface ITokenService<T>
    {
        string GenerateToken(List<Claim> claims);
        string GenerateToken(T dataModel);
        ValidationResult ValidateToken(string token);
        ValidationDataResult<T> ValidateWithReturnData(string token);
    }
}
