using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto.Models
{
    internal class AesKeyIV
    {
        public Byte[] Key = new Byte[16];
        public Byte[] IV = new Byte[16];
        public AesKeyIV(string strKey)
        {
            var sha = new Sha256Digest();
            var hash = new byte[sha.GetDigestSize()];
            var data = Encoding.ASCII.GetBytes(strKey);
            sha.BlockUpdate(data, 0, data.Length);
            sha.DoFinal(hash, 0);
            Array.Copy(hash, 0, Key, 0, 16);
            Array.Copy(hash, 16, IV, 0, 16);
        }
    }
}
