using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Models
{
    /// <summary>
    /// Privilege - Permission information
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPrivilege<T> where T : IPermission
    {
        /// <summary>
        /// Privilege value
        /// </summary>
        int Value { get; }

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
    /// Privilege - Permission information
    /// </summary>
    public class Privilege : IPrivilege<Permission>
    {
        /// <summary>
        /// Privilege name
        /// </summary>
        public string Name { get; internal set; } = default!;   

        /// <summary>
        /// Privilege value
        /// </summary>
        public int Value { get; internal set; }

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
