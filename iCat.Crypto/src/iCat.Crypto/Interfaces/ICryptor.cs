using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto.Interfaces
{
    /// <summary>
    /// Crytpor
    /// </summary>
    public interface ICryptor
    {
        /// <summary>
        /// Category
        /// </summary>
        string Category { get; init; }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        string Encrypt(string plainText);

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        string Decrypt(string cipherText);
    }
}
