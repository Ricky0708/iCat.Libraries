using iCat.Authorization.demo.Enums;
using iCat.Authorization.Web.Providers.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace iCat.Authorization.demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermitProvider _permitProvider;

        public LoginController(
            IHttpContextAccessor httpContextAccessor,
            IPermitProvider permitProvider)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _permitProvider = permitProvider ?? throw new ArgumentNullException(nameof(permitProvider));
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult CookieLogin()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim("UserId", "TestId"),
                _permitProvider.GenerateClaim(UserProfilePermission.Add | UserProfilePermission.ReadAllDetail),
                _permitProvider.GenerateClaim(DepartmentPermission.Delete),
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            _httpContextAccessor.HttpContext?.SignInAsync(principal);
            return Ok();
        }

    }
}
