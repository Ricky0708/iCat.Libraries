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
        private static List<FunctionPermissionData>? _functionDatas;
        private static ConcurrentDictionary<string, List<FunctionPermissionData>> _routePermissionCache = new ConcurrentDictionary<string, List<FunctionPermissionData>>();
        private readonly FunctionPermissionParser _parser;
        private readonly IFunctionPermissionProvider _userPermissionProvider;

        /// <summary>
        /// Authorize AuthorizationPermissionsRequirement
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="userPermissionProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AuthorizationPermissionsHandler(FunctionPermissionParser parser, IFunctionPermissionProvider userPermissionProvider)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _userPermissionProvider = userPermissionProvider ?? throw new ArgumentNullException(nameof(userPermissionProvider));
            if (_functionDatas == null)
            {
                _functionDatas = _parser.GetFunctionPermissionDefinitions();
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
                var userPermissions = _userPermissionProvider.GetUserPermission();
                foreach (var routerPermission in routerPermissions)
                {
                    if (_userPermissionProvider.Validate(userPermissions, routerPermission))
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

        private List<FunctionPermissionData> GetRouterPermissions(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint()!;
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()!;
            var cacheKey = $"{actionDescriptor.ControllerName}{actionDescriptor.ActionName}{string.Join("-", actionDescriptor.Parameters.Select(p => p.ParameterType.Name))}";

            if (!_routePermissionCache.TryGetValue(cacheKey, out var permissionNeedsData))
            {
                var permissionAttrs = actionDescriptor!.MethodInfo.CustomAttributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));
                permissionNeedsData = _parser.GetAuthorizationPermissionsData(permissionAttrs.ToArray());
                _routePermissionCache.TryAdd(cacheKey, permissionNeedsData);
            }
            return permissionNeedsData;
        }

    }
}
