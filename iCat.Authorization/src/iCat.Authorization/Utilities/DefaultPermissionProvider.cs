using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    /// <inheritdoc/>
    public class DefaultPermissionProvider : IPermissionProvider
    {
        private const string _endWith = "Permission";
        private readonly List<Permit> _functionDatas;

        /// <inheritdoc/>
        public DefaultPermissionProvider(Type functionEnum)
        {
            _functionDatas ??= DefaultPermissionProvider.GetDefinitions(functionEnum, DefaultPermissionProvider.GetPermissionEnumList(functionEnum));
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public List<Permit> GetDefinitions()
        {
            return _functionDatas;
        }

        #region private methods

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
            else if (arg.ArgumentType.IsEnum && arg.ArgumentType.Name.EndsWith(_endWith))
            {
                // Current arg belongs to the function
                var function = _functionDatas.Any(p => p.Name == arg.ArgumentType.Name.Replace(_endWith, "")) ?
                    _functionDatas.First(p => p.Name == arg.ArgumentType.Name.Replace(_endWith, ""))
                    : throw new ArgumentException("Permissions is not in the function list.");

                // The function permission list
                var functionPermissions = (IEnumerable<int>)Enum.GetValues(arg.ArgumentType);

                // Function permission data exists
                var permissionNeed = permissionNeeds.FirstOrDefault(p => p.Value == function.Value);

                if (permissionNeed == null)
                {
                    permissionNeeds.Add(
                             new Permit
                             {
                                 Name = function.Name,
                                 Value = function.Value,
                                 PermissionsData = functionPermissions
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
                    var c = functionPermissions.Where(p => ((int)arg.Value! & p) > 0);
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

        /// <summary>
        /// Parsing function and permission mapping
        /// </summary>
        /// <param name="functionEnum"></param>
        /// <param name="functionPermissionEnums"></param>
        /// <returns></returns>
        private static List<Permit> GetDefinitions(Type functionEnum, params Type[] functionPermissionEnums)
        {
            var functionDatas = new List<Permit>();
            foreach (var functionItem in Enum.GetValues(functionEnum))
            {
                var functionData = new Permit
                {
                    Name = Enum.GetName(functionEnum, functionItem),
                    Value = (int)Enum.Parse(functionEnum, Enum.GetName(functionEnum, functionItem)!),
                };

                foreach (var permissionType in functionPermissionEnums.Where(p => p.Name.Replace(_endWith, "") == functionData.Name))
                {
                    foreach (var PermissionItem in Enum.GetValues(permissionType))
                    {
                        functionData.PermissionsData.Add(new Permission
                        {
                            Name = Enum.GetName(permissionType, PermissionItem),
                            Value = (int)Enum.Parse(permissionType, Enum.GetName(permissionType, PermissionItem)!),
                        });
                    }
                }

                functionDatas.Add(functionData);
            }
            return functionDatas;
        }

        /// <summary>
        /// Get permission enum type list from function 
        /// </summary>
        /// <param name="functionEnumType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private static Type[] GetPermissionEnumList(Type functionEnumType)
        {
            var permissionList = new List<Type>();
            foreach (var field in functionEnumType.GetFields().Where(p => p.Name != "value__"))
            {
                var permissionAttribute = field.CustomAttributes.SingleOrDefault(p => p.AttributeType == typeof(PermissionAttribute)) ?? throw new ArgumentNullException($"\"{field.Name}\" has no defined permission attribute.");
                var value = permissionAttribute.ConstructorArguments.FirstOrDefault().Value as Type ?? throw new ArgumentException($"\"{field.Name}\" has no specify permission.");
                if (value.GetCustomAttribute<FlagsAttribute>() == null) throw new ArgumentException($"Enum {value.Name} have to be flag enum");
                permissionList.Add(value);
            }
            return permissionList.ToArray();
        }

        #endregion



    }
}
