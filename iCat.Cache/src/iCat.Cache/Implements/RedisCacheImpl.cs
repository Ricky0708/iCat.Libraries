using iCat.Cache.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace iCat.Cache.Implements
{
    /// <inheritdoc/>
    public class RedisCacheImpl : ICache2
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connection;
        private static LoadedLuaScript? _loadedIncreaseValueLuaScript;
        private static LoadedLuaScript? _loadedHGetAllLuaScript;
        private static LoadedLuaScript? _loadedHSetLuaScript;

        /// <inheritdoc/>
        public RedisCacheImpl(IDistributedCache cache, IConnectionMultiplexer connection)
        {
            _cache = cache;
            _connection = connection;
        }

        /// <summary>
        /// 取得Redis物件
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var result = await _cache.GetStringAsync(key, cancellationToken);
            return string.IsNullOrWhiteSpace(result) ? default : JsonSerializer.Deserialize<T>(result);
        }

        /// <summary>
        /// 取得Redis內容
        /// </summary>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _cache.GetStringAsync(key, cancellationToken);
        }

        /// <summary>
        /// 設置Redis內容(永久存在)
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="cancellationToken"></param>
        public Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            return _cache.SetStringAsync(key, value, cancellationToken);
        }

        /// <summary>
        /// 設置Redis內容(永久存在)
        /// </summary>
        /// <typeparam name="T">型別</typeparam>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync<T>(string key, T? value, CancellationToken cancellationToken = default) where T : class
        {
            return SetStringAsync(key, JsonSerializer.Serialize(value), cancellationToken);
        }

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="absoluteExpirationRelativeToNow">絕對過期時間：時間到後就會消失</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync<T>(string key, T? value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default) where T : class
        {
            return SetStringAsync(key, JsonSerializer.Serialize(value), absoluteExpirationRelativeToNow, cancellationToken);
        }

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="absoluteExpirationRelativeToNow">絕對過期時間：時間到後就會消失</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync(string key, string value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow };
            return _cache.SetStringAsync(key, value, options, cancellationToken);
        }

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="absoluteExpiration">絕對過期時間：時間到後就會消失</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync<T>(string key, T? value, DateTimeOffset absoluteExpiration, CancellationToken cancellationToken = default) where T : class
        {
            return SetStringAsync(key, JsonSerializer.Serialize(value), absoluteExpiration, cancellationToken);
        }

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <typeparam name="T">物件型別</typeparam>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="options">絕對過期時間：時間到後就會消失</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync<T>(string key, T? value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        where T : class
        {
            return SetStringAsync(key, JsonSerializer.Serialize(value), options, cancellationToken);
        }

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="absoluteExpiration">絕對過期時間：時間到後就會消失</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync(string key, string value, DateTimeOffset absoluteExpiration, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions { AbsoluteExpiration = absoluteExpiration };
            return _cache.SetStringAsync(key, value, options, cancellationToken);
        }

        /// <summary>
        /// 設置Redis內容
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="value">內容</param>
        /// <param name="options">保存時間設定</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            return _cache.SetStringAsync(key, value, options, cancellationToken);
        }

        /// <summary>
        /// 刷新Redis內容
        /// </summary>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            return _cache.RefreshAsync(key, cancellationToken);
        }

        /// <summary>
        /// 刪除Redis內容
        /// </summary>
        /// <param name="key">緩存鍵值</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return _cache.RemoveAsync(key, cancellationToken);
        }

        /// <summary>
        /// 取得所有符合match pattern的key
        /// </summary>
        /// <param name="match"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> GetKeys(string match, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var points = _connection.GetEndPoints();
            var keys = (_connection.GetServer(points.First())).KeysAsync(_connection.GetDatabase().Database, match);

            await foreach (var key in keys)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                yield return key.ToString();
            }
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, RedisResult>?> HashGetAsync(RedisKey redisKey, int expiredSeconds, CancellationToken cancellationToken = default)
        {
            if (_loadedHGetAllLuaScript == null)
            {
                string luaScript = @$"
                    local currentValue = redis.call('HGETALL',@redisKey)
                    redis.call('EXPIRE', @redisKey, @expiredSeconds)
                    return currentValue";
                LuaScript? prepared = null;
                StackExchange.Redis.IServer? server;
                var points = _connection.GetEndPoints();
                server = _connection.GetServer(points.First());
                prepared = LuaScript.Prepare(luaScript);
                _loadedHGetAllLuaScript = prepared.Load(server, CommandFlags.None);
            }

            var n = await _loadedHGetAllLuaScript.EvaluateAsync(_connection.GetDatabase(), new
            {
                redisKey = (RedisKey)redisKey,
                expiredSeconds = expiredSeconds
            }, flags: CommandFlags.None);
            return n.ToDictionary();
        }

        /// <inheritdoc/>
        public async Task HashSetAsync(RedisKey redisKey, RedisKey dataKey, RedisValue dataValue, int expiredSeconds, CancellationToken cancellationToken = default)
        {
            if (_loadedHSetLuaScript == null)
            {
                string luaScript = @$"
                    redis.call('HSET', @redisKey, @dataKey, @dataValue)
                    redis.call('EXPIRE', @redisKey, @expiredSeconds)";
                LuaScript? prepared = null;
                StackExchange.Redis.IServer? server;
                var points = _connection.GetEndPoints();
                server = _connection.GetServer(points.First());
                prepared = LuaScript.Prepare(luaScript);
                _loadedHSetLuaScript = prepared.Load(server, CommandFlags.None);
            }

            await _loadedHSetLuaScript.EvaluateAsync(_connection.GetDatabase(), new
            {
                redisKey = (RedisKey)redisKey,
                dataKey = dataKey,
                dataValue = dataValue,
                expiredSeconds = expiredSeconds
            }, flags: CommandFlags.None);
        }

        /// <inheritdoc/>
        public async Task<decimal> IncreaseValueAsync(RedisKey redisKey, RedisKey dataKey, RedisValue dataValue, CancellationToken cancellationToken = default)
        {
            if (_loadedIncreaseValueLuaScript == null)
            {
                string luaScript = @$"
                    local currentValue = redis.call('HGET',@redisKey, @dataKey)
                    local newValue = 0
                    if currentValue==false then newValue = @dataValue else newValue = currentValue + @dataValue end
                    redis.call('HSET', @redisKey, @dataKey, newValue)
                    redis.call('EXPIRE', @redisKey, {1 * 70})
                    return newValue";
                LuaScript? prepared = null;
                StackExchange.Redis.IServer? server;
                var points = _connection.GetEndPoints();
                server = _connection.GetServer(points.First());
                prepared = LuaScript.Prepare(luaScript);
                _loadedIncreaseValueLuaScript = prepared.Load(server, CommandFlags.None);
            }

            var n = await _loadedIncreaseValueLuaScript.EvaluateAsync(_connection.GetDatabase(), new
            {
                redisKey = (RedisKey)redisKey,
                dataKey = dataKey,
                dataValue = (RedisKey)dataValue.ToString()

            }, flags: CommandFlags.None);
            return decimal.Parse(n?.ToString() ?? "0");
        }
    }
}
