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
    public class PrivilegeProvider<T> : IPrivilegeProvider<T> where T : Enum
    {

        private static readonly ConcurrentDictionary<string, List<Privilege<T>>> _routePermissionCache = new();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimProcessor<T> _claimProcessor;
        private readonly IPrivilegeProcessor<T> _privilegeProcessor;

        /// <inheritdoc/>
        public PrivilegeProvider(
            IHttpContextAccessor httpContextAccessor,
            IClaimProcessor<T> claimProcessor,
            IPrivilegeProcessor<T> privilegeProcessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _claimProcessor = claimProcessor ?? throw new ArgumentNullException(nameof(claimProcessor));
            _privilegeProcessor = privilegeProcessor ?? throw new ArgumentNullException(nameof(privilegeProcessor));
        }


        /// <inheritdoc/>
        public Privilege<T> BuildPrivilege(int privilegeValue, int permissionsValue)
        {
            return _privilegeProcessor.BuildPrivilege(privilegeValue, permissionsValue);
        }

        /// <inheritdoc/>
        public Privilege<T> BuildPrivilege<E>(E permissionEnum) where E : Enum
        {
            return _privilegeProcessor.BuildPrivilege(permissionEnum);
        }

        /// <inheritdoc/>
        public IEnumerable<Privilege<T>> GetCurrentUserPrivileges()
        {
            return GetUserPrivileges(_httpContextAccessor.HttpContext!);
        }

        /// <inheritdoc/>
        public IEnumerable<Privilege<T>> GetUserPrivileges(HttpContext httpContext)
        {
            var userPrivileges = httpContext.User.Claims.Where(p => p.Type == Constants.ClaimTypes.Privilege).Select(p =>
            {
                var privilegeClaimData = p.Value.Split(",");
                if (!int.TryParse(privilegeClaimData[0], out var privilegeValue)) throw new ArgumentException("Invalid Privilege claims");
                if (!int.TryParse(privilegeClaimData[1], out var permissionsValue)) throw new ArgumentException("Invalid Privilege claims");
                //var enumValue = (T)(object)privilegeValue;
                //var contructorType = enumValue.GetType().GetMember(enumValue.ToString())[0].CustomAttributes.SingleOrDefault(p => p.AttributeType == typeof(PrivilegeDetailAttribute))?.ConstructorArguments.First().Value as Type;
                return _privilegeProcessor.BuildPrivilege(privilegeValue, permissionsValue);
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPrivileges;
        }

        /// <inheritdoc/>
        public List<Privilege<T>> GetRouterPrivilegesRequired(Endpoint endpoint)
        {
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()!;
            var cacheKey = $"{actionDescriptor.ControllerName}{actionDescriptor.ActionName}{string.Join("-", actionDescriptor.Parameters.Select(p => p.ParameterType.Name))}";

            if (!_routePermissionCache.TryGetValue(cacheKey, out var permissionNeedsData))
            {
                if (!(actionDescriptor!.MethodInfo.CustomAttributes.Any(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute))))) throw new ArgumentException("Not have AuthorizationPermissionsAttribute.");
                var permissionAttrs = actionDescriptor!.MethodInfo.CustomAttributes.Where(p => p.AttributeType.Name.StartsWith(nameof(AuthorizationPermissionsAttribute)));

                permissionNeedsData = _privilegeProcessor.GetPrivilegeFromAttribute(permissionAttrs.ToArray());
                _routePermissionCache.TryAdd(cacheKey, permissionNeedsData);
            }
            return permissionNeedsData;
        }

        /// <inheritdoc/>
        public Claim GenerateClaim(Privilege<T> privilege)
        {
            var claim = _claimProcessor.GeneratePrivilegeClaim(privilege);
            return claim;
        }

        /// <inheritdoc/>
        public Claim GenerateClaim<TPermission>(TPermission permissionEnum) where TPermission : Enum
        {
            var privilege = _privilegeProcessor.BuildPrivilege(permissionEnum);
            var claim = _claimProcessor.GeneratePrivilegeClaim(privilege);
            return claim;
        }

        /// <inheritdoc/>
        public bool ValidatePermission(IEnumerable<Privilege<T>> userPrivilege, Privilege<T> routerPrivilege)
        {
            return _privilegeProcessor.ValidatePermission(userPrivilege, routerPrivilege);
        }

    }
}
