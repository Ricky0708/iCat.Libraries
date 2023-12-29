using iCat.Crypto.Interfaces;
using iCat.Token.Constants;
using iCat.Token.Interfaces;
using iCat.Token.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Sequence
{
    public class TokenValidator<T> : ITokenValidator
    {
        private readonly ICryptor _cryptor;

        public string Category => "Sequence";

        public TokenValidator(ICryptor cryptor)
        {
            _cryptor = cryptor ?? throw new ArgumentNullException(nameof(cryptor));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ValidationResult Validate(string token)
        {
            var type = typeof(T);
            var tokenValue = _cryptor.Decrypt(token);
            var claims = new List<Claim>();
            var result = new ValidationResult
            {
                IsValid = false,
                ErrorMsg = "Decrypt token fail"
            };

            if (!string.IsNullOrEmpty(tokenValue))
            {
                var array = tokenValue.Split('|');
                var position = 0;
                foreach (var prop in type.GetProperties())
                {
                    if (prop.PropertyType.IsArray ||
                    prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                    prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        while (position < array.Length && array[position] != "#")
                        {
                            claims.Add(new Claim(prop.Name, array[position++]));
                        }
                        position++;
                    }
                    else
                    {
                        claims.Add(new Claim(prop.Name, array[position++]));

                    }
                }

                var claimIdentity = new ClaimsIdentity(claims);
                var principal = new ClaimsPrincipal(claimIdentity);

                result.Principal = principal;
                result.ErrorMsg = "";
                result.IsValid = true;
            }
            return result;
        }
    }
}
