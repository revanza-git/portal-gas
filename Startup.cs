using Microsoft.AspNetCore.Identity; // Add this using directive
using Microsoft.EntityFrameworkCore; // Add this using directive
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Admin.Models;
using Admin.Services;
using Admin.Data;
using Admin.Implementations;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging.Debug;
using Admin.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using Microsoft.OpenApi.Models; // Add this using directive

namespace Admin
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
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

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("id-ID");
                options.SupportedCultures = [new CultureInfo("id-ID")];
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                Configuration["Data:PortalNR:ConnectionString"]));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddHttpClient();
            services.AddTransient<ApiHelper>();
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

            // Service fo GCG
            services.AddTransient<IGCGService, GCGService>();
            services.AddTransient<IPelaporanGratifikasiRepository, EFPelaporanGratifikasiRepository>();

            // Service for Gas Monitoring
            services.AddTransient<IORFDataService, ORFDataService>();
            services.AddTransient<IFSRUDataService, FSRUDataService>();
            services.AddTransient<IORFDataDailyService, ORFDataDailyService>();
            services.AddTransient<IFSRUDataDailyService, FSRUDataDailyService>();
            services.AddTransient<ITUGBoatsDataService, TUGBoatsDataService>();
            services.AddTransient<IVesselDataService, VesselDataService>();
            services.AddTransient<IGasmonActivityService, GasmonActivityService>();
            services.AddTransient<IGasmonParameterService, GasmonParameterService>();

            // Service for Semar
            services.AddTransient<ISemarService, SemarService>();

            // Service for DCU
            services.AddTransient<IDCUService, DCUService>();

            // Service for NOC
            services.AddTransient<INOCService, NOCService>();

            // Service for Overtime
            services.AddTransient<ISDMService, SDMService>();

            // Add framework services.
            services.AddControllersWithViews();

            // Add Session Service
            services.AddMemoryCache();
            services.AddSession();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "PortalNR", Version = "v1" });
            });

            // Add logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // Replace the obsolete AddDebug method
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

            //app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseAuthentication(); // Replace app.UseIdentity() with app.UseAuthentication()
            app.UseSession();
            app.UseRequestLocalization();
            app.UseRouting();

            // Add the authorization middleware
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
