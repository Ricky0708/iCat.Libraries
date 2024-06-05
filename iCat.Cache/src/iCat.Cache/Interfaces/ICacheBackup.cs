using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Cache.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheBackup
    {
        /// <summary>
        /// IDistributedCache adapter, System.Text.Json.JsonSerializer.Deserialize and return model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T? Get<T>(string key);

        /// <summary>
        /// IDistributedCache adapter, System.Text.Json.JsonSerializer.Deserialize and return model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[]? GetBytes(string key);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<byte[]?> GetBytesAsync(string key);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string? GetString(string key);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string?> GetStringAsync(string key);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        void Refresh(string key);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RefreshAsync(string key);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveAsync(string key);

        /// <summary>
        /// IDistributedCache adapter, System.Text.Json.JsonSerializer.Serialize Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void Set<T>(string key, T data);

        /// <summary>
        /// IDistributedCache adapter, System.Text.Json.JsonSerializer.Serialize Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="options"></param>
        void Set<T>(string key, T data, DistributedCacheEntryOptions options);

        /// <summary>
        /// IDistributedCache adapter, System.Text.Json.JsonSerializer.Serialize Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T data);

        /// <summary>
        /// IDistributedCache adapter, System.Text.Json.JsonSerializer.Serialize Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T data, DistributedCacheEntryOptions options);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetString(string key, string data);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="options"></param>
        void SetString(string key, string data, DistributedCacheEntryOptions options);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SetStringAsync(string key, string data);

        /// <summary>
        /// IDistributedCache adapter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task SetStringAsync(string key, string data, DistributedCacheEntryOptions options);
    }
}
