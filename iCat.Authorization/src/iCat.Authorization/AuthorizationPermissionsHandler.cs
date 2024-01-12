using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using iCat.Authorization.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization
{
    /// <summary>
    /// Authorize AuthorizationPermissionsRequirement
    /// </summary>
    public class AuthorizationPermissionsHandler : AuthorizationHandler<AuthorizationPermissionsRequirement>
    {
        private const string _endWith = "Permission";
        private static List<Function>? _functionDatas;
        private static ConcurrentDictionary<string, List<Function>> _routePermissionCache = new ConcurrentDictionary<string, List<Function>>();
        private readonly IFunctionPermissionProvider _provider;

        /// <summary>
        /// Authorize AuthorizationPermissionsRequirement
        /// </summary>
        /// <param name="provider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AuthorizationPermissionsHandler(IFunctionPermissionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            if (_functionDatas == null)
            {
                _functionDatas = _provider.GetFunctionPermissionDefinitions();
            }
        }

        /// <summary>
        /// handler
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationPermissionsRequirement requirement)
        {
            if (!context.User.Identity?.IsAuthenticated ?? false) { context.Fail(); return; }

            if (context.Resource is HttpContext httpContext)
            {
                var routerPermissions = GetRouterPermissions(httpContext);
                var userPermissions = _provider.GetUserPermission();
                foreach (var routerPermission in routerPermissions)
                {
                    if (_provider.Validate(userPermissions, routerPermission))
                    {
                        context.Succeed(requirement);
                        await Task.FromResult(0);
                        return;
                    }
                }
                context.Fail();
            }
            else
            {
                context.Fail();
            }
            await Task.FromResult(0);
        }

        private List<Function> GetRouterPermissions(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint()!;
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()!;
            var cacheKey = $"{actionDescriptor.ControllerName}{actionDescriptor.ActionName}{string.Join("-", actionDescriptor.Parameters.Select(p => p.ParameterType.Name))}";

            if (!_routePermissionCache.TryGetValue(cacheKey, out var permissionNeedsData))
            {
                var permissionAttrs = actionDescriptor!.MethodInfo.CustomAttributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));
                permissionNeedsData = _provider.GetAuthorizationPermissionsData(permissionAttrs.ToArray());
                _routePermissionCache.TryAdd(cacheKey, permissionNeedsData);
            }
            return permissionNeedsData;
        }

    }
}
