using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;
using iCat.Crypto.Implements.Cryptors;
using Org.BouncyCastle.Crypto;

namespace iCat.Crypto.Tests
{
    [TestClass()]
    public class RSATests
    {
        string _privateKeyPem = "";
        string _publicKeyPem = "";

        string _privateKeyPem2 = "";
        string _publicKeyPem2 = "";

        [TestInitialize]
        public void Setup()
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024))
            {
                _privateKeyPem = RSA.ExportRSAPrivateKeyPem();
                _publicKeyPem = RSA.ExportRSAPublicKeyPem();
                RSA.PersistKeyInCsp = false;
            }

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024))
            {
                _privateKeyPem2 = RSA.ExportRSAPrivateKeyPem();
                _publicKeyPem2 = RSA.ExportRSAPublicKeyPem();
                RSA.PersistKeyInCsp = false;
            }
        }

        [TestMethod()]
        public void Crypt_Success_Test()
        {
            // arrange
            var plaintext = "12345678";

            // action
            var ciphertext = RSA.Encrypt(_publicKeyPem, plaintext);
            var result = RSA.Decrypt(_privateKeyPem, ciphertext);

            // assert
            Assert.AreEqual(plaintext, result);
        }

        [TestMethod()]
        public void Crypt_Fail_KeyError_Test()
        {
            // arrange
            var plaintext = "12345678";

            // action
            var ciphertext = RSA.Encrypt(_publicKeyPem, plaintext);
            var result = RSA.Decrypt(_privateKeyPem2, ciphertext);

            // assert
            Assert.AreEqual("", result);
        }

        [TestMethod()]
        public void Signature_Success_Test()
        {
            // arrange
            var plaintext = "12345678";

            // action
            var sign = RSA.Sign(_privateKeyPem, plaintext);
            var result = RSA.Verify(_publicKeyPem, sign, plaintext);

            // assert
            Assert.AreEqual(true, result);
        }

        [TestMethod()]
        public void Signature_Fail_Test()
        {
            // arrange
            var plaintext = "12345678";

            // action
            var sign = RSA.Sign(_privateKeyPem, plaintext);
            var result = RSA.Verify(_publicKeyPem2, sign, plaintext);

            // assert
            Assert.AreEqual(false, result);
        }
    }
}