using iCat.Authorization.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization
{
    public class FuncionPermissionParser
    {
        private const string _startWith = "Auth";
        private const string _endWith = "Permission";
        private List<FunctionData> _functionDatas;

        public FuncionPermissionParser(Type functionEnum, params Type[] functionPermissionEnums)
        {
            if (!CheckNamingDefinition(functionEnum, functionPermissionEnums)) throw new ArgumentException("Needs to be an enum type and must follow naming rules. (the name remove suffix from functionPermission type needs to match function type name)");
            _functionDatas = GetPermissionDefinitions(functionEnum, functionPermissionEnums);
        }

        public List<FunctionData> GetPermissionDefinitions()
        {
            return _functionDatas;
        }

        public List<FunctionData> GetAuthorizationPermissionsData(CustomAttributeData[] attributes)
        {
            if (attributes.Any(p => !p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)))) throw new ArgumentException("All attributes must be AuthorizationPermissionsAttribute.");

            var permissionAttrs = attributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));
            var args = permissionAttrs.SelectMany(p => p.ConstructorArguments);
            var permissionNeedsData = new List<FunctionData>();
            foreach (var arg in args)
            {
                GetAttributePermission(arg, ref permissionNeedsData);
            }
            return permissionNeedsData;
        }

        private void GetAttributePermission(CustomAttributeTypedArgument arg, ref List<FunctionData> permissionNeeds)
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
                var function = _functionDatas.Any(p => p.FunctionName == arg.ArgumentType.Name.Replace(_endWith, "")) ?
                    _functionDatas.First(p => p.FunctionName == arg.ArgumentType.Name.Replace(_endWith, ""))
                    : throw new ArgumentException("Permissions is not in the function list.");

                // The function permission list
                var functionPermissions = (IEnumerable<int>)Enum.GetValues(arg.ArgumentType);

                // Function permission data exists
                var permissionNeed = permissionNeeds.FirstOrDefault(p => p.FunctionValue == function.FunctionValue);

                if (permissionNeed == null)
                {
                    permissionNeeds.Add(
                             new FunctionData
                             {
                                 FunctionName = function.FunctionName,
                                 FunctionValue = function.FunctionValue,
                                 PermissionDetails = functionPermissions
                                    .Where(p => ((int)arg.Value! & p) > 0)
                                    .Select(p => new PermissionDetail
                                    {
                                        PermissionName = Enum.GetName(arg.ArgumentType, p!)!,
                                        Permission = p
                                    }).ToList()
                             });
                }
                else
                {
                    var c = functionPermissions.Where(p => ((int)arg.Value! & p) > 0);
                    var notExistsPermission = c.Where(p => !permissionNeed.PermissionDetails.Select(x => x.Permission).Any(y => y == p));
                    permissionNeed.PermissionDetails.AddRange(notExistsPermission.Select(p => new PermissionDetail
                    {
                        PermissionName = Enum.GetName(arg.ArgumentType, p)!,
                        Permission = p
                    }));

                }
            }
            else
            {
                throw new ArgumentException("Constructor parameter type is not an authorization permission type");
            }
        }

        #region private methods
        private List<FunctionData> GetPermissionDefinitions(Type functionEnum, params Type[] functionPermissionEnums)
        {
            var functionDatas = new List<FunctionData>();
            foreach (var functionItem in Enum.GetValues(functionEnum))
            {
                var functionData = new FunctionData
                {
                    FunctionName = Enum.GetName(functionEnum, functionItem),
                    FunctionValue = (int)Enum.Parse(functionEnum, Enum.GetName(functionEnum, functionItem)!),
                };

                foreach (var permissionType in functionPermissionEnums.Where(p => p.Name.Replace(_endWith, "") == functionData.FunctionName))
                {
                    foreach (var PermissionItem in Enum.GetValues(permissionType))
                    {
                        functionData.PermissionDetails.Add(new PermissionDetail
                        {
                            PermissionName = Enum.GetName(permissionType, PermissionItem),
                            Permission = (int)Enum.Parse(permissionType, Enum.GetName(permissionType, PermissionItem)!),
                        });
                    }
                }

                functionDatas.Add(functionData);
            }
            return functionDatas;
        }

        private bool CheckNamingDefinition(Type functionEnum, params Type[] functionPermissionEnums)
        {
            var result = true;
            if (result) result = functionEnum.IsEnum && functionPermissionEnums.All(p => p.IsEnum);
            if (result) result = functionPermissionEnums.All(p => Enum.GetNames(functionEnum).Contains(p.Name.Replace(_endWith, "")));
            return result;
        }

        #endregion
    }
}
