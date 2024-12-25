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
    public class Privilege<T>
    {
        /// <summary>
        /// Privilege name
        /// </summary>
        public string Name { get; internal protected set; } = default!;

        /// <summary>
        /// Privilege value
        /// </summary>
        public T Value { get; internal protected set; } = default!;

        /// <summary>
        /// permission detail
        /// </summary>
        public List<Permission> PermissionsData { get; internal protected set; } = new List<Permission>();

        /// <summary>
        /// Permissions
        /// </summary>
        public int Permissions => (int)PermissionsData?.Sum(p => p.Value)!;
    }
}
