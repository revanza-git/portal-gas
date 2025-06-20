using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class PerformanceMonitoringService : IPerformanceMonitoringService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentQueue<QueryMetric> _queryMetrics;
        private readonly ConcurrentQueue<CacheMetric> _cacheMetrics;
        private readonly ConcurrentQueue<RequestMetric> _requestMetrics;
        private readonly object _lockObject = new object();

        // Keep metrics for the last hour
        private readonly TimeSpan _metricsWindow = TimeSpan.FromHours(1);

        public PerformanceMonitoringService(IMemoryCache cache)
        {
            _cache = cache;
            _queryMetrics = new ConcurrentQueue<QueryMetric>();
            _cacheMetrics = new ConcurrentQueue<CacheMetric>();
            _requestMetrics = new ConcurrentQueue<RequestMetric>();
        }

        public async Task<DatabaseMetrics> GetDatabaseMetricsAsync()
        {
            CleanupOldMetrics();
            
            var cutoff = DateTime.UtcNow.Subtract(_metricsWindow);
            var recentQueries = _queryMetrics.Where(q => q.Timestamp > cutoff).ToList();

            return await Task.FromResult(new DatabaseMetrics
            {
                AverageQueryTime = recentQueries.Count > 0 ? recentQueries.Average(q => q.Duration.TotalMilliseconds) : 0,
                TotalQueries = recentQueries.Count,
                SlowQueries = recentQueries.Count(q => q.Duration.TotalMilliseconds > 1000), // Queries > 1 second
                CachedQueries = recentQueries.Count(q => q.FromCache),
                LastMeasured = DateTime.UtcNow
            });
        }

        public async Task<CacheMetrics> GetCacheMetricsAsync()
        {
            CleanupOldMetrics();
            
            var cutoff = DateTime.UtcNow.Subtract(_metricsWindow);
            var recentCacheMetrics = _cacheMetrics.Where(c => c.Timestamp > cutoff).ToList();

            var hits = recentCacheMetrics.Count(c => c.IsHit);
            var total = recentCacheMetrics.Count;

            // Get memory cache statistics if available
            var memoryUsed = 0L;
            var itemCount = 0;
            
            if (_cache is MemoryCache mc)
            {
                // Use reflection to get internal cache statistics
                try
                {
                    var field = typeof(MemoryCache).GetField("_options", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field?.GetValue(mc) is MemoryCacheOptions options)
                    {
                        memoryUsed = options.SizeLimit ?? 0;
                    }
                }
                catch
                {
                    // Silently fail if reflection doesn't work
                }
            }

            return await Task.FromResult(new CacheMetrics
            {
                TotalRequests = total,
                CacheHits = hits,
                CacheMisses = total - hits,
                MemoryUsed = memoryUsed,
                ItemCount = itemCount,
                LastMeasured = DateTime.UtcNow
            });
        }

        public async Task<ApplicationMetrics> GetApplicationMetricsAsync()
        {
            CleanupOldMetrics();
            
            var cutoff = DateTime.UtcNow.Subtract(_metricsWindow);
            var recentRequests = _requestMetrics.Where(r => r.Timestamp > cutoff).ToList();

            var process = Process.GetCurrentProcess();
            var memoryUsage = process.WorkingSet64;
            
            // Simple CPU usage estimation (this would be more accurate with a dedicated performance counter)
            var cpuUsage = 0.0;

            return await Task.FromResult(new ApplicationMetrics
            {
                AverageResponseTime = recentRequests.Count > 0 ? recentRequests.Average(r => r.Duration.TotalMilliseconds) : 0,
                TotalRequests = recentRequests.Count,
                ErrorRequests = recentRequests.Count(r => r.StatusCode >= 400),
                MemoryUsage = memoryUsage,
                CpuUsage = cpuUsage,
                LastMeasured = DateTime.UtcNow
            });
        }

        public void RecordDatabaseQuery(string query, TimeSpan duration, bool fromCache = false)
        {
            _queryMetrics.Enqueue(new QueryMetric
            {
                Query = query,
                Duration = duration,
                FromCache = fromCache,
                Timestamp = DateTime.UtcNow
            });
        }

        public void RecordCacheHit(string cacheKey)
        {
            _cacheMetrics.Enqueue(new CacheMetric
            {
                CacheKey = cacheKey,
                IsHit = true,
                Timestamp = DateTime.UtcNow
            });
        }

        public void RecordCacheMiss(string cacheKey)
        {
            _cacheMetrics.Enqueue(new CacheMetric
            {
                CacheKey = cacheKey,
                IsHit = false,
                Timestamp = DateTime.UtcNow
            });
        }

        public void RecordRequestMetrics(string action, TimeSpan duration, int statusCode)
        {
            _requestMetrics.Enqueue(new RequestMetric
            {
                Action = action,
                Duration = duration,
                StatusCode = statusCode,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task<HealthStatus> GetHealthStatusAsync()
        {
            var dbMetrics = await GetDatabaseMetricsAsync();
            var cacheMetrics = await GetCacheMetricsAsync();
            var appMetrics = await GetApplicationMetricsAsync();

            var status = "Healthy";
            var details = new Dictionary<string, object>();

            // Determine overall health based on metrics
            if (dbMetrics.AverageQueryTime > 2000 || appMetrics.ErrorRate > 5 || appMetrics.MemoryUsage > 2_000_000_000L) // 2GB
            {
                status = "Degraded";
            }

            if (dbMetrics.AverageQueryTime > 5000 || appMetrics.ErrorRate > 15 || appMetrics.MemoryUsage > 4_000_000_000L) // 4GB
            {
                status = "Unhealthy";
            }

            details.Add("database", new { 
                avgQueryTime = dbMetrics.AverageQueryTime,
                cacheHitRatio = dbMetrics.CacheHitRatio,
                slowQueries = dbMetrics.SlowQueries 
            });
            
            details.Add("cache", new { 
                hitRatio = cacheMetrics.HitRatio,
                totalRequests = cacheMetrics.TotalRequests 
            });
            
            details.Add("application", new { 
                avgResponseTime = appMetrics.AverageResponseTime,
                errorRate = appMetrics.ErrorRate,
                memoryUsageMB = appMetrics.MemoryUsage / (1024 * 1024) 
            });

            return new HealthStatus
            {
                Status = status,
                Details = details,
                Timestamp = DateTime.UtcNow
            };
        }

        private void CleanupOldMetrics()
        {
            var cutoff = DateTime.UtcNow.Subtract(_metricsWindow);

            // Clean up old query metrics
            while (_queryMetrics.TryPeek(out var query) && query.Timestamp < cutoff)
            {
                _queryMetrics.TryDequeue(out _);
            }

            // Clean up old cache metrics
            while (_cacheMetrics.TryPeek(out var cache) && cache.Timestamp < cutoff)
            {
                _cacheMetrics.TryDequeue(out _);
            }

            // Clean up old request metrics
            while (_requestMetrics.TryPeek(out var request) && request.Timestamp < cutoff)
            {
                _requestMetrics.TryDequeue(out _);
            }
        }

        private class QueryMetric
        {
            public string Query { get; set; }
            public TimeSpan Duration { get; set; }
            public bool FromCache { get; set; }
            public DateTime Timestamp { get; set; }
        }

        private class CacheMetric
        {
            public string CacheKey { get; set; }
            public bool IsHit { get; set; }
            public DateTime Timestamp { get; set; }
        }

        private class RequestMetric
        {
            public string Action { get; set; }
            public TimeSpan Duration { get; set; }
            public int StatusCode { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
} 