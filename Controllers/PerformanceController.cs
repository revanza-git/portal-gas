using Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    [Authorize(Roles = "AdminQM")]
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceMonitoringService _performanceService;

        public PerformanceController(IPerformanceMonitoringService performanceService)
        {
            _performanceService = performanceService;
        }

        /// <summary>
        /// Get comprehensive performance dashboard data
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var dbMetrics = await _performanceService.GetDatabaseMetricsAsync();
            var cacheMetrics = await _performanceService.GetCacheMetricsAsync();
            var appMetrics = await _performanceService.GetApplicationMetricsAsync();
            var healthStatus = await _performanceService.GetHealthStatusAsync();

            var dashboard = new
            {
                timestamp = System.DateTime.UtcNow,
                database = new
                {
                    averageQueryTime = $"{dbMetrics.AverageQueryTime:F2}ms",
                    totalQueries = dbMetrics.TotalQueries,
                    slowQueries = dbMetrics.SlowQueries,
                    cacheHitRatio = $"{dbMetrics.CacheHitRatio:F1}%",
                    lastMeasured = dbMetrics.LastMeasured
                },
                cache = new
                {
                    hitRatio = $"{cacheMetrics.HitRatio:F1}%",
                    totalRequests = cacheMetrics.TotalRequests,
                    cacheHits = cacheMetrics.CacheHits,
                    cacheMisses = cacheMetrics.CacheMisses,
                    memoryUsedMB = cacheMetrics.MemoryUsed / (1024 * 1024),
                    itemCount = cacheMetrics.ItemCount,
                    lastMeasured = cacheMetrics.LastMeasured
                },
                application = new
                {
                    averageResponseTime = $"{appMetrics.AverageResponseTime:F2}ms",
                    totalRequests = appMetrics.TotalRequests,
                    errorRequests = appMetrics.ErrorRequests,
                    errorRate = $"{appMetrics.ErrorRate:F2}%",
                    memoryUsageMB = appMetrics.MemoryUsage / (1024 * 1024),
                    cpuUsage = $"{appMetrics.CpuUsage:F1}%",
                    lastMeasured = appMetrics.LastMeasured
                },
                health = new
                {
                    status = healthStatus.Status,
                    details = healthStatus.Details,
                    timestamp = healthStatus.Timestamp
                }
            };

            return Ok(dashboard);
        }

        /// <summary>
        /// Get database performance metrics
        /// </summary>
        [HttpGet("database")]
        public async Task<IActionResult> GetDatabaseMetrics()
        {
            var metrics = await _performanceService.GetDatabaseMetricsAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Get cache performance metrics
        /// </summary>
        [HttpGet("cache")]
        public async Task<IActionResult> GetCacheMetrics()
        {
            var metrics = await _performanceService.GetCacheMetricsAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Get application performance metrics
        /// </summary>
        [HttpGet("application")]
        public async Task<IActionResult> GetApplicationMetrics()
        {
            var metrics = await _performanceService.GetApplicationMetricsAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Get overall health status
        /// </summary>
        [HttpGet("health-status")]
        public async Task<IActionResult> GetHealthStatus()
        {
            var health = await _performanceService.GetHealthStatusAsync();
            return Ok(health);
        }

        /// <summary>
        /// Get performance recommendations
        /// </summary>
        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations()
        {
            var dbMetrics = await _performanceService.GetDatabaseMetricsAsync();
            var cacheMetrics = await _performanceService.GetCacheMetricsAsync();
            var appMetrics = await _performanceService.GetApplicationMetricsAsync();

            var recommendations = new List<string>();

            // Database recommendations
            if (dbMetrics.AverageQueryTime > 1000)
            {
                recommendations.Add("Database queries are averaging over 1 second. Consider optimizing slow queries or adding indexes.");
            }

            if (dbMetrics.CacheHitRatio < 80)
            {
                recommendations.Add($"Database cache hit ratio is {dbMetrics.CacheHitRatio:F1}%. Consider increasing cache expiration times or cache more data.");
            }

            if (dbMetrics.SlowQueries > 10)
            {
                recommendations.Add($"Found {dbMetrics.SlowQueries} slow queries in the last hour. Review and optimize query performance.");
            }

            // Cache recommendations
            if (cacheMetrics.HitRatio < 70)
            {
                recommendations.Add($"Cache hit ratio is {cacheMetrics.HitRatio:F1}%. Consider reviewing cache strategy and expiration policies.");
            }

            // Application recommendations
            if (appMetrics.ErrorRate > 5)
            {
                recommendations.Add($"Application error rate is {appMetrics.ErrorRate:F2}%. Investigate and fix error sources.");
            }

            if (appMetrics.AverageResponseTime > 2000)
            {
                recommendations.Add("Average response time is over 2 seconds. Consider performance optimizations.");
            }

            if (appMetrics.MemoryUsage > 2_000_000_000L) // 2GB
            {
                recommendations.Add($"High memory usage detected ({appMetrics.MemoryUsage / (1024 * 1024)} MB). Monitor for memory leaks.");
            }

            if (recommendations.Count == 0)
            {
                recommendations.Add("All performance metrics are within acceptable ranges. System is performing well.");
            }

            return Ok(new { recommendations, timestamp = System.DateTime.UtcNow });
        }
    }
} 