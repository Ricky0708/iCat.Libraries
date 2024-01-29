using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Models
{
    /// <summary>
    /// Permit - Permission information
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPermit<T> where T : IPermission
    {
        /// <summary>
        /// Permit value
        /// </summary>
        int? Value { get; }

        /// <summary>
        /// permission detail
        /// </summary>
        List<T> PermissionsData { get; }

        /// <summary>
        /// Permissions
        /// </summary>
        int Permissions { get; }
    }

    /// <summary>
    /// Permit - Permission information
    /// </summary>
    public class Permit : IPermit<Permission>
    {
        /// <summary>
        /// Permit name
        /// </summary>
        public string? Name { get; internal set; }

        /// <summary>
        /// Permit value
        /// </summary>
        public int? Value { get; internal set; }

        /// <summary>
        /// permission detail
        /// </summary>
        public List<Permission> PermissionsData { get; internal set; } = new List<Permission>();

        /// <summary>
        /// Permissions
        /// </summary>
        public int Permissions => PermissionsData?.Sum(p => p.Value) ?? 0;
    }
}
