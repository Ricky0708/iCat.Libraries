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
    public class PrivilegeProvider : IPrivilegeProvider
    {

        private static readonly ConcurrentDictionary<string, List<Privilege>> _routePermissionCache = new();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimProcessor _claimProcessor;
        private readonly IPermissionProcessor _permissionProcessor;

        /// <inheritdoc/>
        public PrivilegeProvider(
            IHttpContextAccessor httpContextAccessor,
            IClaimProcessor claimProcessor,
            IPermissionProcessor permissionProcessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _claimProcessor = claimProcessor ?? throw new ArgumentNullException(nameof(claimProcessor));
            _permissionProcessor = permissionProcessor ?? throw new ArgumentNullException(nameof(permissionProcessor));
        }

        /// <inheritdoc/>
        public IEnumerable<Privilege> GetCurrentUserPrivileges()
        {
            var userPrivileges = _httpContextAccessor?.HttpContext?.User.Claims.Where(p => p.Type == Constants.ClaimTypes.Privilege).Select(p =>
            {
                var PrivilegeClaimData = p.Value.Split(",");
                if (!int.TryParse(PrivilegeClaimData[0], out var privilegeValue)) throw new ArgumentException("Invalid Privilege claims");
                if (!int.TryParse(PrivilegeClaimData[1], out var permissionsValue)) throw new ArgumentException("Invalid Privilege claims");
                return _permissionProcessor.BuildPrivilege(privilegeValue, permissionsValue);
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPrivileges;
        }

        /// <inheritdoc/>
        public IEnumerable<Privilege> GetUserPrivileges(HttpContext httpContext)
        {
            var userPrivileges = httpContext.User.Claims.Where(p => p.Type == Constants.ClaimTypes.Privilege).Select(p =>
            {
                var privilegeClaimData = p.Value.Split(",");
                if (!int.TryParse(privilegeClaimData[0], out var privilegeValue)) throw new ArgumentException("Invalid Privilege claims");
                if (!int.TryParse(privilegeClaimData[1], out var permissionsValue)) throw new ArgumentException("Invalid Privilege claims");
                return _permissionProcessor.BuildPrivilege(privilegeValue, permissionsValue);
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPrivileges;
        }

        /// <inheritdoc/>
        public List<Privilege> GetRouterPrivilegesRequired(Endpoint endpoint)
        {
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()!;
            var cacheKey = $"{actionDescriptor.ControllerName}{actionDescriptor.ActionName}{string.Join("-", actionDescriptor.Parameters.Select(p => p.ParameterType.Name))}";

            if (!_routePermissionCache.TryGetValue(cacheKey, out var permissionNeedsData))
            {
                if (!(actionDescriptor!.MethodInfo.CustomAttributes.Any(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute))))) throw new ArgumentException("Not have AuthorizationPermissionsAttribute.");
                var permissionAttrs = actionDescriptor!.MethodInfo.CustomAttributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));

                permissionNeedsData = _permissionProcessor.GetPrivilegeFromAttribute(permissionAttrs.ToArray());
                _routePermissionCache.TryAdd(cacheKey, permissionNeedsData);
            }
            return permissionNeedsData;
        }

        /// <inheritdoc/>
        public Claim GenerateClaim(Privilege privilege)
        {
            var claim = _claimProcessor.GeneratePrivilegeClaim(privilege);
            return claim;
        }

        /// <inheritdoc/>
        public Claim GenerateClaim<TPermission>(TPermission permission) where TPermission : Enum
        {
            var privilege = _permissionProcessor.GetPrivilegeDefinitionFromPermission(permission);
            var claim = _claimProcessor.GeneratePrivilegeClaim(privilege);
            return claim;
        }

        /// <inheritdoc/>
        public bool ValidatePermission(IEnumerable<Privilege> userPrivilege, Privilege routerPrivilege)
        {
            return _permissionProcessor.ValidatePermission(userPrivilege, routerPrivilege);
        }


    }
}
