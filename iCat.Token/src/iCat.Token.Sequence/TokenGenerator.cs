using iCat.Crypto.Interfaces;
using iCat.Token.Constants;
using iCat.Token.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Sequence
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly ICryptor _cryptor;

        public string Category => "Sequence";

        public TokenGenerator(ICryptor cryptor)
        {
            _cryptor = cryptor ?? throw new ArgumentNullException(nameof(cryptor));
        }

        public string GenerateToken(List<Claim> claims)
        {
            var sb = new StringBuilder();
            var preClaim = default(Claim);
            var isContinue = false;
            foreach (var claim in claims)
            {
                if (preClaim?.Type == claim.Type)
                {
                    isContinue = true;
                }
                if (isContinue && preClaim?.Type != claim.Type)
                {
                    sb.Append($"#|");
                    isContinue = false;
                }
                sb.Append($"{claim.Value}|");
                preClaim = claim;
            }
            sb.Remove(sb.Length - 1, 1);
            return _cryptor.Encrypt(sb.ToString());
        }
    }
}
