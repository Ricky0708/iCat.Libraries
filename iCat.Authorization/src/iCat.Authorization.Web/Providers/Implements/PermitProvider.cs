using iCat.Authorization.Models;
using iCat.Authorization.Providers.Implements;
using iCat.Authorization.Providers.Interfaces;
using iCat.Authorization.Web.Providers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Web.Providers.Implements
{
    /// <inheritdoc/>
    public class PermitProvider : IPermitProvider
    {

        private static readonly ConcurrentDictionary<string, List<Permit>> _routePermissionCache = new();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimProcessor _claimProcessor;
        private readonly IPermissionProcessor _permissionProcessor;

        /// <inheritdoc/>
        public PermitProvider(
            IHttpContextAccessor httpContextAccessor,
            IClaimProcessor claimProcessor,
            IPermissionProcessor permissionProcessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _claimProcessor = claimProcessor ?? throw new ArgumentNullException(nameof(claimProcessor));
            _permissionProcessor = permissionProcessor ?? throw new ArgumentNullException(nameof(permissionProcessor));
        }

        /// <inheritdoc/>
        public IEnumerable<Permit> GetCurrentUserPermits()
        {
            var userPermits = _httpContextAccessor?.HttpContext?.User.Claims.Where(p => p.Type == Constants.ClaimTypes.Permit).Select(p =>
            {
                var permitClaimData = p.Value.Split(",");
                if (!int.TryParse(permitClaimData[0], out var permitValue)) throw new ArgumentException("Invalid Permit claims");
                if (!int.TryParse(permitClaimData[1], out var permissionsValue)) throw new ArgumentException("Invalid Permit claims");
                return _permissionProcessor.BuildPermit(permitValue, permissionsValue);
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPermits;
        }

        /// <inheritdoc/>
        public IEnumerable<Permit> GetUserPermits(HttpContext httpContext)
        {
            var userPermits = httpContext.User.Claims.Where(p => p.Type == Constants.ClaimTypes.Permit).Select(p =>
            {
                var permitClaimData = p.Value.Split(",");
                if (!int.TryParse(permitClaimData[0], out var permitValue)) throw new ArgumentException("Invalid Permit claims");
                if (!int.TryParse(permitClaimData[1], out var permissionsValue)) throw new ArgumentException("Invalid Permit claims");
                return _permissionProcessor.BuildPermit(permitValue, permissionsValue);
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPermits;
        }

        /// <inheritdoc/>
        public List<Permit> GetRouterPermitsRequired(Endpoint endpoint)
        {
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()!;
            var cacheKey = $"{actionDescriptor.ControllerName}{actionDescriptor.ActionName}{string.Join("-", actionDescriptor.Parameters.Select(p => p.ParameterType.Name))}";

            if (!_routePermissionCache.TryGetValue(cacheKey, out var permissionNeedsData))
            {
                if (!(actionDescriptor!.MethodInfo.CustomAttributes.Any(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute))))) throw new ArgumentException("Not have AuthorizationPermissionsAttribute.");
                var permissionAttrs = actionDescriptor!.MethodInfo.CustomAttributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));

                permissionNeedsData = _permissionProcessor.GetPermitFromAttribute(permissionAttrs.ToArray());
                _routePermissionCache.TryAdd(cacheKey, permissionNeedsData);
            }
            return permissionNeedsData;
        }

        /// <inheritdoc/>
        public Claim GenerateClaim<T>(IPermit<T> permission) where T : IPermission
        {
            var claim = _claimProcessor.GeneratePermitClaim(permission.Value!.Value, permission.Permissions);
            return claim;
        }

        /// <inheritdoc/>
        public Claim GenerateClaim<TPermission>(TPermission permission) where TPermission : Enum
        {
            var permit = _permissionProcessor.GetPermitDefinitionFromPermission(permission);
            var claim = _claimProcessor.GeneratePermitClaim(permit.Value!.Value, (int)(object)permission);
            return claim;
        }

        /// <inheritdoc/>
        public Claim GenerateClaim(int permit, int permission)
        {
            var claim = _claimProcessor.GeneratePermitClaim(permit, permission);
            return claim;
        }

        /// <inheritdoc/>
        public bool ValidatePermission(IEnumerable<Permit> userPermit, Permit routerPermit)
        {
            return _permissionProcessor.ValidatePermission(userPermit, routerPermit);
        }


    }
}
