using iCat.Authorization.demo.Enums;
using iCat.Authorization.Models;
using iCat.Authorization.Providers.Interfaces;
using iCat.Authorization.Web;
using iCat.Authorization.Web.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace iCat.Authorization.demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IPermitProvider _permitProvider;

        public WeatherForecastController(IClaimProcessor permitClaimProcessor, IPermitProvider permissionProvider)
        {
            _permitProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
        }

        [AuthorizationPermissions(
            DepartmentPermission.Read | DepartmentPermission.Delete,
            UserProfilePermission.Add | UserProfilePermission.Edit | UserProfilePermission.ReadPartialDetail)]
        [HttpGet]
        public IActionResult GetData()
        {
            return Ok(_permitProvider.GetPermits());
        }
    }
}
