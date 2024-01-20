using iCat.Authorization.demo.Enums;
using iCat.Authorization.Models;
using iCat.Authorization.Utilities;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iCat.Authorization.demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IPermitProvider _permitProvider;
        private readonly IPermissionProvider _permissionProvider;

        public WeatherForecastController(IPermitProvider permitProvider, IPermissionProvider permissionProvider)
        {
            _permitProvider = permitProvider ?? throw new ArgumentNullException(nameof(permitProvider));
            _permissionProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
        }

        [AuthorizationPermissions(
            DepartmentPermission.Read | DepartmentPermission.Delete,
            UserProfilePermission.Add | UserProfilePermission.Edit | UserProfilePermission.ReadPartialDetail)]
        [HttpGet]
        public IActionResult GetData()
        {
            return Ok(_permitProvider.GetPermit());
        }
    }
}
