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
    public class ClaimProcessor : IClaimProcessor
    {
        /// <summary>
        /// permissionProvider
        /// </summary>
        private readonly IPermissionProcessor _permissionProvider;

        /// <inheritdoc/>
        public ClaimProcessor(IPermissionProcessor permissionProvider)
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
            var permit = _permissionProvider.GetPermitDefinitionFromPermission(permission);
            var claim = GeneratePermitClaim(permit.Value!.Value, (int)(object)permission);
            return claim;
        }

        /// <inheritdoc/>
        public Claim GeneratePermitClaim(int permit, int permission)
        {
            var claim = new Claim(Constants.ClaimTypes.Permit, $"{permit},{permission}");
            return claim;
        }


    }
}
