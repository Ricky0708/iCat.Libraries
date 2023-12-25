using iCat.Cache.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace iCat.Cache.Implements
{
    public class ForCache : IForCache
    {
        private readonly IDistributedCache _cache;

        public ForCache(IDistributedCache distributedCache)
        {
            _cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public T? Get<T>(string key)
        {
            var cacheValue = _cache.GetString(key);
            if (!string.IsNullOrEmpty(cacheValue))
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(cacheValue);
            }
            return default;

            //var size = Marshal.SizeOf(value);
            //var bytes = new byte[size];
            //var ptr = Marshal.AllocHGlobal(size);
            //Marshal.Copy(bytes, 0, ptr, size);
            //p = (Person)Marshal.PtrToStructure(ptr, typeof(Person));
            //Marshal.FreeHGlobal(ptr);
        }

        public string? GetString(string key)
        {
            var cacheValue = _cache.GetString(key);
            return cacheValue;
        }

        public byte[]? GetBytes(string key)
        {
            var cacheValue = _cache.Get(key);
            return cacheValue;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cacheValue = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cacheValue))
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(cacheValue);
            }
            return default;
        }

        public async Task<string?> GetStringAsync(string key)
        {
            var cacheValue = await _cache.GetStringAsync(key);
            return cacheValue;
        }

        public async Task<byte[]?> GetBytesAsync(string key)
        {
            var cacheValue = await _cache.GetAsync(key);
            return cacheValue;
        }

        public void Set<T>(string key, T data)
        {
            if (data != null)
            {
                var value = System.Text.Json.JsonSerializer.Serialize(data);
                _cache.SetString(key, value);
            }
        }

        public void Set<T>(string key, T data, DistributedCacheEntryOptions options)
        {
            if (data != null)
            {
                var value = System.Text.Json.JsonSerializer.Serialize(data);
                _cache.SetString(key, value, options);
            }
        }

        public void SetString(string key, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                _cache.SetString(key, data);
            }
        }

        public void SetString(string key, string data, DistributedCacheEntryOptions options)
        {
            if (!string.IsNullOrEmpty(data))
            {
                _cache.SetString(key, data, options);
            }
        }

        public async Task SetAsync<T>(string key, T data)
        {
            if (data != null)
            {
                var value = System.Text.Json.JsonSerializer.Serialize(data);
                await _cache.SetStringAsync(key, value);
            }
        }

        public async Task SetAsync<T>(string key, T data, DistributedCacheEntryOptions options)
        {
            if (data != null)
            {
                var value = System.Text.Json.JsonSerializer.Serialize(data);
                await _cache.SetStringAsync(key, value, options);
            }
        }

        public async Task SetStringAsync(string key, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                await _cache.SetStringAsync(key, data);
            }
        }

        public async Task SetStringAsync(string key, string data, DistributedCacheEntryOptions options)
        {
            if (!string.IsNullOrEmpty(data))
            {
                await _cache.SetStringAsync(key, data, options);
            }
        }

        public void Refresh(string key)
        {
            _cache.Refresh(key);
        }

        public async Task RefreshAsync(string key)
        {
            await _cache.RefreshAsync(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

    }
}
