using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.JWT.Models
{
    /// <summary>
    /// Option
    /// </summary>
    public class ValidateOption
    {
        /// <summary>
        /// ClockSkew
        /// </summary>
        public TimeSpan ClockSkew { get; set; } = new TimeSpan(0, 0, 0);

        /// <summary>
        /// RequireExpirationTime
        /// </summary>
        public bool RequireExpirationTime { get; set; } = true;

        /// <summary>
        /// IssuerSigningKey
        /// </summary>
        public string? IssuerSigningKey { get; set; }

        /// <summary>
        /// ValidateAudience (client target)
        /// </summary>
        public bool ValidateAudience { get; set; } = false;

        /// <summary>
        /// ValidateIssuer (server)
        /// </summary>
        public bool ValidateIssuer { get; set; } = false;

        ///// <summary>
        ///// ValidateIssuerSigningKey key
        ///// </summary>
        //public bool ValidateIssuerSigningKey { get; set; } = true;

        /// <summary>
        ///  ValidateLifetime
        /// </summary>
        public bool ValidateLifetime { get; set; } = true;

        /// <summary>
        /// ValidIssuer
        /// </summary>
        public string? ValidIssuer { get; set; }

        /// <summary>
        /// ValidAudience
        /// </summary>
        public string? ValidAudience { get; set; }
    }
}
