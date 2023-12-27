using iCat.Token.Constants;
using iCat.Token.Interfaces;
using iCat.Token.JWT.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.JWT
{
    public class TokenGenerator : ITokenGenerator
    {
        public string Category => "JWT";
        private GenerateOption _options;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="secret"></param>
        /// <exception cref="ArgumentException"></exception>
        public TokenGenerator(string secret)
        {
            _options = new GenerateOption()
            {
                Secret = secret
            };
            var errors = Utilities.CheckJWTOption(_options);
            if (!String.IsNullOrEmpty(errors)) throw new ArgumentException(errors);
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentException"></exception>
        public TokenGenerator(GenerateOption options)
        {
            var errors = Utilities.CheckJWTOption(options);
            if (!String.IsNullOrEmpty(errors)) throw new ArgumentException(errors);
            _options = options;
        }

        /// <summary>
        /// <see cref="ITokenGenerator.GenerateToken(List{Claim})"/>
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public string GenerateToken(List<Claim> claims)
        {
            //assign default options
            _options ??= new GenerateOption();
            _options.Subject = claims;

            var tokenDescriptor = MakeTokenDescriptor(_options);

            var tokenHandler = new JwtSecurityTokenHandler();

            var stoken = tokenHandler.CreateToken(tokenDescriptor);

            var tokenWzScheme = tokenHandler.WriteToken(stoken);

            return tokenWzScheme;
        }

        private SecurityTokenDescriptor MakeTokenDescriptor(GenerateOption options)
        {
            var result = _func(options);
            return result;
        }

        /// <summary>
        /// Execute 
        /// </summary>
        private readonly Func<GenerateOption, SecurityTokenDescriptor> _func = (funOption) =>
        {
            //convert signture to base64
            var symmetricKey = Encoding.UTF8.GetBytes(funOption?.Secret ?? "");

            //Generate ClaimsIdentity
            var claims = new ClaimsIdentity();
            if (funOption!.Subject != null)
            {
                claims.AddClaims(funOption.Subject);
            }
            if (funOption!.AlwaysResetIssuedDate || !funOption.IssuedAt.HasValue)
            {
                funOption.IssuedAt = DateTime.UtcNow;
            }
            //make SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = funOption.IssuedAt,
                NotBefore = funOption.NotBefore,
                Issuer = funOption.Issuer,
                Audience = funOption.Audience,
                Subject = claims,
                Expires = funOption.IssuedAt.Value.AddSeconds(Convert.ToInt32(funOption.ExpireSeconds)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenDescriptor;
        };


    }
}
