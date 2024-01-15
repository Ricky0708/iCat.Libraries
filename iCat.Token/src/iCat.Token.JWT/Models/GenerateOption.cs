using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.JWT.Models
{
    /// <summary>
    /// Option
    /// </summary>
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
        /// Audience
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// Issuer
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// IssuedAt
        /// </summary>
        public DateTime? IssuedAt { get; set; }

        /// <summary>
        /// NotBefore
        /// </summary>
        public DateTime? NotBefore { get; set; }

        /// <summary>
        /// AlwaysResetIssuedDate
        /// </summary>
        public bool AlwaysResetIssuedDate { get; set; }
    }
}
