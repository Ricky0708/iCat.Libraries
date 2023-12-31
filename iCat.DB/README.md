# iCat.DB.CLient

iCat.DB.Client is a UnitOfWork design pattern library for db connection.
It can manages and serves dynamic provide db clients at runtime.

## Description

The library provides two way for registering IUnitOfWork and IConnection.
IUnitOfWork and IConnection export the DBConnection property, which can also used in dapper.net.

1. General fixed connection registration, registered when the application started, can't be modified.
2. Through factory registration, programers can implement "IConnectionProvider" to provide IUnitOfWork/IConnection,

As a reminder, the IUnitOfWork/IConnection obtained from "General Fixed Connection Registration" and "Factory" are different instances.

## Installation
```bash
dotnet add package iCat.DB.Client
dotnet add package iCat.DB.Client.Extension.Web
dotnet add package iCat.DB.Client.Factory
dotnet add package iCat.DB.Client.MSSQL
dotnet add package iCat.DB.Client.MySQL
```

## Sample ( General Fixed Connection Registration )

### Program.cs
```C#
    using iCat.DB.Client.Extension.Web;
    using iCat.DB.Client.Interfaces;
    using iCat.DB.Client.Models;
    using MSSQL = iCat.DB.Client.MSSQL;
    using MySQL = iCat.DB.Client.MSSQL;
```
```C#
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;
            services
                .AddDBClient(
                    new MySQL.DBClient(new DBClientInfo("MainDB", "mysql connection string")),
                    new MSSQL.DBClient(new DBClientInfo("CompanyA", "mssql connection string A")),
                    new MSSQL.DBClient(new DBClientInfo("CompanyB", "mssql connection string B"))
                );

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();
            app.MapControllers();

            foreach (var iConnection in app.Services.GetServices<IConnection>())
            {
                iConnection.ExecutedEvent += (category, command, script) =>
                {
                    // category: MainDB, CompanyA, CompanyB..etc
                    // command: enum (Opened,  Closed, TransactionBegined, Commited, Rollbacked, Executed) 
                    // script: sql statements run by the application
                };
            }

            app.Run();
        }
    }
```
### Controller
```C#
    using iCat.DB.Client.Interfaces;
```
```C#
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly IEnumerable<IUnitOfWork> _unitOfWorks;
        private readonly IEnumerable<IConnection> _connections;

        public DemoController(IEnumerable<IUnitOfWork> unitOfWorks, IEnumerable<IConnection> connections)
        {
            _unitOfWorks = unitOfWorks ?? throw new ArgumentNullException(nameof(unitOfWorks));
            _connections = connections ?? throw new ArgumentNullException(nameof(connections));
        }

        [HttpGet]
        public IActionResult Get()
        {

            using (var unitOfork = _unitOfWorks.First(p => p.Category == "MainDB"))
            {
                try
                {
                    unitOfork.Open();
                    unitOfork.BeginTransaction();

                    var connection = _connections.First(p => p.Category == "MainDB");
                    foreach (var dr in connection.ExecuteReader("SELECT * FROM YourTable", new DbParameter[] { })){
                        // write in your logic
                    }

                    unitOfork.Commit();
                }
                catch (Exception)
                {
                    unitOfork.Rollback();
                }
                finally
                {
                    unitOfork.Close();
                }
            }

            return Ok();
        }
    }
```

## Sample ( Through factory registration )

### Program.cs
```C#
    using iCat.DB.Client.Extension.Web;
    using iCat.DB.Client.Interfaces;
    using iCat.DB.Client.Models;
    using iCat.DB.Client.Factory.Interfaces;
```
```C#
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
```
### Controller
```C#
    using iCat.DB.Client.Factory.Interfaces;
```
```C#
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly IDBClientFactory _clientFactory;

        public DemoController(IDBClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        [HttpGet]
        public IActionResult Get()
        {

            using (var unitOfork = _clientFactory.GetUnitOfWork("MainDB"))
            {
                try
                {
                    unitOfork.Open();
                    unitOfork.BeginTransaction();

                    var connection = _clientFactory.GetConnection("MainDB");
                    foreach (var dr in connection.ExecuteReader("SELECT * FROM YourTable", new DbParameter[] { }))
                    {
                        var filed = dr["fieldName"];
                    };

                    unitOfork.Commit();
                }
                catch (Exception)
                {
                    unitOfork.Rollback();
                }
                finally
                {
                    unitOfork.Close();
                }
            }

            return Ok();
        }
    }
```