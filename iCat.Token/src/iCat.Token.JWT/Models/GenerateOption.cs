using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.JWT.Models
{
    public class GenerateOption
    {
        /// <summary>
        /// signature code
        /// </summary>
        public string? Secret { get; set; }

        /// <summary>
        /// expire second, default 1200
        /// </summary>
        public int ExpireSeconds { get; set; } = 1200;

        /// <summary>
        /// Claims
        /// </summary>
        internal IEnumerable<Claim>? Subject { get; set; }

        /// <summary>
        /// 發行對像
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// 發行者
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// 發行時間
        /// </summary>
        public DateTime? IssuedAt { get; set; }

        /// <summary>
        /// 在此時間前無效
        /// </summary>
        public DateTime? NotBefore { get; set; }

        /// <summary>
        /// 每次發佈更新發行日期
        /// </summary>
        public bool AlwaysResetIssuedDate { get; set; }
    }
}
