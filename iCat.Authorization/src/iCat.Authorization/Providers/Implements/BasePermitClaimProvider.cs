using iCat.Authorization.Models;
using iCat.Authorization.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Providers.Implements
{
    /// <inheritdoc/>
    public abstract class BasePermitClaimProvider : IPermitClaimProcessor
    {
        /// <summary>
        /// permissionProvider
        /// </summary>
        private readonly IPermissionProvider _permissionProvider;

        /// <inheritdoc/>
        public BasePermitClaimProvider(IPermissionProvider permissionProvider)
        {
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
        public Claim GeneratePermitClaim(int permit, int permission)
        {
            var claim = new Claim(Constants.ClaimTypes.Permit, $"{permit},{permission}");
            return claim;
        }

        /// <inheritdoc/>
        public abstract IEnumerable<Permit> GetPermits();

        /// <inheritdoc/>
        public Permit ExtractPermit(int permitValue, int permissionsValue)
        {
            var permit = _permissionProvider.GetDefinitions().FirstOrDefault(x => x.Value == permitValue) ?? throw new ArgumentException("permit in claims is not in permit list");
            return new Permit
            {
                Value = permitValue,
                Name = permit.Name,
                PermissionsData = permit.PermissionsData.Where(x => (x.Value & permissionsValue) > 0).ToList()
            };
        }
    }
}
