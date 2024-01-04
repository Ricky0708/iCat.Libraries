using iCat.Authorization.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization
{
    public class AuthorizationPermissionsHandler : AuthorizationHandler<AuthorizationPermissionsRequirement>
    {
        private const string _startWith = "Auth";
        private const string _endWith = "Permission";
        private static List<Function>? _functions;
        private static ConcurrentDictionary<string, List<PermissionData>> _cache = new ConcurrentDictionary<string, List<PermissionData>>();

        public AuthorizationPermissionsHandler()
        {
            if (_functions == null)
            {
                _functions =
                    Enum
                    .GetValues(
                        AppDomain.CurrentDomain.GetAssemblies().SelectMany(p => p.GetTypes().Where(t => t.IsEnum && t.IsPublic && t.Name == nameof(Function))).Single())
                    .OfType<Function>()
                    .ToList();
            }
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationPermissionsRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                if (context.Resource is HttpContext httpContext)
                {
                    var endpoint = httpContext.GetEndpoint()!;
                    var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()!;
                    var cacheKey = $"{actionDescriptor.ControllerName}{actionDescriptor.ActionName}{string.Join("-", actionDescriptor.Parameters.Select(p => p.ParameterType.Name))}";

                    if (!_cache.TryGetValue(cacheKey, out var permissionNeedsData))
                    {
                        var permissionAttrs = actionDescriptor!.MethodInfo.CustomAttributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));
                        var args = permissionAttrs.SelectMany(p => p.ConstructorArguments);
                        permissionNeedsData = new List<PermissionData>();
                        foreach (var arg in args)
                        {
                            GetPermisionNeeds(arg, ref permissionNeedsData);
                        }
                        _cache.TryAdd(cacheKey, permissionNeedsData);
                    }

                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }
            await Task.FromResult(0);
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
