﻿using iCat.Authorization.demo.Enums;
using iCat.Authorization.demo.Models;
using iCat.Authorization.demo.Wrap;
using iCat.Authorization.Models;
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
        private readonly IPrivilegeProvider<PrivilegeEnum> _privilegeProvider;
        private readonly CurrentUserData _currentUserData;

        public LoginController(
            IHttpContextAccessor httpContextAccessor,
            IPrivilegeProvider<PrivilegeEnum> privilegeProvider,
            CurrentUserData currentUserData)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _privilegeProvider = privilegeProvider ?? throw new ArgumentNullException(nameof(privilegeProvider));
            _currentUserData = currentUserData;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult CookieLogin()
        {
            var privilegeFromPermission = _privilegeProvider.BuildPrivilege(UserProfilePermission.Add | UserProfilePermission.Delete);
            var privilegeFromDB = _privilegeProvider.BuildPrivilege((int)PrivilegeEnum.Department, (int)(DepartmentPermission.Delete | DepartmentPermission.Edit));
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim("UserId", "TestId"),
                _privilegeProvider.GenerateClaim(privilegeFromPermission),
                _privilegeProvider.GenerateClaim(privilegeFromDB),
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            _httpContextAccessor.HttpContext?.SignInAsync(principal);
            return Ok(_currentUserData);
        }

    }
}
