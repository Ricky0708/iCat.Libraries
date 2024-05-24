using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.Cache.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.CacheTests.Models;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using StackExchange.Redis;

namespace iCat.Cache.Implements.Tests
{
    [TestClass()]
    public class RedisCacheImplTests
    {
        [TestMethod()]
        public void GetAsyncTest()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(json);

            var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();


            var cache = new RedisCacheImpl(distributedCache, connectionMultiplexer);

            // action
            var result = cache.GetAsync<TestModel>("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(data.Name, result!.Name);
        }


        [TestMethod()]
        public void GetAsyncTest_Fail_Null_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(default(byte[]));

            var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();


            var cache = new RedisCacheImpl(distributedCache, connectionMultiplexer);

            // action
            var result = cache.GetAsync<TestModel>("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void Get_Fail_Empty_Test()
        {
            // arrange
            var emptyValue = Encoding.UTF8.GetBytes("");
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(emptyValue);

            var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();


            var cache = new RedisCacheImpl(distributedCache, connectionMultiplexer);

            // action
            var result = cache.GetAsync<TestModel>("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }


        [TestMethod()]
        public void GetStringAsyncTest()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var jsonByte = Encoding.UTF8.GetBytes(json);
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(jsonByte);

            var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();

            var cache = new RedisCacheImpl(distributedCache, connectionMultiplexer);

            // action
            var result = cache.GetStringAsync("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(json, result);
        }
    }
}