using iCat.Crypto.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto.Implements.Hashes
{
    /// <inheritdoc/>
    public class Hasher : IHasher
    {
        /// <inheritdoc/>
        public string Category { get; init; } = "Hash";

        private string _key;

        /// <inheritdoc/>
        public Hasher(string key)
        {
            _key = key;
        }

        /// <inheritdoc/>
        public Hasher(string category, string key)
        {
            _key = key;
            Category = category;
        }

        /// <inheritdoc/>
        public string MD5(string message)
        {
            return iCat.Crypto.Hash.MD5(_key, message);
        }

        /// <inheritdoc/>
        public string SHA1(string message)
        {
            return iCat.Crypto.Hash.SHA1(_key, message);
        }

        /// <inheritdoc/>
        public string SHA256(string message)
        {
            return iCat.Crypto.Hash.SHA256(_key, message);
        }

        /// <inheritdoc/>
        public string SHA384(string message)
        {
            return iCat.Crypto.Hash.SHA384(_key, message);
        }

        /// <inheritdoc/>
        public string SHA512(string message)
        {
            return iCat.Crypto.Hash.SHA512(_key, message);
        }
    }
}
