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
    public interface ICache2
    {
        /// <summary>
        /// 取得Redis物件
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 取得Redis內容
        /// </summary>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// 設置Redis內容(永久存在)
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="cancellationToken"></param>
        public Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default);

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="absoluteExpiration">絕對過期時間：時間到後就會消失</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync(string key, string value, DateTimeOffset absoluteExpiration,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="options">保存時間設定</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="absoluteExpirationRelativeToNow">絕對過期時間：時間到後就會消失</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync<T>(string key, T? value, TimeSpan absoluteExpirationRelativeToNow,
            CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 設置Redis內容(永久存在)
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync<T>(string key, T? value, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="absoluteExpiration">絕對過期時間：時間到後就會消失</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync<T>(string key, T? value, DateTimeOffset absoluteExpiration,
            CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="options">絕對過期時間：時間到後就會消失</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync<T>(string key, T? value, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// 刷新Redis內容
        /// </summary>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RefreshAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// 刪除Redis內容
        /// </summary>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得所有符合match pattern的key
        /// </summary>
        /// <param name="match"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<string> GetKeys(string match, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="expiredSeconds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Dictionary<string, RedisResult>?> HashGetAsync(RedisKey redisKey, int expiredSeconds, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="expiredSeconds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HashSetAsync(RedisKey redisKey, RedisKey dataKey, RedisValue dataValue, int expiredSeconds, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<decimal> IncreaseValueAsync(RedisKey redisKey, RedisKey dataKey, RedisValue dataValue, CancellationToken cancellationToken = default);
    }
}
