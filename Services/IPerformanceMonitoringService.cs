using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IPerformanceMonitoringService
    {
        // Database performance metrics
        Task<DatabaseMetrics> GetDatabaseMetricsAsync();
        
        // Cache performance metrics
        Task<CacheMetrics> GetCacheMetricsAsync();
        
        // Application performance metrics
        Task<ApplicationMetrics> GetApplicationMetricsAsync();
        
        // Record performance data
        void RecordDatabaseQuery(string query, TimeSpan duration, bool fromCache = false);
        void RecordCacheHit(string cacheKey);
        void RecordCacheMiss(string cacheKey);
        void RecordRequestMetrics(string action, TimeSpan duration, int statusCode);
        
        // Health status
        Task<HealthStatus> GetHealthStatusAsync();
    }

    public class DatabaseMetrics
    {
        public double AverageQueryTime { get; set; }
        public int TotalQueries { get; set; }
        public int SlowQueries { get; set; }
        public int CachedQueries { get; set; }
        public double CacheHitRatio => TotalQueries > 0 ? (double)CachedQueries / TotalQueries * 100 : 0;
        public DateTime LastMeasured { get; set; }
    }

    public class CacheMetrics
    {
        public int TotalRequests { get; set; }
        public int CacheHits { get; set; }
        public int CacheMisses { get; set; }
        public double HitRatio => TotalRequests > 0 ? (double)CacheHits / TotalRequests * 100 : 0;
        public long MemoryUsed { get; set; }
        public int ItemCount { get; set; }
        public DateTime LastMeasured { get; set; }
    }

    public class ApplicationMetrics
    {
        public double AverageResponseTime { get; set; }
        public int TotalRequests { get; set; }
        public int ErrorRequests { get; set; }
        public double ErrorRate => TotalRequests > 0 ? (double)ErrorRequests / TotalRequests * 100 : 0;
        public long MemoryUsage { get; set; }
        public double CpuUsage { get; set; }
        public DateTime LastMeasured { get; set; }
    }

    public class HealthStatus
    {
        public string Status { get; set; }
        public Dictionary<string, object> Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 