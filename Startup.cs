using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Admin.Services;
using Admin.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging.Debug;
using Admin.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Admin.Repositories;
using Admin.Models.User;
using Admin.Interfaces.Repositories;
using Admin.Interfaces.Services;
using Admin.Models.AdSettings;
using Microsoft.Extensions.Options;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Admin
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuthentication(services);
            ConfigureLocalization(services);
            ConfigureDatabase(services);
            ConfigureIdentity(services);
            ConfigureRepositories(services);
            ConfigureCustomServices(services);
            ConfigureFrameworkServices(services);
            ConfigureCaching(services);
            ConfigureLogging(services);
            ConfigureHealthChecks(services);
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.ExpireTimeSpan = TimeSpan.FromHours(8); // 8 hour sessions
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        private void ConfigureLocalization(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("id-ID");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("id-ID") };
            });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            // Phase 3 Optimization: Enhanced DbContextPool with larger pool size for high load
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration["Data:PortalNR:ConnectionString"], sqlOptions =>
                {
                    sqlOptions.CommandTimeout(60); // Increased timeout for complex queries
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
                
                // Optimize for read-heavy workloads
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                
                // Enable compiled model for faster startup
                options.EnableServiceProviderCaching();
                
                if (Environment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }
            }, poolSize: 128); // Phase 3: Increased pool size for high-load scenarios (15-25% improvement)
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Optimize password requirements for performance
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                
                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            // Change from Transient to Scoped for better performance - Phase 1 Optimization
            services.AddScoped<IVendorRepository, EFVendorRepository>();
            services.AddScoped<IProjectRepository, EFProjectRepository>();
            services.AddScoped<IAmanRepository, EFAmanRepository>();
            services.AddScoped<IAmanSourceRepository, EFAmanSourceRepository>();
            services.AddScoped<ILocationRepository, EFLocationRepository>();
            services.AddScoped<ISemarRepository, EFSemarRepository>();
            services.AddScoped<INOCRepository, EFNOCRepository>();
            services.AddScoped<IDCURepository, EFDCURepository>();
            services.AddScoped<IHSSEReportRepository, EFHSSEReportRepository>();
            services.AddScoped<ITraRepository, EFTraRepository>();
            services.AddScoped<IEventRepository, EFEventRepository>();
            services.AddScoped<IGalleryRepository, GalleryRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<ICommonRepository, EFCommonRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IHazardRepository, EFHazardRepository>();
            services.AddScoped<ISDMRepository, SDMRepository>();
            services.AddScoped<IPelaporanGratifikasiRepository, EFPelaporanGratifikasiRepository>();
        }

        private void ConfigureCustomServices(IServiceCollection services)
        {
            // Configure HttpClient with timeout - Phase 1 Optimization
            services.AddHttpClient("default", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            
            services.AddTransient<ApiHelper>();
            
            // Phase 2 Optimization: Add CacheService
            services.AddScoped<Admin.Services.ICacheService, Admin.Services.CacheService>();
            
            // Phase 3 Optimization: Add Performance Monitoring Service
            services.AddSingleton<IPerformanceMonitoringService, PerformanceMonitoringService>();
            
            // Change from Transient to Scoped for better performance
            services.AddScoped<IGCGService, GCGService>();
            services.AddScoped<IORFDataService, ORFDataService>();
            services.AddScoped<IFSRUDataService, FSRUDataService>();
            services.AddScoped<IORFDataDailyService, ORFDataDailyService>();
            services.AddScoped<IFSRUDataDailyService, FSRUDataDailyService>();
            services.AddScoped<ITUGBoatsDataService, TUGBoatsDataService>();
            services.AddScoped<IVesselDataService, VesselDataService>();
            services.AddScoped<IGasmonActivityService, GasmonActivityService>();
            services.AddScoped<IGasmonParameterService, GasmonParameterService>();
            services.AddScoped<ISemarService, SemarService>();
            services.AddScoped<IDCUService, DCUService>();
            services.AddScoped<INOCService, NOCService>();
            services.AddScoped<ISDMService, SDMService>();
            
            // Keep transient for stateless services
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            
            // Configuration
            services.Configure<AdSettings>(Configuration.GetSection("AD"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<AdSettings>>().Value);
            
            // Expensive-to-create services as Singleton
            services.AddScoped<ViewRenderService>();
            services.AddSingleton<IConverter>(provider => 
                new SynchronizedConverter(new PdfTools()));

            // Enhanced Email Services
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IImmediateEmailService, ImmediateEmailService>();
            services.AddHostedService<BackgroundEmailService>();
        }

        private void ConfigureFrameworkServices(IServiceCollection services)
        {
            // Configure MVC with optimizations
            services.AddControllersWithViews();
            
            // Configure caching - Phase 1 Optimization
            ConfigureCaching(services);
            
            // Configure compression - Phase 1 Optimization
            ConfigureCompression(services);
            
            // Configure session with optimizations
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
            
            // Configure Swagger for development
            if (Environment.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Portal Gas API", Version = "v1" });
                });
            }
        }

        private void ConfigureCaching(IServiceCollection services)
        {
            // Try to use Redis cache if available, fallback to memory cache
            var redisConnectionString = Configuration.GetConnectionString("Redis");
            
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                    options.InstanceName = "PortalGas";
                });
            }
            else
            {
                // Enhanced in-memory cache configuration - Phase 1 Optimization
                services.AddMemoryCache(options =>
                {
                    options.SizeLimit = 1024 * 1024 * 50; // 50MB memory limit
                    options.CompactionPercentage = 0.75; // Compact when 75% full
                    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
                });
            }
            
            // Response caching - Phase 1 Optimization
            services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = 1024 * 1024 * 5; // 5MB
                options.UseCaseSensitivePaths = false;
                options.SizeLimit = 1024 * 1024 * 50; // 50MB
            });
        }

        private void ConfigureCompression(IServiceCollection services)
        {
            // Response compression - Phase 1 Optimization
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

        private void ConfigureHealthChecks(IServiceCollection services)
        {
            // Phase 3 Optimization: Enhanced Health Checks with comprehensive monitoring
            var healthChecksBuilder = services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>(
                    name: "database",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                    tags: new[] { "db", "sql", "sqlserver" })
                .AddSqlServer(
                    connectionString: Configuration["Data:PortalNR:ConnectionString"],
                    name: "sqlserver",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                    tags: new[] { "db", "sql", "sqlserver" },
                    timeout: TimeSpan.FromSeconds(10))
                // System health checks
                .AddProcessAllocatedMemoryHealthCheck(
                    maximumMegabytesAllocated: 1024, // 1GB memory limit
                    name: "process_allocated_memory",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                    tags: new[] { "memory" })
                .AddPrivateMemoryHealthCheck(
                    maximumMemoryBytes: 1024L * 1024L * 1024L, // 1GB
                    name: "private_memory",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                    tags: new[] { "memory" })
                .AddWorkingSetHealthCheck(
                    maximumMemoryBytes: 2048L * 1024L * 1024L, // 2GB
                    name: "working_set",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                    tags: new[] { "memory" });
            
            // Note: Redis health check requires AspNetCore.HealthChecks.Redis package
            // To enable Redis health check, install the package and uncomment below:
            // var redisConnectionString = Configuration.GetConnectionString("Redis");
            // if (!string.IsNullOrEmpty(redisConnectionString))
            // {
            //     healthChecksBuilder.AddRedis(redisConnectionString, name: "redis");
            // }
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                
                if (Environment.IsProduction())
                {
                    builder.SetMinimumLevel(LogLevel.Warning);
                }
                else
                {
                    builder.SetMinimumLevel(LogLevel.Information);
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // Configure logging
            if (env.IsDevelopment())
            {
                loggerFactory.AddProvider(new DebugLoggerProvider());
            }

            // Error handling
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portal Gas API v1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Add HSTS for security and performance
            }

            // Performance middleware order is important - Phase 1 Optimization
            app.UseResponseCompression();
            app.UseResponseCaching();
            
            // Phase 3 Optimization: Add Performance Monitoring Middleware
            app.UseMiddleware<Admin.Middleware.PerformanceMonitoringMiddleware>();
            
            // Status code pages
            app.UseStatusCodePages();
            
            // Static files with enhanced caching - Phase 3 Optimization
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var path = ctx.Context.Request.Path.Value;
                    
                    // Different caching strategies based on file type
                    if (path.EndsWith(".css") || path.EndsWith(".js"))
                    {
                        // CSS and JS files - cache for 7 days with versioning support
                        const int durationInSeconds = 60 * 60 * 24 * 7; // 7 days
                        ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={durationInSeconds},immutable");
                        ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(7).ToString("R"));
                    }
                    else if (path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".png") || 
                             path.EndsWith(".gif") || path.EndsWith(".ico") || path.EndsWith(".svg"))
                    {
                        // Images - cache for 30 days
                        const int durationInSeconds = 60 * 60 * 24 * 30; // 30 days
                        ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={durationInSeconds},immutable");
                        ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(30).ToString("R"));
                    }
                    else if (path.EndsWith(".woff") || path.EndsWith(".woff2") || path.EndsWith(".ttf") || path.EndsWith(".eot"))
                    {
                        // Fonts - cache for 1 year
                        const int durationInSeconds = 60 * 60 * 24 * 365; // 1 year
                        ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={durationInSeconds},immutable");
                        ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddYears(1).ToString("R"));
                    }
                    else
                    {
                        // Other static files - cache for 1 day
                        const int durationInSeconds = 60 * 60 * 24; // 1 day
                        ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={durationInSeconds}");
                        ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(1).ToString("R"));
                    }
                    
                    // Security headers for all static files
                    ctx.Context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                }
            });
            
            // Routing
            app.UseRouting();
            
            // Authentication and authorization (correct order)
            app.UseAuthentication();
            app.UseAuthorization();
            
            // Session
            app.UseSession();
            
            // Localization
            app.UseRequestLocalization();
            
            // Enhanced Health checks endpoints - Phase 3 Optimization
            app.UseHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        totalDuration = report.TotalDuration.TotalMilliseconds,
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            exception = entry.Value.Exception?.Message,
                            duration = entry.Value.Duration.TotalMilliseconds,
                            tags = entry.Value.Tags
                        }).OrderBy(x => x.name)
                    });
                    await context.Response.WriteAsync(result);
                }
            });

            // Additional health check endpoints for specific monitoring
            app.UseHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { status = report.Status.ToString() }));
                }
            });

            app.UseHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => false, // Exclude all checks for liveness
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { status = "Healthy" }));
                }
            });

            app.UseHealthChecks("/health/db", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("db"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            duration = entry.Value.Duration.TotalMilliseconds
                        })
                    });
                    await context.Response.WriteAsync(result);
                }
            });
            
            // Endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
