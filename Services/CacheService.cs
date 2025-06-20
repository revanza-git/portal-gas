using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Admin.Services
{
    public interface ICacheService
    {
        void InvalidateAmanCache(string amanId = null);
        void InvalidateStaticDataCache();
        void InvalidateUserSpecificCache(string userName, string role);
        void SetCache<T>(string key, T value, TimeSpan expiration);
        bool TryGetCache<T>(string key, out T value);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly List<string> _cacheKeys;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
            _cacheKeys = new List<string>();
        }

        public void InvalidateAmanCache(string amanId = null)
        {
            if (!string.IsNullOrEmpty(amanId))
            {
                // Invalidate specific aman cache
                var keysToRemove = _cacheKeys.Where(k => k.Contains($"aman_view_{amanId}")).ToList();
                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                    _cacheKeys.Remove(key);
                }
            }
            else
            {
                // Invalidate all aman-related caches
                var keysToRemove = _cacheKeys.Where(k => k.StartsWith("amans_") || k.StartsWith("aman_")).ToList();
                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                    _cacheKeys.Remove(key);
                }
            }
        }

        public void InvalidateStaticDataCache()
        {
            var staticDataKeys = new[] { "aman_static_data", "aman_view_static_data" };
            foreach (var key in staticDataKeys)
            {
                _cache.Remove(key);
                _cacheKeys.Remove(key);
            }
        }

        public void InvalidateUserSpecificCache(string userName, string role)
        {
            var keysToRemove = _cacheKeys.Where(k => k.Contains($"user_{userName}") || k.Contains($"role_{role}")).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _cacheKeys.Remove(key);
            }
        }

        public void SetCache<T>(string key, T value, TimeSpan expiration)
        {
            _cache.Set(key, value, expiration);
            if (!_cacheKeys.Contains(key))
            {
                _cacheKeys.Add(key);
            }
        }

        public bool TryGetCache<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }
    }
} 