using iCat.Authorization.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iCat.Authorization
{
    public class Core
    {
        private static readonly ConcurrentDictionary<string, List<PermissionData>> _cache = new ConcurrentDictionary<string, List<PermissionData>>();
        private const string _startWith = "Auth";
        private const string _endWith = "Permission";

        public List<PermissionData> GetPermissionData(string cacheKey, CustomAttributeData[] attributes)
        {
            if (!_cache.TryGetValue(cacheKey, out var permissionNeedsData))
            {
                var permissionAttrs = attributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));
                var args = permissionAttrs.SelectMany(p => p.ConstructorArguments);
                permissionNeedsData = new List<PermissionData>();
                foreach (var arg in args)
                {
                    GetPermisionNeeds(arg, ref permissionNeedsData);
                }
                _cache.TryAdd(cacheKey, permissionNeedsData);
            }
            return permissionNeedsData;
        }

        private void GetPermisionNeeds(CustomAttributeTypedArgument arg, ref List<PermissionData> permissionNeeds)
        {
            if (arg.ArgumentType.IsArray)
            {
                var values = arg.Value as ReadOnlyCollection<CustomAttributeTypedArgument>;
                if (values != null)
                    foreach (var value in values)
                        GetPermisionNeeds(value, ref permissionNeeds);
            }
            else if (arg.ArgumentType.IsEnum && arg.ArgumentType.Name.EndsWith(_endWith))
            {
                // Current arg belongs to the function
                var function = _functions?.Any(p => p.ToString() == arg.ArgumentType.Name.Replace(_endWith, "")) ?? throw new ArgumentException("Permissions is not in the function list.")
                    ? _functions.First(p => p.ToString() == arg.ArgumentType.Name.Replace(_endWith, ""))
                    : throw new ArgumentException("Permissions is not in the function list.");

                // The function permission list
                var functionPermissions = (IEnumerable<int>)Enum.GetValues(arg.ArgumentType);

                // Function permission data exists
                var permissionNeed = permissionNeeds.FirstOrDefault(p => p.FunctionValue == (int)function);

                if (permissionNeed == null)
                {
                    permissionNeeds.Add(
                             new PermissionData
                             {
                                 FunctionName = function.ToString(),
                                 FunctionValue = (int)function,
                                 PermissionDetailList = functionPermissions
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
                    var notExistsPermission = c.Where(p => !permissionNeed.PermissionDetailList.Select(x => x.Permission).Any(y => y == p));
                    permissionNeed.PermissionDetailList.AddRange(notExistsPermission.Select(p => new PermissionDetail
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

    }
}
