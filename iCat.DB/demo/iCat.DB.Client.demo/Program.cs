using iCat.DB.Client.Implements;
using iCat.DB.Client.Models;
using Microsoft.Data.SqlClient;
using iCat.DB.Client.Factory.Extensions;
namespace iCat.DB.Client.demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;
            services
                .AddDBFactory(
                    () => new DBClient(new DBClientInfo("MainDB", new SqlConnection("server=192.168.1.3\\SQL2019;user id=sa;password= P@ssw0rd;initial catalog=iNoty;TrustServerCertificate=true"))),
                    () => new DBClient(new DBClientInfo("Company", new SqlConnection("server=192.168.1.3\\SQL2019;user id=sa;password= P@ssw0rd;initial catalog=iNoty;TrustServerCertificate=true")))
                );
            services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.MapControllers();

            app.Run();
        }
    }
}
