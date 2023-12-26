using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Models.JWT
{
    public class ValidateOption
    {
        /// <summary>
        /// 允許時間位移
        /// </summary>
        public TimeSpan ClockSkew { get; set; } = new TimeSpan(0, 0, 0);

        /// <summary>
        /// 是否要求驗證有效時間
        /// </summary>
        public bool RequireExpirationTime { get; set; } = true;

        /// <summary>
        /// 驗證用的 key
        /// </summary>
        public string? IssuerSigningKey { get; set; }

        /// <summary>
        /// 是否驗證發行對像 (client target)
        /// </summary>
        public bool ValidateAudience { get; set; } = false;

        /// <summary>
        /// 是否驗證發行者 (server)
        /// </summary>
        public bool ValidateIssuer { get; set; } = false;

        ///// <summary>
        ///// 是否驗證 key
        ///// </summary>
        //public bool ValidateIssuerSigningKey { get; set; } = true;

        /// <summary>
        ///  是否驗證有效期間
        /// </summary>
        public bool ValidateLifetime { get; set; } = true;

        /// <summary>
        /// 發行者
        /// </summary>
        public string? ValidIssuer { get; set; }

        /// <summary>
        /// 發行對像
        /// </summary>
        public string? ValidAudience { get; set; }
    }
}
