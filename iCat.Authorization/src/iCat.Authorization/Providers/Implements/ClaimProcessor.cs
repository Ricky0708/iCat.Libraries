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
        public Claim GeneratePrivilegeClaim<T>(IPrivilege<T> privilege) where T : IPermission
        {
            var claim = GeneratePrivilegeClaim(privilege.Value!.Value, privilege.Permissions);
            return claim;
        }

        /// <inheritdoc/>
        public Claim GeneratePrivilegeClaim<TPermission>(TPermission permission) where TPermission : Enum
        {
            var privilege = _permissionProvider.GetPrivilegeDefinitionFromPermission(permission);
            var claim = GeneratePrivilegeClaim(privilege.Value!.Value, (int)(object)permission);
            return claim;
        }

        /// <inheritdoc/>
        internal Claim GeneratePrivilegeClaim(int privilege, int permission)
        {
            var claim = new Claim(Constants.ClaimTypes.Privilege, $"{privilege},{permission}");
            return claim;
        }


    }
}
