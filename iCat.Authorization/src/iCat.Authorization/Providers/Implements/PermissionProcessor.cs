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
        private readonly List<Privilege> _privilegeData;

        /// <inheritdoc/>
        public PermissionProcessor(Type privilegeEnum)
        {
            _privilegeData ??= GetDefinitions(privilegeEnum);
        }

        /// <inheritdoc/>
        public List<Privilege> GetDefinitions()
        {
            return _privilegeData;
        }

        /// <inheritdoc/>
        public Privilege GetPrivilegeDefinitionFromPermission<T>(T permission) where T : Enum
        {
            var privilege = GetPrivilegeDefinitionFromPermission(typeof(T));
            return privilege;
        }

        /// <inheritdoc/>
        public Privilege GetPrivilegeDefinitionFromPermission(Type permissionType)
        {
            var privilege = _privilegeData.Any(p => p.Name == permissionType.Name) ?
                 _privilegeData.First(p => p.Name == permissionType.Name)
                 : throw new ArgumentException("Permissions is not in the privilege list.");
            return privilege;
        }

        /// <inheritdoc/>
        public List<Privilege> GetPrivilegeFromAttribute(params CustomAttributeData[] attributes)
        {
            var permissionAttrs = attributes;
            var args = permissionAttrs.SelectMany(p => p.ConstructorArguments);
            var permissionNeedsData = new List<Privilege>();
            foreach (var arg in args)
            {
                GetAttributePermission(arg, ref permissionNeedsData);
            }
            return permissionNeedsData;
        }

        /// <inheritdoc/>
        public Privilege BuildPrivilege(int privilegeValue, int permissionsValue)
        {
            var privilege = GetDefinitions().FirstOrDefault(x => x.Value == privilegeValue) ?? throw new ArgumentException("Privilege in claims is not in privilege list");
            return new Privilege
            {
                Value = privilegeValue,
                Name = privilege.Name,
                PermissionsData = privilege.PermissionsData.Where(x => (x.Value & permissionsValue) > 0).ToList()
            };
        }

        /// <inheritdoc/>
        public Privilege BuildPrivilege<T>(T permission) where T : Enum
        {
            var privilege = GetPrivilegeDefinitionFromPermission(permission.GetType()) ?? throw new ArgumentException("Privilege in claims is not in privilege list");
            return new Privilege
            {
                Value = privilege.Value,
                Name = privilege.Name,
                PermissionsData = privilege.PermissionsData.Where(x => (x.Value & Convert.ToInt64(permission)) > 0).ToList()
            };
        }

        /// <inheritdoc/>
        public bool ValidatePermission<T>(IEnumerable<IPrivilege<T>> ownPrivileges, IPrivilege<T> requiredPrivilege) where T : IPermission
        {
            if (ownPrivileges.Any(p => p.Value == requiredPrivilege.Value && (p.Permissions & requiredPrivilege.Permissions) > 0))
            {
                return true;
            }
            return false;
        }

        #region private methods

        /// <summary>
        /// Parsing privilege and permission mapping
        /// </summary>
        /// <param name="privilegeEnum"></param>
        /// <returns></returns>
        private static List<Privilege> GetDefinitions(Type privilegeEnum)
        {
            var privilegeData = new List<Privilege>();

            foreach (var field in privilegeEnum.GetFields().Where(p => p.Name != "value__"))
            {
                var contructorType = field.CustomAttributes.SingleOrDefault(p => p.AttributeType == typeof(PrivilegDetailAttribute))?.ConstructorArguments.First().Value as Type ?? throw new ArgumentNullException($"\"{field.Name}\" has no defined permission attribute.");
                if (contructorType.GetCustomAttribute<FlagsAttribute>() == null) throw new ArgumentException($"Enum {contructorType.Name} have to be flag enum");

                var privilege = new Privilege
                {
                    Name = contructorType.Name,
                    Value = (int)Enum.Parse(privilegeEnum, Enum.GetName(privilegeEnum, field.GetValue(field)!)!),
                };

                if (privilegeData.Any(p => p.Name == privilege.Name)) throw new ArgumentException($"Enum {contructorType.Name} is duplicate");

                foreach (var PermissionItem in Enum.GetValues(contructorType))
                {
                    privilege.PermissionsData.Add(new Permission
                    {
                        Name = Enum.GetName(contructorType, PermissionItem),
                        Value = (int)Enum.Parse(contructorType, Enum.GetName(contructorType, PermissionItem)!),
                    });
                }

                privilegeData.Add(privilege);
            }
            return privilegeData;
        }

        /// <summary>
        /// Parser AuthorizationPermission constructor
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="permissionNeeds"></param>
        /// <exception cref="ArgumentException"></exception>
        private void GetAttributePermission(CustomAttributeTypedArgument arg, ref List<Privilege> permissionNeeds)
        {
            if (arg.ArgumentType.IsArray)
            {
                if (arg.Value is ReadOnlyCollection<CustomAttributeTypedArgument> values)
                    foreach (var value in values)
                        GetAttributePermission(value, ref permissionNeeds);
            }
            else if (arg.ArgumentType.IsEnum)
            {
                // Privilege definition
                var privilegeDefinition = GetPrivilegeDefinitionFromPermission(arg.ArgumentType);

                // The permission list
                var permissions = (IEnumerable<int>)Enum.GetValues(arg.ArgumentType);

                // permission data exists
                var permissionNeed = permissionNeeds.FirstOrDefault(p => p.Value == privilegeDefinition.Value);

                if (permissionNeed == null)
                {
                    permissionNeeds.Add(
                             new Privilege
                             {
                                 Name = privilegeDefinition.Name,
                                 Value = privilegeDefinition.Value,
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



        #endregion
    }
}
