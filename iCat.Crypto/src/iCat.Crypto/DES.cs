using iCat.Crypto.Models;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto
{
    public static class DES
    {
        public static string Encrypt(string key, string plaintext)
        {
            if (plaintext.Length > 92160)
                return "Error. Data String too large. Keep within 90Kb.";
            var keyIv = new DesKeyIV(key);
            // var engine = new DesEngine();
            // new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));
            var cipher = CipherUtilities.GetCipher("DES/CBC/PKCS5Padding");
            cipher.Init(true, new ParametersWithIV(new KeyParameter(keyIv.Key), keyIv.IV));
            var rbData = Encoding.Unicode.GetBytes(plaintext);
            return Convert.ToBase64String(cipher.DoFinal(rbData));
        }

        public static string Decrypt(string key, string ciphertext)
        {

            try
            {
                if (string.IsNullOrEmpty(ciphertext)) return "ERROR: EncString is NULL!";
                var keyIv = new DesKeyIV(key);
                var cipher = CipherUtilities.GetCipher("DES/CBC/PKCS5Padding");
                cipher.Init(false, new ParametersWithIV(new KeyParameter(keyIv.Key), keyIv.IV));
                var encData = Convert.FromBase64String(ciphertext);
                return Encoding.Unicode.GetString(cipher.DoFinal(encData));
            }
            catch (Exception)
            {

            }
            return "";


        }
    }
}
