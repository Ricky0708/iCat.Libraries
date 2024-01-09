# iCat.Authorization

iCat.Authorization is integrated to the `Policy-based authorization`.<br>
It customs `IAuthorizationRequirement`, `AuthorizationHandler` and provide provider for processing authorization-related data.

## Installation
```bash
dotnet add package iCat.Authorization
```

## Configuration

### Define functions and permissions enums mapping

The defination of functions and permissions need to follow these rules.

1. Permission enum name must end with `Permission`.
2. Permission needs to use bit wises value and set `Flags` attribute on the class.
3. Use the `Permission` attribute to specify the function's permission.

```C#
    using iCat.Authorization;
```

```C#
    public enum MyFunction
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

Register providers and functions/permissions using the `.AddAuthorizationPermission(typeof(MyFunction))` method, add a requirment to the policies via `.AddAuthorizationPermissionRequirment()`.
iCat.Authorization needs to use `IHttpContextAccessor` to obtain the current requested functions/permissions.

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
            .AddAuthorizationPermission(typeof(MyFunction))
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

### Obtain current user function/permissions, claims

The `IFunctionPermissionProvider` provides currently requested user functions/permissions, and provides a method to get claim for the logged in user from the `FunctionPermissionData`


```C#
    using iCat.Authorization;
    using iCat.Authorization.Utilities;
```


```C#
   [ApiController]
   [Route("[controller]")]
   public class TestController : ControllerBase
   {
       private readonly IFunctionPermissionProvider _userPermissionProvider;

       public TestController(IFunctionPermissionProvider userPermissionProvider)
       {
           _userPermissionProvider = userPermissionProvider ?? throw new ArgumentNullException(nameof(userPermissionProvider));
       }

       [AuthorizationPermissions(
           DepartmentPermission.Read | DepartmentPermission.Delete,
           UserProfilePermission.Add | UserProfilePermission.Edit | UserProfilePermission.Read)]
       [HttpGet("[action]")]
       public IActionResult GetData()
       {
            var claim = _userPermissionProvider.GetClaimFromFunctionPermissionData(new FunctionPermissionData
            {
                FunctionName = MyFunction.UserProfile.ToString(),
                FunctionValue = (int)MyFunction.UserProfile,
                PermissionDetails = new List<PermissionDetail> { new PermissionDetail
                {
                    PermissionName = UserProfilePermission.Add.ToString(),
                    Permission = (int)UserProfilePermission.Add,
                }}
            });

            var userPermissionData = _userPermissionProvider.GetUserPermission();
            return Ok(userPermissionData);
       }
   }
```