using iCat.Authorization.demo.Enums;
using iCat.Authorization.Providers.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermitClaimProcessor _permitClaimProcessor;

        public LoginController(
            IHttpContextAccessor httpContextAccessor,
            IPermitClaimProcessor permitClaimGenerator)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _permitClaimProcessor = permitClaimGenerator ?? throw new ArgumentNullException(nameof(permitClaimGenerator));
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult CookieLogin()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim("UserId", "TestId"),
                _permitClaimProcessor.GeneratePermitClaim(UserProfilePermission.Add | UserProfilePermission.ReadAllDetail),
                _permitClaimProcessor.GeneratePermitClaim(DepartmentPermission.Delete),
            };


            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            _httpContextAccessor.HttpContext?.SignInAsync(principal);
            return Ok();
        }

    }
}
