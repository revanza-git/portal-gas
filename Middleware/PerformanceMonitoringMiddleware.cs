using Admin.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Admin.Middleware
{
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
        private readonly IPerformanceMonitoringService _performanceService;

        public PerformanceMonitoringMiddleware(
            RequestDelegate next,
            ILogger<PerformanceMonitoringMiddleware> logger,
            IPerformanceMonitoringService performanceService)
        {
            _next = next;
            _logger = logger;
            _performanceService = performanceService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var path = context.Request.Path.Value;
            
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request failed for {Path}", path);
                context.Response.StatusCode = 500;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                
                // Only track controller actions, not static files or health checks
                if (ShouldTrackRequest(path))
                {
                    var action = GetActionFromPath(path);
                    _performanceService.RecordRequestMetrics(action, stopwatch.Elapsed, context.Response.StatusCode);
                    
                    // Log slow requests
                    if (stopwatch.ElapsedMilliseconds > 2000)
                    {
                        _logger.LogWarning("Slow request detected: {Path} took {ElapsedMs}ms", 
                            path, stopwatch.ElapsedMilliseconds);
                    }
                }
            }
        }

        private static bool ShouldTrackRequest(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // Skip static files and monitoring endpoints
            return !path.StartsWith("/css/") &&
                   !path.StartsWith("/js/") &&
                   !path.StartsWith("/images/") &&
                   !path.StartsWith("/lib/") &&
                   !path.StartsWith("/admin/") &&
                   !path.StartsWith("/health") &&
                   !path.StartsWith("/api/performance") &&
                   !path.Contains(".");
        }

        private static string GetActionFromPath(string path)
        {
            if (string.IsNullOrEmpty(path) || path == "/")
                return "Home/Index";

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            if (segments.Length == 0)
                return "Home/Index";
            
            if (segments.Length == 1)
                return $"{segments[0]}/Index";
                
            return $"{segments[0]}/{segments[1]}";
        }
    }
} 