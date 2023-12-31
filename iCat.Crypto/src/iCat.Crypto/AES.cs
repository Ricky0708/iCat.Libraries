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
    /// <summary>
    /// AES Encrypto
    /// </summary>
    public static class AES
    {
        /// <summary>
        /// AES Encrypt
        /// </summary>
        /// <param name="key"></param>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string Encrypt(string key, string plaintext)
        {
            var keyIv = new AesKeyIV(key);
            // Default - AES/GCM/NoPadding、System.Security.AES - AES/CBC/PKCS7
            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
            cipher.Init(true, new ParametersWithIV(new KeyParameter(keyIv.Key), keyIv.IV));
            var rawData = Encoding.UTF8.GetBytes(plaintext);
            return Convert.ToBase64String(cipher.DoFinal(rawData));
        }

        /// <summary>
        /// AES Decrypt
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public static string Decrypt(string key, string ciphertext)
        {
            try
            {
                var keyIv = new AesKeyIV(key);
                var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
                cipher.Init(false, new ParametersWithIV(new KeyParameter(keyIv.Key), keyIv.IV));
                var encData = Convert.FromBase64String(ciphertext);
                return Encoding.UTF8.GetString(cipher.DoFinal(encData));
            }
            catch (Exception)
            {

            }
            return "";

        }
    }
}
