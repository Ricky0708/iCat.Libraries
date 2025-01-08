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
    public class ClaimProcessor<T> : IClaimProcessor<T> where T : Enum
    {
        /// <summary>
        /// permissionProvider
        /// </summary>
        private readonly IPrivilegeProcessor<T> _permissionProvider;

        /// <inheritdoc/>
        public ClaimProcessor(IPrivilegeProcessor<T> permissionProvider)
        {
            _permissionProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
        }

        /// <inheritdoc/>
        public Claim GeneratePrivilegeClaim(Privilege<T> privilege)
        {
            var claim = GeneratePrivilegeClaim((int)(object)privilege.Value!, privilege.Permissions);
            return claim;
        }

        /// <inheritdoc/>
        public Claim GeneratePrivilegeClaim<TPermission>(TPermission permission) where TPermission : Enum
        {
            var privilege = _permissionProvider.GetPrivilegeDefinitionFromPermission(permission);
            var claim = GeneratePrivilegeClaim((int)(object)privilege.Value!, (int)(object)permission);
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
