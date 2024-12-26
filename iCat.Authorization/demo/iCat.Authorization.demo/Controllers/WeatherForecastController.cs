using iCat.Authorization.demo.Enums;
using iCat.Authorization.demo.Wrap;
using iCat.Authorization.Providers.Interfaces;
using iCat.Authorization.Web;
using iCat.Authorization.Web.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace iCat.Authorization.demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IPrivilegeProvider _privilegeProvider;
        private readonly IPrivilegeProcessor _permissionProcessor;

        public WeatherForecastController(
            IPrivilegeProvider permissionProvider,
            IPrivilegeProcessor permissionProcessor)
        {
            _privilegeProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
            _permissionProcessor = permissionProcessor ?? throw new ArgumentNullException(nameof(permissionProcessor));
        }

        [PermissionsAuthorization(
            DepartmentPermission.Read | DepartmentPermission.Delete,
            UserProfilePermission.Add | UserProfilePermission.Edit | UserProfilePermission.ReadPartialDetail)]
        [HttpGet]
        public IActionResult GetData()
        {
            return Ok(_privilegeProvider.GetCurrentUserPrivileges());
        }
    }
}
