using iCat.Cache.Interfaces;
using iCat.Cache.Models;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace iCat.Cache.Implements
{
    /// <inheritdoc/>
    public class RedisCacheImpl : ICache
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connection;
        private static LoadedLuaScript? _loadedIncreaseValueLuaScript;
        private static LoadedLuaScript? _loadedHGetAllLuaScript;
        private static LoadedLuaScript? _loadedHSetLuaScript;


        /// <inheritdoc/>
        public string Category => nameof(RedisCacheImpl);

        /// <inheritdoc/>
        public RedisCacheImpl(IDistributedCache cache, IConnectionMultiplexer connection)
        {
            _cache = cache;
            _connection = connection;
        }

        /// <inheritdoc/>
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var result = await GetStringAsync(key, cancellationToken);
            await RefreshAsync(key, cancellationToken);
            return string.IsNullOrWhiteSpace(result) ? default : JsonSerializer.Deserialize<T>(result);
        }

        /// <inheritdoc/>
        public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            var result = await _cache.GetStringAsync(key, cancellationToken);
            await RefreshAsync(key, cancellationToken);
            return result;
        }

        /// <inheritdoc/>
        public async Task SetStringAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            await _cache.SetStringAsync(key, value, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task SetStringAsync<T>(string key, T? value, CancellationToken cancellationToken = default) where T : class
        {
            await SetStringAsync(key, JsonSerializer.Serialize(value), cancellationToken);
        }

        /// <inheritdoc/>
        public async Task SetStringAsync(string key, string value, CacheOptions options, CancellationToken cancellationToken = default)
        {
            await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration,
            }, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task SetStringAsync<T>(string key, T? value, CacheOptions options, CancellationToken cancellationToken = default) where T : class
        {
            await SetStringAsync(key, JsonSerializer.Serialize(value), options, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RefreshAsync(key, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<string> GetKeys(string match, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var points = _connection.GetEndPoints();
            var keys = (_connection.GetServer(points.First())).KeysAsync(_connection.GetDatabase().Database, match);

            await foreach (var key in keys)
            {
                if (cancellationToken.IsCancellationRequested) break;
                yield return key.ToString();
            }
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, string?>?> HashGetAsync(string redisKey, CancellationToken cancellationToken = default)
        {
            if (_loadedHGetAllLuaScript == null)
            {
                string luaScript = @$"
                    local currentValue = redis.call('HGETALL',@redisKey)
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
            }, flags: CommandFlags.None);
            var dic = n.ToDictionary();
            await RefreshAsync(redisKey, cancellationToken);
            return n.ToDictionary().Select(p => KeyValuePair.Create(p.Key, (string?)p.Value)).ToDictionary(p => p.Key, p => p.Value);
        }

        /// <inheritdoc/>
        public async Task HashSetAsync(string redisKey, string dataKey, string dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            if (_loadedHSetLuaScript == null)
            {
                //redis.call('EXPIRE', @redisKey, @expiredSeconds)

                string luaScript = @$"
                    redis.call('HSET', @redisKey, @absexpKey, @absexpValue, @sldexpKey, @sldexpValue, @dataKey, @dataValue)
                    if @expiredAt ~= '-1' then
                      redis.call('EXPIRE', @redisKey, @expiredAt)
                    end
                ";
                LuaScript? prepared = null;
                StackExchange.Redis.IServer? server;
                var points = _connection.GetEndPoints();
                server = _connection.GetServer(points.First());
                prepared = LuaScript.Prepare(luaScript);
                _loadedHSetLuaScript = prepared.Load(server, CommandFlags.None);
            }
            var creationTime = DateTimeOffset.UtcNow;
            var absexpValue = GetAbsoluteExpiration(options?.AbsoluteExpiration ?? DateTimeOffset.Now, options ?? new CacheOptions());
            await _loadedHSetLuaScript.EvaluateAsync(_connection.GetDatabase(), new
            {
                redisKey = (RedisKey)redisKey,
                dataKey = (RedisValue)dataKey,
                dataValue = dataValue,
                absexpKey = "absexp",
                absexpValue = absexpValue?.Ticks ?? -1,
                sldexpKey = "sldexp",
                sldexpValue = options?.SlidingExpiration?.Ticks ?? -1,
                expiredAt = GetExpirationInSeconds(creationTime, absexpValue, options ?? new CacheOptions()) ?? -1
            }, flags: CommandFlags.None);
        }

        /// <inheritdoc/>
        public async Task<byte> IncreaseValueAsync(string redisKey, string dataKey, byte dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return byte.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<sbyte> IncreaseValueAsync(string redisKey, string dataKey, sbyte dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return sbyte.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<short> IncreaseValueAsync(string redisKey, string dataKey, short dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return short.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<ushort> IncreaseValueAsync(string redisKey, string dataKey, ushort dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return ushort.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<int> IncreaseValueAsync(string redisKey, string dataKey, int dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return int.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<uint> IncreaseValueAsync(string redisKey, string dataKey, uint dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return uint.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<long> IncreaseValueAsync(string redisKey, string dataKey, long dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return long.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<ulong> IncreaseValueAsync(string redisKey, string dataKey, ulong dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return ulong.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<float> IncreaseValueAsync(string redisKey, string dataKey, float dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return float.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<double> IncreaseValueAsync(string redisKey, string dataKey, double dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return double.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        /// <inheritdoc/>
        public async Task<decimal> IncreaseValueAsync(string redisKey, string dataKey, decimal dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            return decimal.Parse(await IncreaseValueAsync((RedisKey)redisKey, (RedisValue)dataKey, (RedisValue)dataValue.ToString(), options));
        }

        private async Task<string> IncreaseValueAsync(RedisKey redisKey, RedisValue dataKey, RedisValue dataValue, CacheOptions options, CancellationToken cancellationToken = default)
        {
            if (_loadedIncreaseValueLuaScript == null)
            {
                string luaScript = @$"
                    local currentValue = redis.call('HGET',@redisKey, @dataKey)
                    local newValue = 0
                    if currentValue==false then newValue = @dataValue else newValue = currentValue + @dataValue end
                    redis.call('HSET', @redisKey, @absexpKey, @absexpValue, @sldexpKey, @sldexpValue, @dataKey, newValue)
                    if @expiredAt ~= '-1' then
                      redis.call('EXPIRE', @redisKey, @expiredAt)
                    end
                    return tostring(newValue)";
                LuaScript? prepared = null;
                StackExchange.Redis.IServer? server;
                var points = _connection.GetEndPoints();
                server = _connection.GetServer(points.First());
                prepared = LuaScript.Prepare(luaScript);
                _loadedIncreaseValueLuaScript = prepared.Load(server, CommandFlags.None);
            }

            var creationTime = DateTimeOffset.UtcNow;
            var absexpValue = GetAbsoluteExpiration(options?.AbsoluteExpiration ?? DateTimeOffset.Now, options ?? new CacheOptions());
            var n = await _loadedIncreaseValueLuaScript.EvaluateAsync(_connection.GetDatabase(), new
            {
                redisKey = redisKey,
                dataKey = dataKey,
                dataValue = dataValue,
                absexpKey = "absexp",
                absexpValue = absexpValue?.Ticks ?? -1,
                sldexpKey = "sldexp",
                sldexpValue = options?.SlidingExpiration?.Ticks ?? -1,
                expiredAt = GetExpirationInSeconds(creationTime, absexpValue, options ?? new CacheOptions()) ?? -1
            }, flags: CommandFlags.None);
            return n?.ToString() ?? "0";
        }

        private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, CacheOptions options)
        {
            if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentOutOfRangeException(
                    nameof(DistributedCacheEntryOptions.AbsoluteExpiration),
                    options.AbsoluteExpiration.Value,
                    "The absolute expiration value must be in the future.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            }

            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                return creationTime + options.AbsoluteExpirationRelativeToNow;
            }

            return options.AbsoluteExpiration;
        }

        private static long? GetExpirationInSeconds(DateTimeOffset creationTime, DateTimeOffset? absoluteExpiration, CacheOptions options)
        {
            if (absoluteExpiration.HasValue && options.SlidingExpiration.HasValue)
            {
                return (long)Math.Min(
                    (absoluteExpiration.Value - creationTime).TotalSeconds,
                    options.SlidingExpiration.Value.TotalSeconds);
            }
            else if (absoluteExpiration.HasValue)
            {
                return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;
            }
            else if (options.SlidingExpiration.HasValue)
            {
                return (long)options.SlidingExpiration.Value.TotalSeconds;
            }
            return null;
        }
    }
}
