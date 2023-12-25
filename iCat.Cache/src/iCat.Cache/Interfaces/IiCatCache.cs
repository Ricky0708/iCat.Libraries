using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Cache.Interfaces
{
    public interface IiCatCache
    {
        T? Get<T>(string key);
        Task<T?> GetAsync<T>(string key);
        byte[]? GetBytes(string key);
        Task<byte[]?> GetBytesAsync(string key);
        string? GetString(string key);
        Task<string?> GetStringAsync(string key);
        void Refresh(string key);
        Task RefreshAsync(string key);
        void Remove(string key);
        Task RemoveAsync(string key);
        void Set<T>(string key, T data);
        void Set<T>(string key, T data, DistributedCacheEntryOptions options);
        Task SetAsync<T>(string key, T data);
        Task SetAsync<T>(string key, T data, DistributedCacheEntryOptions options);
        void SetString(string key, string data);
        void SetString(string key, string data, DistributedCacheEntryOptions options);
        Task SetStringAsync(string key, string data);
        Task SetStringAsync(string key, string data, DistributedCacheEntryOptions options);
    }
}
