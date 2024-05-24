using iCat.Cache.Implements;
using iCat.Cache.Interfaces;

namespace iCat.Cache.demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDistributedMemoryCache(); // inject memory cache into IDistributedCache
            builder.Services.AddSingleton<ICacheBackup, iCat.Cache.Implements.Cache>(); // IDistributedCache adapter

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
