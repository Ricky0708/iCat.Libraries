using iCat.Authorization.Models;
using iCat.Authorization.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Providers.Implements
{
    /// <inheritdoc/>
    public class PermissionProcessor : IPermissionProcessor
    {
        private readonly List<Permit> _permitData;

        /// <inheritdoc/>
        public PermissionProcessor(Type permitEnum)
        {
            //_permitData ??= DefaultPermissionProvider.GetDefinitions(permitEnum, DefaultPermissionProvider.GetPermissionEnumList(permitEnum));
            _permitData ??= GetDefinitions(permitEnum);
        }

        /// <inheritdoc/>
        public List<Permit> GetDefinitions()
        {
            return _permitData;
        }

        /// <inheritdoc/>
        public Permit GetPermitDefinitionFromPermission<T>(T permission) where T : Enum
        {
            var permit = GetPermitDefinitionFromPermission(permission.GetType());
            return permit;
        }

        /// <inheritdoc/>
        public Permit GetPermitDefinitionFromPermission(Type permissionType)
        {
            var permit = _permitData.Any(p => p.Name == permissionType.Name) ?
                 _permitData.First(p => p.Name == permissionType.Name)
                 : throw new ArgumentException("Permissions is not in the permit list.");
            return permit;
        }

        /// <inheritdoc/>
        public List<Permit> GetPermitFromAttribute(params CustomAttributeData[] attributes)
        {
            var permissionAttrs = attributes;
            var args = permissionAttrs.SelectMany(p => p.ConstructorArguments);
            var permissionNeedsData = new List<Permit>();
            foreach (var arg in args)
            {
                GetAttributePermission(arg, ref permissionNeedsData);
            }
            return permissionNeedsData;
        }

        /// <inheritdoc/>
        public Permit BuildPermit(int permitValue, int permissionsValue)
        {
            var permit = GetDefinitions().FirstOrDefault(x => x.Value == permitValue) ?? throw new ArgumentException("permit in claims is not in permit list");
            return new Permit
            {
                Value = permitValue,
                Name = permit.Name,
                PermissionsData = permit.PermissionsData.Where(x => (x.Value & permissionsValue) > 0).ToList()
            };
        }

        /// <inheritdoc/>
        public Permit BuildPermit<T>(T permission) where T : Enum
        {
            var permit = GetPermitDefinitionFromPermission(permission.GetType()) ?? throw new ArgumentException("permit in claims is not in permit list");
            return new Permit
            {
                Value = permit.Value,
                Name = permit.Name,
                PermissionsData = permit.PermissionsData.Where(x => (x.Value & Convert.ToInt64(permission)) > 0).ToList()
            };
        }

        /// <inheritdoc/>
        public bool ValidatePermission<T>(IEnumerable<IPermit<T>> ownPermits, IPermit<T> requiredPermit) where T : IPermission
        {
            if (ownPermits.Any(p => p.Value == requiredPermit.Value && (p.Permissions & requiredPermit.Permissions) > 0))
            {
                return true;
            }
            return false;
        }

        #region private methods

        /// <summary>
        /// Parsing permit and permission mapping
        /// </summary>
        /// <param name="permitEnum"></param>
        /// <returns></returns>
        private static List<Permit> GetDefinitions(Type permitEnum)
        {
            var permitData = new List<Permit>();

            foreach (var field in permitEnum.GetFields().Where(p => p.Name != "value__"))
            {
                var contructorType = field.CustomAttributes.SingleOrDefault(p => p.AttributeType == typeof(PermissionRelationAttribute))?.ConstructorArguments.First().Value as Type ?? throw new ArgumentNullException($"\"{field.Name}\" has no defined permission attribute.");
                if (contructorType.GetCustomAttribute<FlagsAttribute>() == null) throw new ArgumentException($"Enum {contructorType.Name} have to be flag enum");

                var permit = new Permit
                {
                    Name = contructorType.Name,
                    Value = (int)Enum.Parse(permitEnum, Enum.GetName(permitEnum, field.GetValue(field)!)!),
                };

                if (permitData.Any(p => p.Name == permit.Name)) throw new ArgumentException($"Enum {contructorType.Name} is duplicate");

                foreach (var PermissionItem in Enum.GetValues(contructorType))
                {
                    permit.PermissionsData.Add(new Permission
                    {
                        Name = Enum.GetName(contructorType, PermissionItem),
                        Value = (int)Enum.Parse(contructorType, Enum.GetName(contructorType, PermissionItem)!),
                    });
                }

                permitData.Add(permit);
            }

            //foreach (var permitItem in Enum.GetValues(permitEnum))
            //{
            //    var permit = new Permit
            //    {
            //        Name = Enum.GetName(permitEnum, permitItem),
            //        Value = (int)Enum.Parse(permitEnum, Enum.GetName(permitEnum, permitItem)!),
            //    };

            //    foreach (var permissionType in permissionEnums.Where(p => p.Name.Replace(_endWith, "") == permit.Name))
            //    {
            //        foreach (var PermissionItem in Enum.GetValues(permissionType))
            //        {
            //            permit.PermissionsData.Add(new Permission
            //            {
            //                Name = Enum.GetName(permissionType, PermissionItem),
            //                Value = (int)Enum.Parse(permissionType, Enum.GetName(permissionType, PermissionItem)!),
            //            });
            //        }
            //    }

            //    permitData.Add(permit);
            //}
            return permitData;
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
                var permitDefinition = GetPermitDefinitionFromPermission(arg.ArgumentType);

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

        ///// <summary>
        ///// Get permission enum type list from permit 
        ///// </summary>
        ///// <param name="permitEnumType"></param>
        ///// <returns></returns>
        ///// <exception cref="ArgumentNullException"></exception>
        ///// <exception cref="ArgumentException"></exception>
        //private static Type[] GetPermissionEnumList(Type permitEnumType)
        //{
        //    var permissionList = new List<Type>();
        //    foreach (var field in permitEnumType.GetFields().Where(p => p.Name != "value__"))
        //    {
        //        var permissionAttribute = field.CustomAttributes.SingleOrDefault(p => p.AttributeType == typeof(PermissionAttribute)) ?? throw new ArgumentNullException($"\"{field.Name}\" has no defined permission attribute.");
        //        var value = permissionAttribute.ConstructorArguments.FirstOrDefault().Value as Type ?? throw new ArgumentException($"\"{field.Name}\" has no specify permission.");
        //        if (value.GetCustomAttribute<FlagsAttribute>() == null) throw new ArgumentException($"Enum {value.Name} have to be flag enum");
        //        permissionList.Add(value);
        //    }
        //    return permissionList.ToArray();
        //}

        #endregion
    }
}
