using iCat.Authorization.Providers.Implements;
using iCat.Authorization.Models;
using iCat.Authorization.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace iCat.Authorization.Web.Providers
{
    /// <inheritdoc/>
    public class PermissionProvider : BasePermissionProvider
    {
        /// <inheritdoc/>
        public PermissionProvider(Type permitEnum) : base(permitEnum)
        {
        }

        /// <inheritdoc/>
        public override List<Permit> GetPermissionRequired(params CustomAttributeData[] attributes)
        {
            if (attributes.Any(p => !p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)))) throw new ArgumentException("All attributes must be AuthorizationPermissionsAttribute.");

            var permissionAttrs = attributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));
            var args = permissionAttrs.SelectMany(p => p.ConstructorArguments);
            var permissionNeedsData = new List<Permit>();
            foreach (var arg in args)
            {
                GetAttributePermission(arg, ref permissionNeedsData);
            }
            return permissionNeedsData;
        }
    }
}
