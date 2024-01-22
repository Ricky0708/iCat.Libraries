# iCat.Authorization

iCat.Authorization is integrated to the `Policy-based authorization`.<br>
It customs `IAuthorizationRequirement`, `AuthorizationHandler` and provide provider for processing authorization-related data.

## Installation
```bash
dotnet add package iCat.Authorization
```

## Configuration

### Define permits and permissions enums mapping

The defination of permits and permissions need to follow these rules.

1. Permission needs to use bit wises value and set `Flags` attribute on the class.
2. Use the `Permission` attribute to specify the permit's permission.

```C#
    using iCat.Authorization;
```

```C#
    public enum PermitEnum
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

Register providers and permits/permissions using the `.AddAuthorizationPermission(typeof(Permit))` method, add a requirment to the policies via `.AddAuthorizationPermissionRequirment()`.<br>
iCat.Authorization needs to use `IHttpContextAccessor` to obtain the current requested permits/permissions.

```C#
    using iCat.Authorization.Extensions;
```

```C#
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        // Add services to the container.
        builder.Services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddAuthorizationPermission(typeof(PermitEnum))
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

Set the permission for the action through the `AuthorizationPermission` attribute.

```C#
    using iCat.Authorization;
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

### Obtain current user permits, claims

The `IPermitClaimProcessor` provides a method to obtain the logged in user's claim from the `Permit`. <br>
The `IPermissionProvider` provides all permits definition.


```C#
    using iCat.Authorization;
    using iCat.Authorization.Utilities;
```


```C#
   [ApiController]
   [Route("[controller]")]
   public class TestController : ControllerBase
   {
        private readonly IPermitClaimProcessor _permitClaimProcessor;
        private readonly IPermissionProvider _permissionProvider;

       public TestController(IPermitClaimProcessor permitClaimeProcessor, IPermissionProvider permissionProvider)
       {
            _permitClaimProcessor = permitClaimeProcessor ?? throw new ArgumentNullException(nameof(permitProvider));
            _permissionProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
       }
       
       [AuthorizationPermissions(
            DepartmentPermission.Read | DepartmentPermission.Delete,
            UserProfilePermission.Add | UserProfilePermission.Edit | UserProfilePermission.ReadPartialDetail)]
       [HttpGet("[action]")]
       public IActionResult GetData()
       {
            var claim = _permitClaimProcessor.GeneratePermitClaim(new Permit
            {
                Value = (int)PermitEnum.UserProfile,
                PermissionsData = new List<Permission> { new Permission
                {
                    Value = (int)UserProfilePermission.Add,
                }}
            });

            var permits = _permissionProvider.GetDefinitions();
            var userPermits = _permitClaimProcessor.GetPermit();
            return Ok(userPermits);
       }
   }
```