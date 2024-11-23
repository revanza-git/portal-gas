using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Admin.Models.Aman;
using Admin.Models.DCU;
using Admin.Models.User;
using Admin.Models.Common;
using Admin.Models.HSSE;
using Admin.Models.Gasmon;
using Admin.Models.Semar;
using Admin.Models.Tugboat;
using Admin.Models.GCG;
using Admin.Models.NOC;
using Admin.Models.Gallery;
using Admin.Models.Overtime;
using Admin.Models.Vendors;
using Admin.Models.Tra;
using Admin.Models.Event;
using Admin.Models.News;

namespace Admin.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<GasmonParameter>()
            .HasKey(c => new { c.ParameterID, c.Tahun });
            
            builder.Entity<CocCoi>()
            .HasKey(c => new { c.Year, c.UserID });
            
        }
        
        public DbSet<DCU> DCUs { get; set; }
        public DbSet<JenisPekerjaan> JenisPekerjaan { get; set; }
     
        public DbSet<ApplicationUser> AspNetUsers { get; set; }
        public DbSet<Aman> Amans { get; set; }
        public DbSet<Semar> Semars { get; set; }
        public DbSet<HSSEReport> HSSEReports { get; set; }
        public DbSet<Tra> Tras { get; set; }
        public DbSet<NOC> NOCs { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ObservationList> ObservationLists { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Responsible> Responsibles { get; set; }
        public DbSet<AmanCorrectionType> AmanCorrectionTypes { get; set; }
        public DbSet<AmanSource> AmanSources { get; set; }
        public DbSet<Reschedule> Reschedules { get; set; }
        public DbSet<SemarLevel> SemarLevels { get; set; }
        public DbSet<SemarType> SemarTypes { get; set; }
        public DbSet<SemarProduct> SemarProducts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Hazard> Hazards { get; set; }
        public DbSet<SemarDownloadHistory> SemarDownloadHistories { get; set; }
        public DbSet<DCUDownloadHistory> DCUDownloadHistories { get; set; }
        public DbSet<PelaporanGratifikasi> PelaporanGratifikasi { get; set; }
        public DbSet<ORFData> ORFData { get; set; }
        public DbSet<FSRUData> FSRUData { get; set; }
        public DbSet<FSRUDataDaily> FSRUDataDaily { get; set; }
        public DbSet<ORFDataDaily> ORFDataDaily { get; set; }
        public DbSet<TUGBoatsData> TUGBoatsData { get; set; }
        public DbSet<Boat> Boats { get; set; }
        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<VesselData> VesselData { get; set; }
        public DbSet<GasmonActivity> GasmonActivity { get; set; }
        public DbSet<GasmonParameter> GasmonParameter { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<CocCoi> CocCois { get; set; }
        public DbSet<ClsrList> ClsrLists { get; set; }
        public DbSet<Overtime> Overtime { get; set; }
        public DbSet<Jabatan> Jabatan { get; set; }
        public DbSet<Atasan> Atasan { get; set; }

    }
}
