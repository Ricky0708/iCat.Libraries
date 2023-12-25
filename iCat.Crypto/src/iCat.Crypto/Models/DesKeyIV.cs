using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto.Models
{
    internal class DesKeyIV
    {
        public Byte[] Key = new Byte[8];
        public Byte[] IV = new Byte[8];
        public DesKeyIV(string strKey)
        {
            var sha = new Sha1Digest();
            var hash = new byte[sha.GetDigestSize()];
            var data = Encoding.ASCII.GetBytes(strKey);
            sha.BlockUpdate(data, 0, data.Length);
            sha.DoFinal(hash, 0);
            for (int i = 0; i < 8; i++) Key[i] = hash[i];
            for (int i = 8; i < 16; i++) IV[i - 8] = hash[i];
        }
    }
}
