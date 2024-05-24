using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.Cache.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace iCat.Cache.demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ICacheBackup _cache;

        public DemoController(ICacheBackup cache)
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
}
