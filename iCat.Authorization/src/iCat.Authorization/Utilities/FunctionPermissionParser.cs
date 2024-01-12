using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    /// <summary>
    /// FunctionPermission enum type parser
    /// </summary>
    internal sealed class FunctionPermissionParser
    {
        private const string _endWith = "Permission";
        private readonly List<Function> _functionDatas;

        /// <summary>
        /// FunctionPermission enum type parser
        /// </summary>
        /// <param name="functionEnum"></param>
        /// <exception cref="ArgumentException"></exception>
        public FunctionPermissionParser(Type functionEnum)
        {
            _functionDatas ??= GetPermissionDefinitions(functionEnum, GetFunctionPermissinoFromAttribute(functionEnum));
        }

        ///// <summary>
        ///// FunctionPermission enum type parser
        ///// </summary>
        ///// <param name="functionEnum"></param>
        ///// <param name="functionPermissionEnums"></param>
        ///// <exception cref="ArgumentException"></exception>
        //public FunctionPermissionParser(Type functionEnum, params Type[] functionPermissionEnums)
        //{
        //    if (!CheckNamingDefinition(functionEnum, functionPermissionEnums)) throw new ArgumentException("Needs to be an enum type and must follow naming rules. (the name remove suffix from functionPermission type needs to match function type name)");
        //    _functionDatas ??= GetPermissionDefinitions(functionEnum, functionPermissionEnums);
        //}

        /// <summary>
        /// Get function and permission mapping
        /// </summary>
        /// <returns></returns>
        public List<Function> GetFunctionPermissionDefinitions()
        {
            return _functionDatas;
        }

        /// <summary>
        /// Get AuthorizationPermissin attribute information
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public List<Function> GetAuthorizationPermissionsData(params CustomAttributeData[] attributes)
        {
            if (attributes.Any(p => !p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)))) throw new ArgumentException("All attributes must be AuthorizationPermissionsAttribute.");

            var permissionAttrs = attributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));
            var args = permissionAttrs.SelectMany(p => p.ConstructorArguments);
            var permissionNeedsData = new List<Function>();
            foreach (var arg in args)
            {
                GetAttributePermission(arg, ref permissionNeedsData);
            }
            return permissionNeedsData;
        }

        /// <summary>
        /// Get claim from function permission data
        /// </summary>
        /// <param name="functionPermissionData"></param>
        /// <returns></returns>
        public Claim GetClaimFromFunctionPermissionData(Function functionPermissionData)
        {
            var claim = new Claim(AuthorizationPermissionClaimTypes.Permission, $"{functionPermissionData.Value},{functionPermissionData.Permissions}");
            return claim;
        }

        #region private methods

        /// <summary>
        /// Get the specified permission from the attribute of field in the function
        /// </summary>
        /// <param name="functionEnumType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private Type[] GetFunctionPermissinoFromAttribute(Type functionEnumType)
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

        /// <summary>
        /// Parser AuthorizationPermission constructor
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="permissionNeeds"></param>
        /// <exception cref="ArgumentException"></exception>
        private void GetAttributePermission(CustomAttributeTypedArgument arg, ref List<Function> permissionNeeds)
        {
            if (arg.ArgumentType.IsArray)
            {
                var values = arg.Value as ReadOnlyCollection<CustomAttributeTypedArgument>;
                if (values != null)
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
                             new Function
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
        private List<Function> GetPermissionDefinitions(Type functionEnum, params Type[] functionPermissionEnums)
        {
            var functionDatas = new List<Function>();
            foreach (var functionItem in Enum.GetValues(functionEnum))
            {
                var functionData = new Function
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
        /// Check type and naming rule
        /// </summary>
        /// <param name="functionEnum"></param>
        /// <param name="functionPermissionEnums"></param>
        /// <returns></returns>
        private bool CheckNamingDefinition(Type functionEnum, params Type[] functionPermissionEnums)
        {
            var result = true;
            if (result) result = functionEnum.IsEnum && functionPermissionEnums.All(p => p.IsEnum);
            if (result) result = functionPermissionEnums.All(p => Enum.GetNames(functionEnum).Contains(p.Name.Replace(_endWith, "")));
            if (result) result = Enum.GetNames(functionEnum).All(p => functionPermissionEnums.Any(d => d.Name.Replace(_endWith, "") == p));
            return result;
        }

        #endregion
    }
}
