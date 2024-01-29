using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Models
{
    /// <summary>
    /// Permission detail
    /// </summary>
    public interface IPermission
    {
        /// <summary>
        /// Permission
        /// </summary>
        int Value { get; }
    }

    /// <summary>
    /// Permission detail
    /// </summary>
    public class Permission : IPermission
    {
        /// <summary>
        /// Permission name
        /// </summary>
        public string? Name { get; internal set; }

        /// <summary>
        /// Permission
        /// </summary>
        public int Value { get; internal set; }
    }
}
