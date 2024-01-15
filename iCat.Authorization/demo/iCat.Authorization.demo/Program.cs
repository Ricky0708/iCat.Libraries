using iCat.Authorization.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using iCat.Authorization.demo.Enums;
namespace iCat.Authorization.demo
{
    public class Program
    {
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
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
