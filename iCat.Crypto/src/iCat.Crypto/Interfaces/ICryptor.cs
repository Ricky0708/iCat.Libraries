using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto.Interfaces
{
    public interface ICryptor
    {
        string Category { get; init; }
        string Encrypt(string rawText);
        string Decrypt(string encText);
    }
}
