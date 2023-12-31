﻿using iCat.Crypto.Interfaces;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto.Implements.Cryptors
{
    /// <inheritdoc/>
    public class RSA : ICryptor
    {
        private readonly string _publicKey;
        private readonly string _privateKey;

        /// <inheritdoc/>
        public string Category { get; init; } = "RSA";

        /// <inheritdoc/>
        public RSA(string publicKeyPem, string privateKeyPem)
        {
            _publicKey = publicKeyPem ?? throw new ArgumentNullException(nameof(publicKeyPem));
            _privateKey = privateKeyPem ?? throw new ArgumentNullException(nameof(privateKeyPem));
        }

        /// <inheritdoc/>
        public RSA(string category, string publicKey, string privateKey)
        {
            Category = category;
            _publicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
            _privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
        }

        /// <inheritdoc/>
        public string Decrypt(string ciphertext)
        {
            return iCat.Crypto.RSA.Decrypt(_privateKey, ciphertext);
        }

        /// <inheritdoc/>
        public string Encrypt(string plaintext)
        {
            return iCat.Crypto.RSA.Encrypt(_publicKey, plaintext);
        }
    }
}
