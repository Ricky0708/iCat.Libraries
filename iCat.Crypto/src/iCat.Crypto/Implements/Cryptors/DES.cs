using iCat.Crypto.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto.Implements.Cryptors
{
    /// <inheritdoc/>
    public class DES : ICryptor
    {
        private readonly string _key;

        /// <inheritdoc/>
        public string Category { get; init; } = "DES";

        public DES(string key)
        {
            _key = key;
        }

        /// <inheritdoc/>
        public DES(string category, string key)
        {
            _key = key;
            Category = category;
        }

        /// <inheritdoc/>
        public string Decrypt(string ciphertext)
        {
            return iCat.Crypto.DES.Decrypt(_key, ciphertext);
        }

        /// <inheritdoc/>
        public string Encrypt(string plaintext)
        {
            return iCat.Crypto.DES.Encrypt(_key, plaintext);
        }
    }
}
