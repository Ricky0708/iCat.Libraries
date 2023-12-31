using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto
{
    /// <summary>
    /// RSA
    /// </summary>
    public static class RSA
    {
        /// <summary>
        /// RSA Encrypt
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string Encrypt(string publicKey, string plaintext)
        {
            using (var rsaPublic = new RSACryptoServiceProvider())
            {
                rsaPublic.ImportFromPem(publicKey.AsSpan());

                byte[] publicValue = rsaPublic.Encrypt(Encoding.UTF8.GetBytes(plaintext), false);
                string publicStr = Convert.ToBase64String(publicValue);//使用Base64將byte轉換為string
                return publicStr;
            }
        }

        /// <summary>
        /// RSA Decrypt
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string privateKey, string cipherText)
        {
            using (var rsaPrivate = new RSACryptoServiceProvider())
            {
                try
                {
                    rsaPrivate.ImportFromPem(privateKey.AsSpan());

                    byte[] privateValue = rsaPrivate.Decrypt(Convert.FromBase64String(cipherText), false);//使用Base64將string轉換為byte
                    string privateStr = Encoding.UTF8.GetString(privateValue);
                    return privateStr;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Private key to sign
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string Sign(string privateKey, string plaintext)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportFromPem(privateKey.AsSpan());

                var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                var signBytes = rsa.SignData(plaintextBytes, SHA256.Create());
                var sign = Convert.ToBase64String(signBytes);
                return sign;
            }
        }

        /// <summary>
        /// PublicKey to verify
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="sign"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Verify(string publicKey, string sign, string data)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportFromPem(publicKey.AsSpan());
                var signBytes = Convert.FromBase64String(sign);
                var dataBytes = Encoding.UTF8.GetBytes(data);
                bool isValid = rsa.VerifyData(dataBytes, SHA256.Create(), signBytes);
                return isValid;
            }
        }
    }
}
