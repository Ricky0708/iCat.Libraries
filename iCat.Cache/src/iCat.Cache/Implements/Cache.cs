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
        /// <inheritdoc/>
    public class Cache : ICacheBackup
    {
        private readonly IDistributedCache _cache;

        /// <inheritdoc/>
        public Cache(IDistributedCache distributedCache)
        {
            _cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public string? GetString(string key)
        {
            var cacheValue = _cache.GetString(key);
            return cacheValue;
        }

        /// <inheritdoc/>
        public byte[]? GetBytes(string key)
        {
            var cacheValue = _cache.Get(key);
            return cacheValue;
        }

        /// <inheritdoc/>
        public async Task<T?> GetAsync<T>(string key)
        {
            var cacheValue = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cacheValue))
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(cacheValue);
            }
            return default;
        }

        /// <inheritdoc/>
        public async Task<string?> GetStringAsync(string key)
        {
            var cacheValue = await _cache.GetStringAsync(key);
            return cacheValue;
        }

        /// <inheritdoc/>
        public async Task<byte[]?> GetBytesAsync(string key)
        {
            var cacheValue = await _cache.GetAsync(key);
            return cacheValue;
        }

        /// <inheritdoc/>
        public void Set<T>(string key, T data)
        {
            if (data != null)
            {
                var value = System.Text.Json.JsonSerializer.Serialize(data);
                _cache.SetString(key, value);
            }
        }

        /// <inheritdoc/>
        public void Set<T>(string key, T data, DistributedCacheEntryOptions options)
        {
            if (data != null)
            {
                var value = System.Text.Json.JsonSerializer.Serialize(data);
                _cache.SetString(key, value, options);
            }
        }

        /// <inheritdoc/>
        public void SetString(string key, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                _cache.SetString(key, data);
            }
        }

        /// <inheritdoc/>
        public void SetString(string key, string data, DistributedCacheEntryOptions options)
        {
            if (!string.IsNullOrEmpty(data))
            {
                _cache.SetString(key, data, options);
            }
        }

        /// <inheritdoc/>
        public async Task SetAsync<T>(string key, T data)
        {
            if (data != null)
            {
                var value = System.Text.Json.JsonSerializer.Serialize(data);
                await _cache.SetStringAsync(key, value);
            }
        }

        /// <inheritdoc/>
        public async Task SetAsync<T>(string key, T data, DistributedCacheEntryOptions options)
        {
            if (data != null)
            {
                var value = System.Text.Json.JsonSerializer.Serialize(data);
                await _cache.SetStringAsync(key, value, options);
            }
        }

        /// <inheritdoc/>
        public async Task SetStringAsync(string key, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                await _cache.SetStringAsync(key, data);
            }
        }

        /// <inheritdoc/>
        public async Task SetStringAsync(string key, string data, DistributedCacheEntryOptions options)
        {
            if (!string.IsNullOrEmpty(data))
            {
                await _cache.SetStringAsync(key, data, options);
            }
        }

        /// <inheritdoc/>
        public void Refresh(string key)
        {
            _cache.Refresh(key);
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(string key)
        {
            await _cache.RefreshAsync(key);
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

    }
}
