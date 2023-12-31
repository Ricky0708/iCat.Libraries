using iCat.DB.Client.Extension.Web;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Interfaces;
using iCat.DB.Client.Models;
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
                .AddDBClientFactory( // DefaultConnectionProvider ( you can custom 
                    () => new MySQL.DBClient(new DBClientInfo("MainDB", "mysql connection string")),
                    () => new MSSQL.DBClient(new DBClientInfo("CompanyA", "mssql connection string A")),
                    () => new MSSQL.DBClient(new DBClientInfo("CompanyB", "mssql connection string B"))
                );


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();
            app.MapControllers();

            foreach (var factory in app.Services.GetServices<IDBClientFactory>())
            {
                foreach (var connection in factory.GetConnections())
                {
                    connection.ExecutedEvent += (category, command, script) =>
                    {
                        // category: MainDB, CompanyA, CompanyB..etc
                        // command: enum (Opened,  Closed, TransactionBegined, Commited, Rollbacked, Executed) 
                        // script: sql statements run by the application
                    };
                }

            }

            app.Run();
        }
    }
}
