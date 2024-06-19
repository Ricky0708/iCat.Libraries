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
    public abstract class BasePermissionProvider : IPermissionProvider
    {
        private readonly List<Permit> _permitData;

        /// <inheritdoc/>
        public BasePermissionProvider(Type permitEnum)
        {
            //_permitData ??= DefaultPermissionProvider.GetDefinitions(permitEnum, DefaultPermissionProvider.GetPermissionEnumList(permitEnum));
            _permitData ??= GetDefinitions(permitEnum);
        }

        /// <inheritdoc/>
        public abstract List<Permit> GetPermissionRequired(params CustomAttributeData[] attributes);

        /// <inheritdoc/>
        public List<Permit> GetDefinitions()
        {
            return _permitData;
        }

        /// <inheritdoc/>
        public Permit GetPermitFromPermission<T>(T permission) where T : Enum
        {
            var permit = GetPermitFromPermission(permission.GetType());
            return permit;
        }

        /// <inheritdoc/>
        public bool Validate<T>(IEnumerable<IPermit<T>> permissions, IPermit<T> permissionRequired) where T : IPermission
        {
            if (permissions.Any(p => p.Value == permissionRequired.Value && (p.Permissions & permissionRequired.Permissions) > 0))
            {
                return true;
            }
            return false;
        }


        #region private methods

        /// <summary>
        /// Parser AuthorizationPermission constructor
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="permissionNeeds"></param>
        /// <exception cref="ArgumentException"></exception>
        protected void GetAttributePermission(CustomAttributeTypedArgument arg, ref List<Permit> permissionNeeds)
        {
            if (arg.ArgumentType.IsArray)
            {
                if (arg.Value is ReadOnlyCollection<CustomAttributeTypedArgument> values)
                    foreach (var value in values)
                        GetAttributePermission(value, ref permissionNeeds);
            }
            else if (arg.ArgumentType.IsEnum)
            {
                // Current arg belongs to the permit
                var permit = GetPermitFromPermission(arg.ArgumentType);

                // The permission list
                var permissions = (IEnumerable<int>)Enum.GetValues(arg.ArgumentType);

                // permission data exists
                var permissionNeed = permissionNeeds.FirstOrDefault(p => p.Value == permit.Value);

                if (permissionNeed == null)
                {
                    permissionNeeds.Add(
                             new Permit
                             {
                                 Name = permit.Name,
                                 Value = permit.Value,
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

        /// <summary>
        /// Get permit definition from permission type
        /// </summary>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private Permit GetPermitFromPermission(Type permissionType)
        {
            var permit = _permitData.Any(p => p.Name == permissionType.Name) ?
                 _permitData.First(p => p.Name == permissionType.Name)
                 : throw new ArgumentException("Permissions is not in the permit list.");
            return permit;
        }

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
                var contructorType = field.CustomAttributes.SingleOrDefault(p => p.AttributeType == typeof(PermissionAttribute))?.ConstructorArguments.First().Value as Type ?? throw new ArgumentNullException($"\"{field.Name}\" has no defined permission attribute.");
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
