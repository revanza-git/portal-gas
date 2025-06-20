using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Admin.Migration
{
    /// <inheritdoc />
    public partial class AddRemainingTables : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Hazards
            migrationBuilder.CreateTable(
                name: "Hazards",
                columns: table => new
                {
                    HazardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hazards", x => x.HazardID);
                });

            // HSSEReports
            migrationBuilder.CreateTable(
                name: "HSSEReports",
                columns: table => new
                {
                    HSSEReportID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Company = table.Column<string>(type: "varchar(max)", nullable: true),
                    Service = table.Column<string>(type: "varchar(max)", nullable: true),
                    PersonOnBoard = table.Column<int>(type: "int", nullable: false),
                    SafemanHours = table.Column<int>(type: "int", nullable: false),
                    NumberOfFatalityCase = table.Column<int>(type: "int", nullable: false),
                    NumberOfLTICase = table.Column<int>(type: "int", nullable: false),
                    NumberOfMTC = table.Column<int>(type: "int", nullable: false),
                    NumberOfRWC = table.Column<int>(type: "int", nullable: false),
                    NumberOfFirstAid = table.Column<int>(type: "int", nullable: false),
                    NumberOfOilSpill = table.Column<int>(type: "int", nullable: false),
                    NumberOfSafetyMeeting = table.Column<int>(type: "int", nullable: false),
                    DokumentasiSafetyMeeting = table.Column<string>(type: "varchar(max)", nullable: true),
                    NumberOfToolboxMeeting = table.Column<int>(type: "int", nullable: false),
                    DokumentasiToolboxMeeting = table.Column<string>(type: "varchar(max)", nullable: true),
                    NumberOfEmergencyDrill = table.Column<int>(type: "int", nullable: false),
                    DokumentasiEmergencyDrill = table.Column<string>(type: "varchar(max)", nullable: true),
                    NumberOfManagementVisit = table.Column<int>(type: "int", nullable: false),
                    DokumentasiManagementVisit = table.Column<string>(type: "varchar(max)", nullable: true),
                    ReportedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    ReportingDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HSSEReports", x => x.HSSEReportID);
                });

            // NOCs
            migrationBuilder.CreateTable(
                name: "NOCs",
                columns: table => new
                {
                    NOCID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Photo = table.Column<string>(type: "varchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "varchar(max)", nullable: true),
                    Date = table.Column<string>(type: "varchar(max)", nullable: true),
                    Time = table.Column<string>(type: "varchar(max)", nullable: true),
                    Lokasi = table.Column<int>(type: "int", nullable: false),
                    DaftarPengamatan = table.Column<int>(type: "int", nullable: false),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true),
                    Tindakan = table.Column<string>(type: "varchar(max)", nullable: true),
                    Rekomendasi = table.Column<string>(type: "varchar(max)", nullable: true),
                    Prioritas = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    NamaObserver = table.Column<string>(type: "varchar(max)", nullable: true),
                    DivisiObserver = table.Column<string>(type: "varchar(max)", nullable: true),
                    UnsafeAction = table.Column<int>(type: "int", nullable: false),
                    UnsafeCondition = table.Column<int>(type: "int", nullable: false),
                    Clsr = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOCs", x => x.NOCID);
                });

            // ObservationLists
            migrationBuilder.CreateTable(
                name: "ObservationLists",
                columns: table => new
                {
                    ObservationListID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationLists", x => x.ObservationListID);
                });

            // ClsrLists
            migrationBuilder.CreateTable(
                name: "ClsrLists",
                columns: table => new
                {
                    ClsrID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClsrLists", x => x.ClsrID);
                });

            // UnsafeActionLists
            migrationBuilder.CreateTable(
                name: "UnsafeActionLists",
                columns: table => new
                {
                    UnsafeActionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnsafeActionLists", x => x.UnsafeActionID);
                });

            // UnsafeConditionLists
            migrationBuilder.CreateTable(
                name: "UnsafeConditionLists",
                columns: table => new
                {
                    UnsafeConditionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnsafeConditionLists", x => x.UnsafeConditionID);
                });

            // Responsibles
            migrationBuilder.CreateTable(
                name: "Responsibles",
                columns: table => new
                {
                    ResponsibleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsibles", x => x.ResponsibleID);
                });

            // SemarLevels
            migrationBuilder.CreateTable(
                name: "SemarLevels",
                columns: table => new
                {
                    SemarLevelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemarLevels", x => x.SemarLevelID);
                });

            // SemarTypes
            migrationBuilder.CreateTable(
                name: "SemarTypes",
                columns: table => new
                {
                    SemarTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true),
                    Type = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemarTypes", x => x.SemarTypeID);
                });

            // SemarTypeCategories
            migrationBuilder.CreateTable(
                name: "SemarTypeCategories",
                columns: table => new
                {
                    SemarTypeCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemarTypeCategoryCode = table.Column<string>(type: "varchar(max)", nullable: true),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemarTypeCategories", x => x.SemarTypeCategoryID);
                });

            // Semars
            migrationBuilder.CreateTable(
                name: "Semars",
                columns: table => new
                {
                    SemarID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    NoDocument = table.Column<string>(type: "varchar(max)", nullable: true),
                    Title = table.Column<string>(type: "varchar(max)", nullable: true),
                    SemarLevel = table.Column<int>(type: "int", nullable: false),
                    Owner = table.Column<string>(type: "varchar(max)", nullable: true),
                    PublishDate = table.Column<DateTime>(type: "date", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", nullable: true),
                    Revision = table.Column<string>(type: "varchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "varchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "varchar(max)", nullable: true),
                    Classification = table.Column<int>(type: "int", nullable: false),
                    Creator = table.Column<string>(type: "varchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpiredNotification = table.Column<int>(type: "int", nullable: false),
                    Product = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semars", x => x.SemarID);
                });

            // SemarTemplates
            migrationBuilder.CreateTable(
                name: "SemarTemplates",
                columns: table => new
                {
                    SemarTemplateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemarTemplateCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamaTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipeDokumen = table.Column<int>(type: "int", nullable: false),
                    Pengunggah = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Tahun = table.Column<int>(type: "int", nullable: false),
                    Revisi = table.Column<int>(type: "int", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemarTemplates", x => x.SemarTemplateID);
                });

            // Vendors
            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    VendorID = table.Column<string>(type: "varchar(450)", nullable: false),
                    VendorName = table.Column<string>(type: "varchar(max)", nullable: true),
                    Email = table.Column<string>(type: "varchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.VendorID);
                });

            // Projects
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectID = table.Column<string>(type: "varchar(450)", nullable: false),
                    ProjectName = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorID = table.Column<string>(type: "varchar(max)", nullable: true),
                    SponsorPekerjaan = table.Column<string>(type: "varchar(max)", nullable: true),
                    HSSE = table.Column<string>(type: "varchar(max)", nullable: true),
                    PemilikWilayah = table.Column<string>(type: "varchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectID);
                });

            // Tras
            migrationBuilder.CreateTable(
                name: "Tras",
                columns: table => new
                {
                    TraID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Company = table.Column<string>(type: "varchar(max)", nullable: true),
                    Project = table.Column<string>(type: "varchar(max)", nullable: true),
                    PenanggungJawab = table.Column<string>(type: "varchar(max)", nullable: true),
                    Posisi = table.Column<string>(type: "varchar(max)", nullable: true),
                    DocNo = table.Column<string>(type: "varchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TeamLeader = table.Column<string>(type: "varchar(max)", nullable: true),
                    SponsorPekerjaan = table.Column<string>(type: "varchar(max)", nullable: true),
                    HSSE = table.Column<string>(type: "varchar(max)", nullable: true),
                    PimpinanPemilikWilayah = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tras", x => x.TraID);
                });

            // ProjectTasks
            migrationBuilder.CreateTable(
                name: "ProjectTasks",
                columns: table => new
                {
                    ProjectTaskID = table.Column<string>(type: "varchar(450)", nullable: false),
                    TraID = table.Column<string>(type: "varchar(max)", nullable: true),
                    SequenceOfBasicJobSteps = table.Column<string>(type: "varchar(max)", nullable: true),
                    Hazard = table.Column<string>(type: "varchar(max)", nullable: true),
                    Consequence = table.Column<string>(type: "varchar(max)", nullable: true),
                    InitialRisk = table.Column<string>(type: "varchar(max)", nullable: true),
                    RecommendedAction = table.Column<string>(type: "varchar(max)", nullable: true),
                    RoleResponsibility = table.Column<string>(type: "varchar(max)", nullable: true),
                    ResidualRisk = table.Column<string>(type: "varchar(max)", nullable: true),
                    ALARP = table.Column<string>(type: "varchar(max)", nullable: true),
                    AC = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTasks", x => x.ProjectTaskID);
                });

            // Workers
            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    WorkerID = table.Column<string>(type: "varchar(450)", nullable: false),
                    WorkerName = table.Column<string>(type: "varchar(max)", nullable: true),
                    TraID = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.WorkerID);
                });

            // PIC
            migrationBuilder.CreateTable(
                name: "PIC",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usernamePIC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    departmentPIC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    module = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_timestamp = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_timestamp = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PIC", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PIC");
            migrationBuilder.DropTable(name: "Workers");
            migrationBuilder.DropTable(name: "ProjectTasks");
            migrationBuilder.DropTable(name: "Tras");
            migrationBuilder.DropTable(name: "Projects");
            migrationBuilder.DropTable(name: "Vendors");
            migrationBuilder.DropTable(name: "SemarTemplates");
            migrationBuilder.DropTable(name: "Semars");
            migrationBuilder.DropTable(name: "SemarTypeCategories");
            migrationBuilder.DropTable(name: "SemarTypes");
            migrationBuilder.DropTable(name: "SemarLevels");
            migrationBuilder.DropTable(name: "Responsibles");
            migrationBuilder.DropTable(name: "UnsafeConditionLists");
            migrationBuilder.DropTable(name: "UnsafeActionLists");
            migrationBuilder.DropTable(name: "ClsrLists");
            migrationBuilder.DropTable(name: "ObservationLists");
            migrationBuilder.DropTable(name: "NOCs");
            migrationBuilder.DropTable(name: "HSSEReports");
            migrationBuilder.DropTable(name: "Hazards");
        }
    }
} 