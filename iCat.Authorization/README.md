# iCat.Cache

iCat.Cache is an adapter for IDistributedCache, which makes the use of IDistributedCache more convenient.

Refer [Microsoft's DistributedCache documentation](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-8.0) for IDistrubuteCache

## Installation
```bash
dotnet add package iCat.Cache
```

## Configuration

```C#
    using iCat.Cache.Implements;
    using iCat.Cache.Interfaces;
```
```C#
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDistributedMemoryCache(); // inject memory cache into IDistributedCache
            builder.Services.AddSingleton<ICache, iCat.Cache.Implements.Cache>(); // IDistributedCache adapter

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
```
---
### Sample

```C# Using
    using iCat.Cache.Interfaces;
    using Microsoft.Extensions.Caching.Distributed;
```

```C#
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ICache _cache;

        public DemoController(ICache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [HttpGet]
        public IActionResult Get()
        {
            // set cache
            _cache.Set("key", new { Name = "name" }, new DistributedCacheEntryOptions { SlidingExpiration = new TimeSpan(3000) });

            // get cache
            var result = _cache.GetString("key");

            return Ok(result);
        }
    }
```