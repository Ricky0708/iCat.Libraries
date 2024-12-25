using iCat.Authorization.demo.Enums;
using iCat.Authorization.Providers.Implements;
using iCat.Authorization.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.demo.Wrap
{
    public interface IClaimProcessor : IClaimProcessor<PrivilegeEnum>
    {

    }

    public class ClaimProcessor : ClaimProcessor<PrivilegeEnum>, IClaimProcessor
    {
        public ClaimProcessor(IPrivilegeProcessor<PrivilegeEnum> permissionProvider) : base(permissionProvider)
        {
        }
    }
}
