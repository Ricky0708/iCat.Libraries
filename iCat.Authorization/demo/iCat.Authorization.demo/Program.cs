using iCat.Authorization.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using iCat.Authorization.demo.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using iCat.Authorization.Models;
namespace iCat.Authorization.demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // Add services to the container.
            ConfigureAuthorization(services);

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static IServiceCollection ConfigureAuthorization(IServiceCollection services)
        {
            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddAuthorizationPermission(typeof(PermitEnum))
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddAuthorizationPermissionRequirment()
                        .RequireAuthenticatedUser()
                        .Build();

                })
                .AddAuthentication()
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Events.OnRedirectToAccessDenied = (s) =>
                    {
                        s.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToLogin = (s) =>
                    {
                        s.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                });
            return services;
        }
    }
}
