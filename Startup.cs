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
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuthentication(services);
            ConfigureLocalization(services);
            ConfigureDatabase(services);
            ConfigureIdentity(services);
            ConfigureRepositories(services);
            ConfigureCustomServices(services);
            ConfigureFrameworkServices(services);
            ConfigureLogging(services);
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration["Data:PortalNR:ConnectionString"]));
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            services.AddTransient<IVendorRepository, EFVendorRepository>();
            services.AddTransient<IProjectRepository, EFProjectRepository>();
            services.AddTransient<IAmanRepository, EFAmanRepository>();
            services.AddTransient<IAmanSourceRepository, EFAmanSourceRepository>();
            services.AddTransient<ILocationRepository, EFLocationRepository>();
            services.AddTransient<ISemarRepository, EFSemarRepository>();
            services.AddTransient<INOCRepository, EFNOCRepository>();
            services.AddTransient<IDCURepository, EFDCURepository>();
            services.AddTransient<IHSSEReportRepository, EFHSSEReportRepository>();
            services.AddTransient<ITraRepository, EFTraRepository>();
            services.AddTransient<IEventRepository, EFEventRepository>();
            services.AddTransient<IGalleryRepository, GalleryRepository>();
            services.AddTransient<INewsRepository, NewsRepository>();
            services.AddTransient<ICommonRepository, EFCommonRepository>();
            services.AddTransient<IEmailRepository, EmailRepository>();
            services.AddTransient<IHazardRepository, EFHazardRepository>();
            services.AddTransient<ISDMRepository, SDMRepository>();
            services.AddTransient<IPelaporanGratifikasiRepository, EFPelaporanGratifikasiRepository>();
        }

        private void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddHttpClient(); // Register HttpClient
            services.AddTransient<ApiHelper>();
            services.AddTransient<IGCGService, GCGService>();
            services.AddTransient<IORFDataService, ORFDataService>();
            services.AddTransient<IFSRUDataService, FSRUDataService>();
            services.AddTransient<IORFDataDailyService, ORFDataDailyService>();
            services.AddTransient<IFSRUDataDailyService, FSRUDataDailyService>();
            services.AddTransient<ITUGBoatsDataService, TUGBoatsDataService>();
            services.AddTransient<IVesselDataService, VesselDataService>();
            services.AddTransient<IGasmonActivityService, GasmonActivityService>();
            services.AddTransient<IGasmonParameterService, GasmonParameterService>();
            services.AddTransient<ISemarService, SemarService>();
            services.AddTransient<IDCUService, DCUService>();
            services.AddTransient<INOCService, NOCService>();
            services.AddTransient<ISDMService, SDMService>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.Configure<AdSettings>(Configuration.GetSection("AD"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<AdSettings>>().Value); // Register AdSettings
        }

        private void ConfigureFrameworkServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMemoryCache();
            services.AddSession();
            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "PortalNR", Version = "v1" });
            });
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new DebugLoggerProvider());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PortalNR v1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();
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
    }
}
