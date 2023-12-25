using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;

namespace iCat.Crypto.Tests
{
    [TestClass()]
    public class AESTests
    {
        [TestMethod()]
        public void Crypt_Success_Test()
        {
            // arrange
            var key = "12345678";
            var plaintext = "12345678";

            // action
            var ciphertext = AES.Encrypt(key, plaintext);
            var result = AES.Decrypt("12345678", ciphertext);

            // assert
            Assert.AreEqual(plaintext, result);
        }

        [TestMethod()]
        public void Crypt_Fail_KeyError_Test()
        {
            // arrange
            var key = "12345678";
            var plaintext = "12345678";

            // action
            var ciphertext = AES.Encrypt(key, plaintext);
            var result = AES.Decrypt("123456789", ciphertext);

            // assert
            Assert.AreEqual("", result);
        }
    }
}