using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Crypto.Interfaces
{
    /// <summary>
    /// Hash
    /// </summary>
    public interface IHasher
    {
        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; init; }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        string MD5(string message);

        /// <summary>
        /// SHA 1
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        string SHA1(string message);

        /// <summary>
        /// SHA256
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        string SHA256(string message);

        /// <summary>
        /// SHA384
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        string SHA384(string message);

        /// <summary>
        /// SHA512
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        string SHA512(string message);

    }
}
