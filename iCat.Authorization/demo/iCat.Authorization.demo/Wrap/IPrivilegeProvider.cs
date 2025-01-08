using iCat.Authorization.demo.Enums;
using iCat.Authorization.Models;
using iCat.Authorization.Providers.Interfaces;
using iCat.Authorization.Web.Providers.Implements;
using iCat.Authorization.Web.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.demo.Wrap
{
    public interface IPrivilegeProvider : IPrivilegeProvider<PrivilegeEnum>
    {

    }

    public class PrivilegeProvider : PrivilegeProvider<PrivilegeEnum>, IPrivilegeProvider
    {
        public PrivilegeProvider(IHttpContextAccessor httpContextAccessor, IClaimProcessor<PrivilegeEnum> claimProcessor, IPrivilegeProcessor<PrivilegeEnum> permissionProcessor) : base(httpContextAccessor, claimProcessor, permissionProcessor)
        {
        }
    }



}
