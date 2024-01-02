# iCat.DB.CLient

iCat.DB.Client is a UnitOfWork design pattern library for db connection.
It can manages and serves dynamic provide db clients at runtime.

## Description

The library provides two way for registering IUnitOfWork and IConnection.
IUnitOfWork and IConnection export the DBConnection property, which can also used in dapper.net.

1. General fixed connection registration, registered when the application started, can't be modified.
2. Through factory registration, programers can implement "IDBClientProvider" to provide IUnitOfWork/IConnection,

As a reminder, the IUnitOfWork/IConnection obtained from "General Fixed Connection Registration" and "Factory" are different instances.

## Installation
```bash
dotnet add package iCat.DB.Client
dotnet add package iCat.DB.Client.Extension.Web
dotnet add package iCat.DB.Client.Factory
```

## Sample ( Single database )

### Program.cs
```C#
    using iCat.DB.Client.Extension.Web;
    using iCat.DB.Client.Implements;
    using iCat.DB.Client.Models;
```
```C#
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;
            services.AddDBClient((s) => new DBClient(new SqlConnection("Your connection string")));
            services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.MapControllers();


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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnection _connection;

        public DemoController(IUnitOfWork unitOfWork, IConnection connection)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        [HttpGet]
        public IActionResult Get()
        {

            using (_unitOfWork)
            {
                try
                {
                    _unitOfWork.Open();
                    _unitOfWork.BeginTransaction();

                    foreach (var dr in _connection.ExecuteReader("SELECT * FROM YourTable", new DbParameter[] { }))
                    {
                        var filed = dr["fieldName"];
                    };

                    _unitOfWork.Commit();
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                }
                finally
                {
                    _unitOfWork.Close();
                }
            }

            return Ok();
        }
    }
```

## Sample ( General Fixed Connections Registration )

### Program.cs
```C#
using iCat.DB.Client.Extension.Web;
using iCat.DB.Client.Implements;
using iCat.DB.Client.Models;
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
                .AddDBClients(
                    (s) => new DBClient(new DBClientInfo("MainDB", new SqlConnection("Your connection string"))),
                    (s) => new DBClient(new DBClientInfo("Company", new SqlConnection("Your connection string")))
                );
            services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.MapControllers();

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

            using (var unitOfWork = _unitOfWorks.First(p => p.Category == "MainDB"))
            {
                try
                {
                    unitOfWork.Open();
                    unitOfWork.BeginTransaction();
                    var connection = _connections.First(p => p.Category == "MainDB");

                    foreach (var dr in connection.ExecuteReader("SELECT * FROM YourTable", new DbParameter[] { }))
                    {
                        var filed = dr["fieldName"];
                    };

                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                }
                finally
                {
                    unitOfWork.Close();
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
    using iCat.DB.Client.Implements;
    using iCat.DB.Client.Models;
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
                .AddDBClientFactory(
                    () => new DBClient(new DBClientInfo("MainDB", new SqlConnection("Your connection string"))),
                    () => new DBClient(new DBClientInfo("Company", new SqlConnection("Your connection string")))
                );
            services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.MapControllers();

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
        private readonly IDBClientFactory _factory;

        public DemoController(IDBClientFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        [HttpGet]
        public IActionResult Get()
        {
            using (var unitOfWork = _factory.GetUnitOfWork("MainDB"))
            {
                try
                {
                    unitOfWork.Open();
                    unitOfWork.BeginTransaction();
                    var connection = _factory.GetConnection("MainDB");

                    foreach (var dr in connection.ExecuteReader("SELECT * FROM YourTable", new DbParameter[] { }))
                    {
                        var filed = dr["fieldName"];
                    };

                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                }
                finally
                {
                    unitOfWork.Close();
                }
            }

            return Ok();
        }
    }
```