using iCat.Token.Constants;
using iCat.Token.Interfaces;
using iCat.Token.JWT.Models;
using iCat.Token.Models;
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
    public class TokenValidator : ITokenValidator
    {
        public string Category => "JWT";
        private readonly ValidateOption _parameters;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="secret"></param>
        /// <exception cref="ArgumentException"></exception>
        public TokenValidator(string secret)
        {
            _parameters = new ValidateOption()
            {
                IssuerSigningKey = secret
            };
            var errors = Utilities.CheckJWTOption(_parameters);
            if (!String.IsNullOrEmpty(errors)) throw new ArgumentException(errors);
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="parameters"></param>
        /// <exception cref="ArgumentException"></exception>
        public TokenValidator(ValidateOption parameters)
        {
            var errors = Utilities.CheckJWTOption(parameters);
            if (!String.IsNullOrEmpty(errors)) throw new ArgumentException(errors);
            _parameters = parameters;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ValidationResult Validate(string token)
        {

            var paras = new TokenValidationParameters()
            {
                ClockSkew = _parameters.ClockSkew,
                RequireExpirationTime = _parameters.RequireExpirationTime,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_parameters.IssuerSigningKey!)),
                ValidateAudience = _parameters.ValidateAudience,
                ValidateIssuer = _parameters.ValidateIssuer,
                ValidateLifetime = _parameters.ValidateLifetime,
                ValidIssuer = _parameters.ValidIssuer,
                ValidAudience = _parameters.ValidAudience,
                //ValidateIssuerSigningKey = _parameters.ValidateIssuerSigningKey,
            };
            var result = Func(token, paras);
            return result;

        }

        /// <summary>
        /// Execute validation
        /// </summary>
        /// <param name="funcToken"></param>
        /// <param name="funcParameters"></param>
        /// <returns></returns>
        private static ValidationResult Func(string funcToken, TokenValidationParameters funcParameters)
        {
            bool isValidate;
            ClaimsPrincipal? principal;
            string? errorMsg;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                isValidate = true;
                errorMsg = "";
                principal = tokenHandler.ValidateToken(funcToken, funcParameters, out SecurityToken? securityToken);
            }
            catch (SecurityTokenInvalidIssuerException ex)
            {
                isValidate = false;
                errorMsg = ex.Message;
                principal = null;
            }
            catch (SecurityTokenInvalidAudienceException ex)
            {
                isValidate = false;
                errorMsg = ex.Message;
                principal = null;
            }
            catch (SecurityTokenExpiredException ex)
            {
                isValidate = false;
                errorMsg = ex.Message;
                principal = null;
            }
            catch (Exception ex)
            {
                isValidate = false;
                errorMsg = ex.Message;
                principal = null;
            }
            return new ValidationResult
            {
                IsValid = isValidate,
                ErrorMsg = errorMsg,
                Principal = principal
            };
        }
    }
}
