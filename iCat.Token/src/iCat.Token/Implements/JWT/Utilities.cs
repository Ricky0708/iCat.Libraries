using iCat.Token.Models.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Implements.JWT
{
    internal static class Utilities
    {
        internal static string CheckJWTOption(ValidateOption option)
        {
            var result = "";
            result += IsSecretEmpty(option?.IssuerSigningKey ?? "");
            result += IsSecretGreatThen16(option?.IssuerSigningKey ?? "");
            return result;
        }
        internal static string CheckJWTOption(GenerateOption option)
        {
            var result = "";
            result += IsSecretEmpty(option?.Secret ?? "");
            result += IsSecretGreatThen16(option?.Secret ?? "");
            return result;
        }

        private static string IsSecretEmpty(string key)
        {
            if (key == null || String.IsNullOrEmpty(key)) return "options or secret can't be null";
            return "";
        }
        /// <summary>
        /// great then 128 bit(16 byte)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string IsSecretGreatThen16(string key)
        {
            if (key.Length < 16) return "Minimum secret lenth is 16";
            return "";
        }
    }
}
