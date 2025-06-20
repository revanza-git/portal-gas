# Performance Optimization Guide for Portal Gas Project

## Overview
This guide provides comprehensive performance optimization strategies for your .NET 8 ASP.NET Core portal-gas application with Entity Framework Core and SQL Server.

## 1. Database Performance Optimizations

### 1.1 Database Indexing Strategy
Add strategic indexes to improve query performance:

```sql
-- Critical indexes for frequently queried tables
CREATE INDEX IX_Amans_Status_CreationDateTime ON Amans (Status, CreationDateTime);
CREATE INDEX IX_Amans_Department_Status ON Amans (Department, Status);
CREATE INDEX IX_Amans_Location_Status ON Amans (Location, Status);

-- Gas monitoring data indexes
CREATE INDEX IX_Gm_FSRUData_Date_FSRUID ON Gm_FSRUData (Date, FSRUID);
CREATE INDEX IX_Gm_ORFData_Date_LineID ON Gm_ORFData (Date, LineID);
CREATE INDEX IX_Gm_VesselData_Date_VesselID ON Gm_VesselData (Date, VesselID);

-- User activity indexes
CREATE INDEX IX_AspNetUsers_Email ON AspNetUsers (Email);
CREATE INDEX IX_DCUs_Date_JenisPekerjaan ON DCUs (Date, JenisPekerjaan);

-- Content management indexes
CREATE INDEX IX_News_Status_PublishingDate ON News (Status, PublishingDate);
CREATE INDEX IX_Photos_GalleryID_Status ON Photos (GalleryID, Status);
CREATE INDEX IX_Videos_GalleryID_Status ON Videos (GalleryID, Status);

-- Overtime and scheduling indexes
CREATE INDEX IX_Overtime_tanggal_status ON Overtime (tanggal, status);
CREATE INDEX IX_Overtime_Department_tanggal ON Overtime (Department, tanggal);
```

### 1.2 Database Connection Optimization
Update your `ApplicationDbContext` configuration:

```csharp
// In Startup.cs ConfigureDatabase method
private void ConfigureDatabase(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(Configuration["Data:PortalNR:ConnectionString"], sqlOptions =>
        {
            sqlOptions.CommandTimeout(30); // Set command timeout
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });
        
        // Enable sensitive data logging only in development
        if (Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
        }
        
        // Disable change tracking for read-only scenarios
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });
    
    // Add connection pooling
    services.AddDbContextPool<ApplicationDbContext>(options =>
        options.UseSqlServer(Configuration["Data:PortalNR:ConnectionString"]));
}
```

### 1.3 Query Optimization Patterns
Implement efficient querying patterns in your repositories:

```csharp
// Example for AmanRepository - use projection and filtering
public async Task<IEnumerable<AmanSummaryDto>> GetAmanSummariesAsync(int page, int pageSize)
{
    return await _context.Amans
        .AsNoTracking() // Disable change tracking for read-only queries
        .Where(a => a.Status != 0) // Filter early
        .Select(a => new AmanSummaryDto // Project only needed fields
        {
            AmanID = a.AmanID,
            Findings = a.Findings,
            Status = a.Status,
            CreationDateTime = a.CreationDateTime
        })
        .OrderByDescending(a => a.CreationDateTime)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}

// Use compiled queries for frequently executed queries
private static readonly Func<ApplicationDbContext, string, Task<Aman>> GetAmanByIdQuery =
    EF.CompileAsyncQuery((ApplicationDbContext context, string id) =>
        context.Amans.FirstOrDefault(a => a.AmanID == id));
```

## 2. Application-Level Performance Optimizations

### 2.1 Caching Strategy
Implement comprehensive caching:

```csharp
// Update Startup.cs to add Redis cache
private void ConfigureFrameworkServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    
    // Add distributed cache (Redis recommended for production)
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = Configuration.GetConnectionString("Redis");
    });
    
    // Fallback to in-memory cache for development
    services.AddMemoryCache(options =>
    {
        options.SizeLimit = 1024; // Limit cache size
    });
    
    // Add response caching
    services.AddResponseCaching(options =>
    {
        options.MaximumBodySize = 1024 * 1024; // 1MB
        options.UseCaseSensitivePaths = false;
    });
    
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
}
```

### 2.2 Async/Await Optimization
Ensure all I/O operations are properly async:

```csharp
// Example controller optimization
[HttpGet]
[ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "page", "size" })]
public async Task<IActionResult> Index(int page = 1, int size = 20)
{
    var cacheKey = $"amans_page_{page}_size_{size}";
    
    if (!_cache.TryGetValue(cacheKey, out var cachedResult))
    {
        var amans = await _amanRepository.GetPagedAsync(page, size);
        cachedResult = amans;
        
        _cache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(10));
    }
    
    return View(cachedResult);
}
```

### 2.3 Dependency Injection Optimization
Update service registrations for better performance:

```csharp
private void ConfigureRepositories(IServiceCollection services)
{
    // Use Scoped instead of Transient for repositories that maintain state
    services.AddScoped<IAmanRepository, EFAmanRepository>();
    services.AddScoped<ISemarRepository, EFSemarRepository>();
    services.AddScoped<INOCRepository, EFNOCRepository>();
    
    // Keep Transient for stateless services
    services.AddTransient<IEmailSender, AuthMessageSender>();
    services.AddTransient<ISmsSender, AuthMessageSender>();
    
    // Use Singleton for expensive-to-create services
    services.AddSingleton<IConverter>(provider => 
        new SynchronizedConverter(new PdfTools()));
}
```

## 3. Frontend Performance Optimizations

### 3.1 Static File Optimization
Add compression and caching for static files:

```csharp
// In Configure method of Startup.cs
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Add response compression
    app.UseResponseCompression();
    
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts(); // Add HSTS for security and performance
    }

    app.UseStatusCodePages();
    
    // Optimize static file serving
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            // Cache static files for 1 year
            ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
        }
    });
    
    // Add response caching middleware
    app.UseResponseCaching();
    
    app.UseAuthentication();
    app.UseSession();
    app.UseRequestLocalization();
    app.UseRouting();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });
}
```

### 3.2 Add Response Compression
Add compression services:

```csharp
private void ConfigureFrameworkServices(IServiceCollection services)
{
    services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
        {
            "application/json",
            "text/json",
            "text/css",
            "application/javascript",
            "text/html",
            "application/xml",
            "text/xml",
            "application/atom+xml",
            "text/plain"
        });
    });
    
    services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    });
    
    services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    });
}
```

## 4. Memory and Resource Management

### 4.1 Implement Proper Disposal Patterns
Create a base repository with proper disposal:

```csharp
public abstract class BaseRepository : IDisposable
{
    protected readonly ApplicationDbContext _context;
    private bool _disposed = false;

    protected BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
```

### 4.2 Optimize File Upload Handling
For large file uploads (like your PDF generation):

```csharp
[HttpPost]
[RequestSizeLimit(50 * 1024 * 1024)] // 50MB limit
public async Task<IActionResult> UploadFile(IFormFile file)
{
    if (file?.Length > 0)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        
        // Process file in chunks to avoid memory issues
        var buffer = new byte[8192];
        // ... processing logic
    }
    
    return Ok();
}
```

## 5. Monitoring and Diagnostics

### 5.1 Add Health Checks
```csharp
private void ConfigureFrameworkServices(IServiceCollection services)
{
    services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>()
        .AddSqlServer(Configuration["Data:PortalNR:ConnectionString"])
        .AddMemoryHealthCheck("memory");
}

// In Configure method
app.UseHealthChecks("/health");
```

### 5.2 Add Application Insights or Logging
```csharp
services.AddApplicationInsightsTelemetry();

// Or configure detailed logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
    builder.AddEventSourceLogger();
    
    if (env.IsProduction())
    {
        builder.SetMinimumLevel(LogLevel.Warning);
    }
});
```

## 6. Infrastructure Recommendations

### 6.1 Database Server Optimization
- **Connection Pooling**: Set appropriate max pool size in connection string
- **Memory Configuration**: Allocate adequate RAM to SQL Server
- **Index Maintenance**: Schedule regular index rebuilding/reorganization
- **Statistics Updates**: Keep statistics current for optimal query plans

### 6.2 Application Server Optimization
- **IIS Configuration**: Optimize application pool settings
- **Memory Limits**: Set appropriate memory limits for your application
- **Garbage Collection**: Consider server GC mode for better throughput

### 6.3 Network Optimization
- **CDN**: Use CDN for static assets (CSS, JS, images)
- **Load Balancing**: Implement load balancing for high availability
- **SSL/TLS**: Use HTTP/2 for better performance

## 7. Code-Specific Optimizations

### 7.1 Lazy Loading vs Eager Loading
```csharp
// Use explicit loading for better control
public async Task<Aman> GetAmanWithDetailsAsync(string id)
{
    var aman = await _context.Amans
        .Include(a => a.Location)
        .Include(a => a.AmanSource)
        .Include(a => a.AmanCorrectionType)
        .AsNoTracking()
        .FirstOrDefaultAsync(a => a.AmanID == id);
        
    return aman;
}
```

### 7.2 Bulk Operations
```csharp
// Use bulk operations for multiple inserts/updates
public async Task BulkInsertGasmonDataAsync(IEnumerable<FSRUData> data)
{
    _context.FSRUData.AddRange(data);
    await _context.SaveChangesAsync();
}
```

## 8. Implementation Priority

### High Priority (Immediate Impact)
1. Add database indexes
2. Implement response caching
3. Add compression
4. Optimize DbContext configuration

### Medium Priority (Significant Impact)
1. Implement distributed caching (Redis)
2. Add health checks
3. Optimize file upload handling
4. Add monitoring/logging

### Low Priority (Long-term Benefits)
1. Implement CDN
2. Add load balancing
3. Optimize garbage collection
4. Database server tuning

## 9. Performance Testing

After implementing optimizations, use these tools to measure improvements:

- **Application Insights**: Monitor application performance
- **SQL Server Profiler**: Analyze database queries
- **dotMemory/PerfView**: Profile memory usage
- **Load Testing**: Use tools like NBomber or Artillery

## 10. Monitoring KPIs

Track these key performance indicators:
- **Response Time**: Average page load time
- **Throughput**: Requests per second
- **Error Rate**: 4xx/5xx error percentage
- **Database Performance**: Query execution time
- **Memory Usage**: Application memory consumption
- **CPU Usage**: Server CPU utilization

This comprehensive optimization strategy should significantly improve your portal-gas application's performance across all layers. 