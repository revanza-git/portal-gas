using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Admin.Migration
{
    /// <inheritdoc />
    public partial class InitialCreate : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AmanCorrectionTypes
            migrationBuilder.CreateTable(
                name: "AmanCorrectionTypes",
                columns: table => new
                {
                    AmanCorrectionTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmanCorrectionTypes", x => x.AmanCorrectionTypeID);
                });

            // AmanSources
            migrationBuilder.CreateTable(
                name: "AmanSources",
                columns: table => new
                {
                    AmanSourceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmanSources", x => x.AmanSourceID);
                });

            // Departments
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true),
                    Induk = table.Column<string>(type: "varchar(max)", nullable: true),
                    IsDepartment = table.Column<string>(type: "char(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentID);
                });

            // Locations
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationID);
                });

            // Jabatan
            migrationBuilder.CreateTable(
                name: "Jabatan",
                columns: table => new
                {
                    JabatanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JabatanCode = table.Column<string>(type: "varchar(max)", nullable: true),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true),
                    Department = table.Column<int>(type: "int", nullable: false),
                    Atasan = table.Column<string>(type: "varchar(max)", nullable: true),
                    IsDriver = table.Column<bool>(type: "bit", nullable: false),
                    IsSecretary = table.Column<bool>(type: "bit", nullable: false),
                    IsDirector = table.Column<bool>(type: "bit", nullable: false),
                    Hide = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jabatan", x => x.JabatanID);
                });

            // JenisPekerjaan
            migrationBuilder.CreateTable(
                name: "JenisPekerjaan",
                columns: table => new
                {
                    jenis_pekerjaanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JenisPekerjaan", x => x.jenis_pekerjaanID);
                });

            // Amans
            migrationBuilder.CreateTable(
                name: "Amans",
                columns: table => new
                {
                    AmanID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Classification = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Responsible = table.Column<string>(type: "varchar(max)", nullable: true),
                    Source = table.Column<int>(type: "int", nullable: false),
                    CorrectionType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Verifier = table.Column<string>(type: "varchar(max)", nullable: true),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "varchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "varchar(max)", nullable: true),
                    Department = table.Column<string>(type: "varchar(max)", nullable: true),
                    Creator = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "date", nullable: true),
                    OverdueNotif = table.Column<int>(type: "int", nullable: false),
                    Auditors = table.Column<string>(type: "varchar(max)", nullable: true),
                    NOCID = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amans", x => x.AmanID);
                });

            // DCUs
            migrationBuilder.CreateTable(
                name: "DCUs",
                columns: table => new
                {
                    DCUID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Nama = table.Column<string>(type: "varchar(max)", nullable: true),
                    JenisPekerjaan = table.Column<int>(type: "int", nullable: false),
                    Sistole = table.Column<string>(type: "varchar(max)", nullable: true),
                    Diastole = table.Column<string>(type: "varchar(max)", nullable: true),
                    Nadi = table.Column<string>(type: "varchar(max)", nullable: true),
                    Suhu = table.Column<string>(type: "varchar(max)", nullable: true),
                    Keluhan = table.Column<string>(type: "varchar(max)", nullable: true),
                    Foto = table.Column<string>(type: "varchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "varchar(max)", nullable: true),
                    DeskripsiPekerjaan = table.Column<string>(type: "varchar(max)", nullable: true),
                    NamaPerusahaan = table.Column<string>(type: "varchar(max)", nullable: true),
                    Other = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DCUs", x => x.DCUID);
                });

            // Atms
            migrationBuilder.CreateTable(
                name: "Atms",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    SelectedLocationID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FatBody = table.Column<double>(type: "float", nullable: false),
                    FatVisceral = table.Column<double>(type: "float", nullable: false),
                    Keluhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TensionImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeightImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_timestamp = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_timestamp = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atms", x => x.ID);
                });

            // Emails
            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    EmailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Receiver = table.Column<string>(type: "varchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "varchar(max)", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Schedule = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.EmailID);
                });

            // Galleries
            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    GalleryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    Creator = table.Column<string>(type: "varchar(max)", nullable: true),
                    Department = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.GalleryID);
                });

            // Photos
            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    PhotoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Keterangan = table.Column<string>(type: "varchar(max)", nullable: true),
                    GalleryID = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "varchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    Creator = table.Column<string>(type: "varchar(max)", nullable: true),
                    Department = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.PhotoID);
                });

            // Videos
            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    VideoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Keterangan = table.Column<string>(type: "varchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "varchar(max)", nullable: true),
                    GalleryID = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    Creator = table.Column<string>(type: "varchar(max)", nullable: true),
                    Department = table.Column<string>(type: "varchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.VideoID);
                });

            // News
            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    NewsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "varchar(max)", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Department = table.Column<string>(type: "varchar(max)", nullable: true),
                    Author = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    PublishingDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Counter = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.NewsID);
                });

            // Overtime
            migrationBuilder.CreateTable(
                name: "Overtime",
                columns: table => new
                {
                    overtimeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    posisi = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<int>(type: "int", nullable: false),
                    tanggal = table.Column<DateTime>(type: "date", nullable: false),
                    jamMulaiKerja = table.Column<TimeSpan>(type: "time", nullable: false),
                    jamSelesaiKerja = table.Column<TimeSpan>(type: "time", nullable: false),
                    jamAwalLembur = table.Column<TimeSpan>(type: "time", nullable: false),
                    jamAkhirLembur = table.Column<TimeSpan>(type: "time", nullable: false),
                    workDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    assigner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    totalHours = table.Column<double>(type: "float", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Overtime", x => x.overtimeID);
                });

            // OvertimeApprovers
            migrationBuilder.CreateTable(
                name: "OvertimeApprovers",
                columns: table => new
                {
                    OvertimeApproverID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(max)", nullable: true),
                    Department = table.Column<string>(type: "varchar(max)", nullable: true),
                    Role = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeApprovers", x => x.OvertimeApproverID);
                });

            // Reschedules
            migrationBuilder.CreateTable(
                name: "Reschedules",
                columns: table => new
                {
                    RescheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmanID = table.Column<string>(type: "varchar(max)", nullable: true),
                    OldEndDate = table.Column<DateTime>(type: "date", nullable: false),
                    NewEndDate = table.Column<DateTime>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "varchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reschedules", x => x.RescheduleID);
                });

            // PelaporanGratifikasi
            migrationBuilder.CreateTable(
                name: "PelaporanGratifikasi",
                columns: table => new
                {
                    PelaporanGratifikasiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "varchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    AdaPenerimaanGratifikasi = table.Column<int>(type: "int", nullable: false),
                    AdaPemberianGratifikasi = table.Column<int>(type: "int", nullable: false),
                    AdaPermintaanGratifikasi = table.Column<int>(type: "int", nullable: false),
                    DeskripsiPenerimaanGratifikasi = table.Column<string>(type: "text", nullable: true),
                    DeskripsiPemberianGratifikasi = table.Column<string>(type: "text", nullable: true),
                    DeskripsiPermintaanGratifikasi = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PelaporanGratifikasi", x => x.PelaporanGratifikasiID);
                });

            // GCG_CocCoi
            migrationBuilder.CreateTable(
                name: "GCG_CocCoi",
                columns: table => new
                {
                    Year = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "varchar(450)", nullable: false),
                    CoI = table.Column<bool>(type: "bit", nullable: false),
                    CoC = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CoISignedTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CoCSignedTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GCG_CocCoi", x => new { x.Year, x.UserID });
                });

            // GCG_CocCoi_Deleted
            migrationBuilder.CreateTable(
                name: "GCG_CocCoi_Deleted",
                columns: table => new
                {
                    Year = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "varchar(450)", nullable: false),
                    CoI = table.Column<bool>(type: "bit", nullable: false),
                    CoC = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CoISignedTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CoCSignedTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GCG_CocCoi_Deleted", x => new { x.Year, x.UserID });
                });

            // Additional tables will be added in a second migration due to length constraints
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "GCG_CocCoi_Deleted");
            migrationBuilder.DropTable(name: "GCG_CocCoi");
            migrationBuilder.DropTable(name: "PelaporanGratifikasi");
            migrationBuilder.DropTable(name: "Reschedules");
            migrationBuilder.DropTable(name: "OvertimeApprovers");
            migrationBuilder.DropTable(name: "Overtime");
            migrationBuilder.DropTable(name: "News");
            migrationBuilder.DropTable(name: "Videos");
            migrationBuilder.DropTable(name: "Photos");
            migrationBuilder.DropTable(name: "Galleries");
            migrationBuilder.DropTable(name: "Emails");
            migrationBuilder.DropTable(name: "Atms");
            migrationBuilder.DropTable(name: "DCUs");
            migrationBuilder.DropTable(name: "Amans");
            migrationBuilder.DropTable(name: "JenisPekerjaan");
            migrationBuilder.DropTable(name: "Jabatan");
            migrationBuilder.DropTable(name: "Locations");
            migrationBuilder.DropTable(name: "Departments");
            migrationBuilder.DropTable(name: "AmanSources");
            migrationBuilder.DropTable(name: "AmanCorrectionTypes");
        }
    }
} 