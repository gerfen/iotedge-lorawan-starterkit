﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace LoraKeysManagerFacade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using StackExchange.Redis;

    public class LoRaDeviceCacheRedisStore : ILoRaDeviceCacheStore
    {
        private IDatabase redisCache;
        private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(10);

        public LoRaDeviceCacheRedisStore(IDatabase redisCache)
        {
            this.redisCache = redisCache;
        }

        public async Task<bool> LockTakeAsync(string key, string lockOwner, TimeSpan expiration, bool block = true)
        {
            var start = DateTime.UtcNow;
            var taken = false;
            while (!(taken = this.redisCache.LockTake(key, lockOwner, expiration)) && block)
            {
                if (DateTime.UtcNow - start > LockTimeout)
                    break;
                await Task.Delay(100);
            }

            return taken;
        }

        public string StringGet(string key)
        {
            return this.redisCache.StringGet(key, CommandFlags.DemandMaster);
        }

        public T GetObject<T>(string key)
            where T : class
        {
            var str = this.StringGet(key);
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch
            {
                return null;
            }
        }

        public bool StringSet(string key, string value, TimeSpan? expiry, bool onlyIfNotExists = false)
        {
            var when = onlyIfNotExists ? When.NotExists : When.Always;
            return this.redisCache.StringSet(key, value, expiry, when, CommandFlags.DemandMaster);
        }

        public bool ObjectSet<T>(string key, T value, TimeSpan? expiration, bool onlyIfNotExists = false)
            where T : class
        {
            var str = value != null ? JsonConvert.SerializeObject(value) : null;
            return this.StringSet(key, str, expiration, onlyIfNotExists);
        }

        public bool KeyExists(string key)
        {
            return this.redisCache.KeyExists(key, CommandFlags.DemandMaster);
        }

        public bool KeyDelete(string key)
        {
            return this.redisCache.KeyDelete(key);
        }

        public bool LockRelease(string key, string value)
        {
            return this.redisCache.LockRelease(key, value);
        }

        public long ListAdd(string key, string value, TimeSpan? expiry = null)
        {
            var itemCount = this.redisCache.ListRightPush(key, value);
            if (expiry.HasValue)
                this.redisCache.KeyExpire(key, expiry);

            return itemCount;
        }

        public IReadOnlyList<string> ListGet(string key)
        {
            var list = this.redisCache.ListRange(key);
            return list.Select(x => (string)x).ToList();
        }

        public bool TrySetHashObject(string cacheKey, string subkey, string value, TimeSpan? timeToExpire = null)
        {
            var returnValue = this.redisCache.HashSet(cacheKey, subkey, value);
            if (timeToExpire.HasValue)
            {
                this.redisCache.KeyExpire(cacheKey, DateTime.UtcNow.Add(timeToExpire.Value));
            }

            return returnValue;
        }

        public RedisValue[] TryGetHashObject(string key)
        {
            return this.redisCache.HashValues(key);
        }

        public void ReplaceHashObjects(string cacheKey, List<DevAddrCacheInfo> list, TimeSpan? timeToExpire = null)
        {
            HashEntry[] hashEntries = new HashEntry[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                hashEntries[i] = new HashEntry(list[i].DevEUI, JsonConvert.SerializeObject(list[i]));
            }

            this.redisCache.HashSet(cacheKey, hashEntries);
            if (timeToExpire.HasValue)
            {
                this.redisCache.KeyExpire(cacheKey, DateTime.UtcNow.Add(timeToExpire.Value));
            }
        }

        public void ChangeLockTTL(string key, TimeSpan timeToExpire)
        {
                this.redisCache.KeyExpire(key, DateTime.UtcNow.Add(timeToExpire));
        }
    }
}
