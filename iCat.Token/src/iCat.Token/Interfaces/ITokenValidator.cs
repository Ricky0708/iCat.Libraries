using iCat.Token.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Interfaces
{
    /// <summary>
    /// Token validator
    /// </summary>
    public interface ITokenValidator
    {
        /// <summary>
        /// Category
        /// </summary>
        string Category { get; }

        /// <summary>
        /// 驗證 token 有效並取得授權資訊
        /// 自行客制化需記得在ClaimsPrincipal中設定 AuthenticationType，設定來源在
        /// 加入參考System.IdentityModel.dll(已存在於.net framework組件中
        /// System.Security.Claims.AuthenticationType.xxx
        /// 基本上可以隨便設，或者參考以下網站說明
        /// https://msdn.microsoft.com/zh-tw/library/system.security.claims.authenticationtypes(v=vs.110).aspx
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ValidationResult Validate(string token);
    }
}
