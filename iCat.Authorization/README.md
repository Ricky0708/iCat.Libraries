# iCat.Authorization.Web

iCat.Authorization.Web is integrated to the `Policy-based authorization`.<br>
It customs `IAuthorizationRequirement`, `AuthorizationHandler` and provide provider for processing authorization-related data.

## Installation
```bash
dotnet add package iCat.Authorization.Web
```

## Configuration

### Define privileges and permissions enums mapping

The defination of privileges and permissions need to follow these rules.

1. Permission needs to use bit wises value and set `Flags` attribute on the class.
2. Use the `Permission` attribute to specify the permissions of the privilege .

```C#
    using iCat.Authorization;
```

```C#
    public enum PrivilegeEnum
    {
        [Permission(typeof(UserProfilePermission))]
        UserProfile = 1,
        [Permission(typeof(OrderPermission))]
        Order = 2,
        [Permission(typeof(DepartmentPermission))]
        Department = 3
    }

    [Flags]
    public enum UserProfilePermission
    {
        Add = 1,
        Edit = 2,
        ReadPartialDetail = 4,
        Delete = 8,
        ReadAllDetail = 16,
    }

    [Flags]
    public enum OrderPermission
    {
        Add = 1,
        Read = 2,
        Edit = 4,
        Delete = 8
    }

    [Flags]
    public enum DepartmentPermission
    {
        Add = 1,
        Edit = 2,
        Read = 4,
        Delete = 8
    }
```

### Configure Requirment and Handler

Register providers and privileges/permissions using the `.AddWebAuthorizationPermission(typeof(PrivilegeEnum))` method, add a requirment to the policies via `.AddAuthorizationPermissionRequirment()`.<br>
iCat.Authorization.Web needs to use `IHttpContextAccessor` to obtain the current requested privileges/permissions.

```C#
    using iCat.Authorization.Web.Extensions;
```

```C#
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        // Add services to the container.
        builder.Services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddAuthorizationPermission(typeof(PrivilegeEnum))
            .AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, "Bearer")
                    .AddAuthorizationPermissionRequirment()
                    .RequireAuthenticatedUser()
                    .Build();

            });

        ...

        app.Run();
    }
```

### AuthorizationPermission on action

Set the permission for the action through the `AuthorizationPermissions` attribute.

```C#
    using iCat.Authorization.Web;
```

```C#
    ...

    [AuthorizationPermissions(
        DepartmentPermission.Read | DepartmentPermission.Delete,
        UserProfilePermission.Add | UserProfilePermission.Edit | UserProfilePermission.Read)]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetData()
    {
        ...
    }
```

### Obtain current user privileges, claims

The `IPrivilegeProvider` provides methods to obtain the logged user's claim from the `Privilege`. <br>


```C#
    using iCat.Authorization.Web;
```


```C#
   [ApiController]
   [Route("[controller]")]
   public class TestController : ControllerBase
   {
        private readonly IPrivilegeProvider _privilegeProvider;

       public TestController(IPrivilegeProvider privilegeProvider, IPermissionProvider permissionProvider)
       {
            _privilegeProvider = privilegeProvider ?? throw new ArgumentNullException(nameof(privilegeProvider));
       }
       
       [AuthorizationPermissions(
            DepartmentPermission.Read | DepartmentPermission.Delete,
            UserProfilePermission.Add | UserProfilePermission.Edit | UserProfilePermission.ReadPartialDetail)]
       [HttpGet("[action]")]
       public IActionResult GetData()
       {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim("UserId", "TestId"),
                _privilegeProvider.GenerateClaim(UserProfilePermission.Add | UserProfilePermission.ReadAllDetail),
                _privilegeProvider.GenerateClaim(DepartmentPermission.Delete),
            };

            var userPrivileges = _privilegeProvider.GetCurrentUserPrivileges();
            return Ok(userPrivileges);
       }
   }
```