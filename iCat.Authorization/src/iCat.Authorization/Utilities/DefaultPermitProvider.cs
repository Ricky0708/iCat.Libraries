using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    /// <inheritdoc/>
    public class DefaultPermitProvider : IPermitProvider
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IPermissionProvider _permissionProvider;

        /// <inheritdoc/>
        public DefaultPermitProvider(IHttpContextAccessor? httpContextAccessor, IPermissionProvider permissionProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
        }

        /// <inheritdoc/>
        public Claim GeneratePermitClaim(Permit permission)
        {
            var claim = new Claim(AuthorizationPermissionClaimTypes.Permit, $"{permission.Value},{permission.Permissions}");
            return claim;
        }

        /// <inheritdoc/>
        public IEnumerable<Permit> GetPermit()
        {
            var userPermission = _httpContextAccessor?.HttpContext?.User.Claims.Where(p => p.Type == AuthorizationPermissionClaimTypes.Permit).Select(p =>
            {
                var functionPermission = p.Value.Split(",");
                if (!int.TryParse(functionPermission[0], out var functionValue)) throw new ArgumentException("Invalid Permission claims");
                if (!int.TryParse(functionPermission[1], out var permissionValue)) throw new ArgumentException("Invalid Permission claims");
                var function = _permissionProvider.GetDefinitions().FirstOrDefault(p => p.Value == functionValue) ?? throw new ArgumentException("Function in claims is not in function list");
                return new Permit
                {
                    Value = function.Value,
                    Name = function.Name,
                    PermissionsData = function.PermissionsData.Where(p => (p.Value & permissionValue) > 0).ToList()
                };
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPermission;
        }

        /// <inheritdoc/>
        public bool Validate(IEnumerable<Permit> permits, Permit permissionRequired)
        {
            if (permits.Any(p => p.Value == permissionRequired.Value && (p.Permissions & permissionRequired.Permissions) > 0))
            {
                return true;
            }
            return false;
        }
    }
}
