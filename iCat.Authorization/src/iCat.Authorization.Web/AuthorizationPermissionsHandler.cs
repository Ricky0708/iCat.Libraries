using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using iCat.Authorization.Providers.Interfaces;
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
using System.Xml.Linq;

namespace iCat.Authorization.Web
{
    /// <summary>
    /// Authorize AuthorizationPermissionsRequirement
    /// </summary>
    public class AuthorizationPermissionsHandler : AuthorizationHandler<AuthorizationPermissionsRequirement>
    {
        private const string _endWith = "Permission";
        private static readonly ConcurrentDictionary<string, List<Permit>> _routePermissionCache = new();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionProcessor _permissionProvider;
        private readonly IClaimProcessor _permitClaimProcessor;

        /// <summary>
        /// Authorize AuthorizationPermissionsRequirement
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="permissionProvider"></param>
        /// <param name="permitClaimProcessor"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AuthorizationPermissionsHandler(
            IHttpContextAccessor httpContextAccessor,
            IPermissionProcessor permissionProvider,
            IClaimProcessor permitClaimProcessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
            _permitClaimProcessor = permitClaimProcessor ?? throw new ArgumentNullException(nameof(permitClaimProcessor));
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
                var routerPermits = GetRouterPermits(httpContext);
                var userPermit = GetPermits();
                foreach (var routerPermit in routerPermits)
                {
                    if (_permissionProvider.ValidatePermission(userPermit, routerPermit))
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

        /// <summary>
        /// Get currently authenticated user permit
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Permit> GetPermits()
        {
            var userPermits = _httpContextAccessor?.HttpContext?.User.Claims.Where(p => p.Type == Constants.ClaimTypes.Permit).Select(p =>
            {
                var permitClaimData = p.Value.Split(",");
                if (!int.TryParse(permitClaimData[0], out var permitValue)) throw new ArgumentException("Invalid Permit claims");
                if (!int.TryParse(permitClaimData[1], out var permissionsValue)) throw new ArgumentException("Invalid Permit claims");
                return _permissionProvider.BuildPermit(permitValue, permissionsValue);
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPermits;
        }

        private List<Permit> GetRouterPermits(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint()!;
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()!;
            var cacheKey = $"{actionDescriptor.ControllerName}{actionDescriptor.ActionName}{string.Join("-", actionDescriptor.Parameters.Select(p => p.ParameterType.Name))}";

            if (!_routePermissionCache.TryGetValue(cacheKey, out var permissionNeedsData))
            {
                if (!(actionDescriptor!.MethodInfo.CustomAttributes.Any(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute))))) throw new ArgumentException("Not have AuthorizationPermissionsAttribute.");
                var permissionAttrs = actionDescriptor!.MethodInfo.CustomAttributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));

                permissionNeedsData = _permissionProvider.GetPermitFromAttribute(permissionAttrs.ToArray());
                _routePermissionCache.TryAdd(cacheKey, permissionNeedsData);
            }
            return permissionNeedsData;
        }

    }
}
