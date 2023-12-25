using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.Cache.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Microsoft.Extensions.Caching.Distributed;
using iCat.CacheTests.Models;
using System.Data;
using NSubstitute.ReturnsExtensions;
using NuGet.Frameworks;

namespace iCat.Cache.Implements.Tests
{
    [TestClass()]
    public class CacheTests
    {
        [TestMethod()]
        public void Get_Success_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Get(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(json);
            //distributedCache.Set(Arg.Do<string>(
            //    p => Assert.AreEqual(p, "AAA")), 
            //    Arg.Any<byte[]>(), 
            //    Arg.Any<DistributedCacheEntryOptions>());


            var cache = new Cache(distributedCache);

            // action
            var result = cache.Get<TestModel>("AAA");

            // assert
            distributedCache.Received(1).Get(Arg.Any<string>());
            Assert.AreEqual(data.Name, result!.Name);
        }

        [TestMethod()]
        public void Get_Fail_Null_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Get(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(p => null);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.Get<TestModel>("AAA");

            // assert
            distributedCache.Received(1).Get(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void Get_Fail_Empty_Test()
        {
            // arrange
            var emptyValue = Encoding.UTF8.GetBytes("");
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Get(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(emptyValue);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.Get<TestModel>("AAA");

            // assert
            distributedCache.Received(1).Get(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void GetString_Success_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var jsonByte = Encoding.UTF8.GetBytes(json);
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Get(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(jsonByte);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetString("AAA");

            // assert
            distributedCache.Received(1).Get(Arg.Any<string>());
            Assert.AreEqual(json, result);
        }

        [TestMethod()]
        public void GetString_Faili_Null_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Get(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(p => null);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetString("AAA");

            // assert
            distributedCache.Received(1).Get(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void GetString_Faili_Empty_Test()
        {
            // arrange
            var json = "";
            var jsonByte = Encoding.UTF8.GetBytes(json);
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Get(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(jsonByte);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetString("AAA");

            // assert
            distributedCache.Received(1).Get(Arg.Any<string>());
            Assert.AreEqual(json, result);
        }

        [TestMethod()]
        public void GetBytes_Success_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var jsonBytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Get(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(jsonBytes);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetBytes("AAA");

            // assert
            distributedCache.Received(1).Get(Arg.Any<string>());
            Assert.AreEqual(jsonBytes, result);
        }

        [TestMethod()]
        public void GetBytes_Fail_Null_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Get(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(p => null);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetBytes("AAA");

            // assert
            distributedCache.Received(1).Get(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void GetBytes_Fail_Empty_Test()
        {
            // arrange
            var jsonBytes = Encoding.UTF8.GetBytes("");
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Get(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(jsonBytes);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetBytes("AAA");

            // assert
            distributedCache.Received(1).Get(Arg.Any<string>());
            Assert.AreEqual(jsonBytes, result);
        }

        [TestMethod()]
        public void GetAsync_Success_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).ReturnsNull();

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetAsync<TestModel>("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void GetAsync_Fail_Null_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(json);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetAsync<TestModel>("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(data.Name, result!.Name);
        }

        [TestMethod()]
        public void GetAsync_Fail_Empty_Test()
        {
            // arrange
            var json = Encoding.UTF8.GetBytes("");
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(json);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetAsync<TestModel>("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void GetStringAsync_Success_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var jsonByte = Encoding.UTF8.GetBytes(json);
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(jsonByte);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetStringAsync("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(json, result);
        }

        [TestMethod()]
        public void GetStringAsync_Fail_Null_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).ReturnsNull();

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetStringAsync("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void GetStringAsync_Fail_Empty_Test()
        {
            // arrange
            var json = Encoding.UTF8.GetBytes("");
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(json);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetStringAsync("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual("", result);
        }

        [TestMethod()]
        public void GetBytesAsync_Success_Test()
        {

            // arrange
            var data = new TestModel { Name = "Ricky" };
            var jsonBytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data));
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(jsonBytes);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetBytesAsync("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(jsonBytes, result);
        }

        [TestMethod()]
        public void GetBytesAsync_Fail_Null_Test()
        {

            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).ReturnsNull();

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetBytesAsync("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void GetBytesAsync_Fail_Empty_Test()
        {

            // arrange
            var json = "";
            var jsonByte = Encoding.UTF8.GetBytes(json);
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.GetAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA"))).Returns(jsonByte);

            var cache = new Cache(distributedCache);

            // action
            var result = cache.GetBytesAsync("AAA").Result;

            // assert
            distributedCache.Received(1).GetAsync(Arg.Any<string>());
            Assert.AreEqual(jsonByte, result);
        }

        [TestMethod()]
        public void Set_Success_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Set(
                Arg.Do<string>(p => Assert.AreEqual(p, "AAA")),
                Arg.Do<byte[]>(p => Assert.AreEqual(Encoding.UTF8.GetString(p), json)),
                Arg.Any<DistributedCacheEntryOptions>());


            var cache = new Cache(distributedCache);

            // action
            cache.Set("AAA", data);

            // assert
            distributedCache.Received(1).Set(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }

        [TestMethod()]
        public void Set_Fail_Null_Test()
        {
            // arrange
            var data = default(TestModel);
            var distributedCache = Substitute.For<IDistributedCache>();

            var cache = new Cache(distributedCache);

            // action
            cache.Set("AAA", data);

            // assert
            distributedCache.Received(0).Set(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }

        [TestMethod()]
        public void SetString_Success_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Set(
                Arg.Do<string>(p => Assert.AreEqual(p, "AAA")),
                Arg.Do<byte[]>(p => Assert.AreEqual(Encoding.UTF8.GetString(p), json)),
                Arg.Any<DistributedCacheEntryOptions>());


            var cache = new Cache(distributedCache);

            // action
            cache.SetString("AAA", json);

            // assert
            distributedCache.Received(1).Set(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }

        [TestMethod()]
        public void SetString_Fail_Null_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();


            var cache = new Cache(distributedCache);

            // action
            cache.SetString("AAA", "");

            // assert
            distributedCache.Received(0).Set(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }

        [TestMethod()]
        public void SetAsync_Success_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.SetAsync(
                Arg.Do<string>(p => Assert.AreEqual(p, "AAA")),
                Arg.Do<byte[]>(p => Assert.AreEqual(Encoding.UTF8.GetString(p), json)),
                Arg.Any<DistributedCacheEntryOptions>());


            var cache = new Cache(distributedCache);

            // action
            cache.SetAsync("AAA", data).Wait();

            // assert
            distributedCache.Received(1).SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }

        [TestMethod()]
        public void SetAsync_Fail_Null_Test()
        {
            // arrange
            var data = default(TestModel);
            var distributedCache = Substitute.For<IDistributedCache>();

            var cache = new Cache(distributedCache);

            // action
            cache.SetAsync("AAA", data).Wait();

            // assert
            distributedCache.Received(0).SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }

        [TestMethod()]
        public void SetStringAsync_Success_Test()
        {
            // arrange
            var data = new TestModel { Name = "Ricky" };
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.SetAsync(
                Arg.Do<string>(p => Assert.AreEqual(p, "AAA")),
                Arg.Do<byte[]>(p => Assert.AreEqual(Encoding.UTF8.GetString(p), json)),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());


            var cache = new Cache(distributedCache);

            // action
            cache.SetStringAsync("AAA", json).Wait();

            // assert
            distributedCache.Received(1).SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }

        [TestMethod()]
        public void SetStringAsync_Fail_Null_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();


            var cache = new Cache(distributedCache);

            // action
            cache.SetStringAsync("AAA", "").Wait();

            // assert
            distributedCache.Received(0).SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>());
        }

        [TestMethod()]
        public void Refresh_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Refresh(Arg.Do<string>(p => Assert.AreEqual(p, "AAA")));


            var cache = new Cache(distributedCache);

            // action
            cache.Refresh("AAA");

            // assert
            distributedCache.Received(1).Refresh(Arg.Any<string>());
        }

        [TestMethod()]
        public void RefreshAsync_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.RefreshAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA")));


            var cache = new Cache(distributedCache);

            // action
            cache.RefreshAsync("AAA").Wait();

            // assert
            distributedCache.Received(1).RefreshAsync(Arg.Any<string>());
        }

        [TestMethod()]
        public void Remove_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.Remove(Arg.Do<string>(p => Assert.AreEqual(p, "AAA")));


            var cache = new Cache(distributedCache);

            // action
            cache.Remove("AAA");

            // assert
            distributedCache.Received(1).Remove(Arg.Any<string>());
        }

        [TestMethod()]
        public void RemoveAsync_Test()
        {
            // arrange
            var distributedCache = Substitute.For<IDistributedCache>();
            distributedCache.RemoveAsync(Arg.Do<string>(p => Assert.AreEqual(p, "AAA")));


            var cache = new Cache(distributedCache);

            // action
            cache.RemoveAsync("AAA").Wait();

            // assert
            distributedCache.Received(1).RemoveAsync(Arg.Any<string>());
        }
    }
}