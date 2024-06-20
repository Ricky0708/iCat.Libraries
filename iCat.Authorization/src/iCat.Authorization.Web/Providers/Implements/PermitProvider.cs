using iCat.Authorization.Models;
using iCat.Authorization.Providers.Implements;
using iCat.Authorization.Providers.Interfaces;
using iCat.Authorization.Web.Providers.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
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
        public IEnumerable<Permit> GetPermits()
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

    }
}
