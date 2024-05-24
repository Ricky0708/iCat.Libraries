using iCat.Cache.Models;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
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
    public interface ICache
    {
        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Get value by key
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Get value by ke
        /// </summary>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set key value without expired
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set key value without expired
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SetStringAsync<T>(string key, T? value, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Set key value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SetStringAsync(string key, string value, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set key value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SetStringAsync<T>(string key, T? value, CacheOptions options, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Refresh
        /// </summary>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RefreshAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove target key
        /// </summary>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all keys that matched the pattern
        /// </summary>
        /// <param name="match"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IAsyncEnumerable<string> GetKeys(string match, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get feilds value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="expiredSeconds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Dictionary<string, string?>?> HashGetAsync(string redisKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set filed value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="expiredSeconds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HashSetAsync(string redisKey, string dataKey, string dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<byte> IncreaseValueAsync(string redisKey, string dataKey, byte dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<sbyte> IncreaseValueAsync(string redisKey, string dataKey, sbyte dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<short> IncreaseValueAsync(string redisKey, string dataKey, short dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ushort> IncreaseValueAsync(string redisKey, string dataKey, ushort dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> IncreaseValueAsync(string redisKey, string dataKey, int dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<uint> IncreaseValueAsync(string redisKey, string dataKey, uint dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> IncreaseValueAsync(string redisKey, string dataKey, long dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ulong> IncreaseValueAsync(string redisKey, string dataKey, ulong dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<float> IncreaseValueAsync(string redisKey, string dataKey, float dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<double> IncreaseValueAsync(string redisKey, string dataKey, double dataValue, CacheOptions options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Increase field value
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<decimal> IncreaseValueAsync(string redisKey, string dataKey, decimal dataValue, CacheOptions options, CancellationToken cancellationToken = default);
    }
}
