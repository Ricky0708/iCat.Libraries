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
    public class DefaultPermitClaimProcessor : IPermitClaimProcessor
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IPermissionProvider _permissionProvider;

        /// <inheritdoc/>
        public DefaultPermitClaimProcessor(IHttpContextAccessor? httpContextAccessor, IPermissionProvider permissionProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _permissionProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
        }

        /// <inheritdoc/>
        public Claim GeneratePermitClaim<T>(IPermit<T> permission) where T : IPermission
        {
            var claim = GeneratePermitClaim(permission.Value!.Value, permission.Permissions);
            return claim;
        }

        /// <inheritdoc/>
        public Claim GeneratePermitClaim<TPermission>(TPermission permission) where TPermission : Enum
        {
            var permit = _permissionProvider.GetPermitFromPermission(permission);
            var claim = GeneratePermitClaim(permit.Value!.Value, (int)(object)permission);
            return claim;
        }

        /// <inheritdoc/>
        public virtual Claim GeneratePermitClaim(int permit, int permission)
        {
            var claim = new Claim(Constants.ClaimTypes.Permit, $"{permit},{permission}");
            return claim;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<Permit> GetPermits()
        {
            var userPermits = _httpContextAccessor?.HttpContext?.User.Claims.Where(p => p.Type == Constants.ClaimTypes.Permit).Select(p =>
            {
                var permission = p.Value.Split(",");
                if (!int.TryParse(permission[0], out var permitValue)) throw new ArgumentException("Invalid Permit claims");
                if (!int.TryParse(permission[1], out var permissionValue)) throw new ArgumentException("Invalid Permit claims");
                var permit = _permissionProvider.GetDefinitions().FirstOrDefault(x => x.Value == permitValue) ?? throw new ArgumentException("permit in claims is not in permit list");
                return new Permit
                {
                    Value = permit.Value,
                    Name = permit.Name,
                    PermissionsData = permit.PermissionsData.Where(x => (x.Value & permissionValue) > 0).ToList()
                };
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPermits;
        }
    }
}
