using iCat.Authorization.Providers.Implements;
using iCat.Authorization.Models;
using iCat.Authorization.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections.ObjectModel;

namespace iCat.Authorization.Web.Providers
{
    /// <inheritdoc/>
    public class PermissionProvider 
    {
        private readonly IPermissionProcessor _permissionProvider;

        /// <inheritdoc/>
        public PermissionProvider(IPermissionProcessor permissionProvider)
        {
            _permissionProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
        }

        /// <summary>
        /// Get AuthorizationPermissin attribute information
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public List<Permit> GetPermissionRequired(params CustomAttributeData[] attributes)
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

        /// <summary>
        /// Parser AuthorizationPermission constructor
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="permissionNeeds"></param>
        /// <exception cref="ArgumentException"></exception>
        private void GetAttributePermission(CustomAttributeTypedArgument arg, ref List<Permit> permissionNeeds)
        {
            if (arg.ArgumentType.IsArray)
            {
                if (arg.Value is ReadOnlyCollection<CustomAttributeTypedArgument> values)
                    foreach (var value in values)
                        GetAttributePermission(value, ref permissionNeeds);
            }
            else if (arg.ArgumentType.IsEnum)
            {
                // permit definition
                var permitDefinition = _permissionProvider.GetPermitDefinitionFromPermission(arg.ArgumentType);

                // The permission list
                var permissions = (IEnumerable<int>)Enum.GetValues(arg.ArgumentType);

                // permission data exists
                var permissionNeed = permissionNeeds.FirstOrDefault(p => p.Value == permitDefinition.Value);

                if (permissionNeed == null)
                {
                    permissionNeeds.Add(
                             new Permit
                             {
                                 Name = permitDefinition.Name,
                                 Value = permitDefinition.Value,
                                 PermissionsData = permissions
                                    .Where(p => ((int)arg.Value! & p) > 0)
                                    .Select(p => new Permission
                                    {
                                        Name = Enum.GetName(arg.ArgumentType, p!)!,
                                        Value = p
                                    }).ToList()
                             });
                }
                else
                {
                    var c = permissions.Where(p => ((int)arg.Value! & p) > 0);
                    var notExistsPermission = c.Where(p => !permissionNeed.PermissionsData.Select(x => x.Value).Any(y => y == p));
                    permissionNeed.PermissionsData.AddRange(notExistsPermission.Select(p => new Permission
                    {
                        Name = Enum.GetName(arg.ArgumentType, p)!,
                        Value = p
                    }));

                }
            }
            else
            {
                throw new ArgumentException("Constructor parameter type is not an authorization permission type");
            }
        }
    }
}
