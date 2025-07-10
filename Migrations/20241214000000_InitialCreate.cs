using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Admin.Migrations
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
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemarTypes", x => x.SemarTypeID);
                });

            // SemarProducts
            migrationBuilder.CreateTable(
                name: "SemarProducts",
                columns: table => new
                {
                    SemarProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Deskripsi = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemarProducts", x => x.SemarProductID);
                });

            // Semars
            migrationBuilder.CreateTable(
                name: "Semars",
                columns: table => new
                {
                    SemarID = table.Column<string>(type: "varchar(450)", nullable: false),
                    SemarType = table.Column<int>(type: "int", nullable: false),
                    SemarLevel = table.Column<int>(type: "int", nullable: false),
                    SemarProduct = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    Title = table.Column<string>(type: "varchar(max)", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Responsible = table.Column<string>(type: "varchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "varchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "varchar(max)", nullable: true),
                    Department = table.Column<string>(type: "varchar(max)", nullable: true),
                    Creator = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    OverdueNotif = table.Column<int>(type: "int", nullable: false),
                    NOCID = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semars", x => x.SemarID);
                });

            // Vendors
            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    VendorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorName = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorType = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorAddress = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorEmail = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorPhone = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorFax = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorContact = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorContactPhone = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorContactEmail = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorDescription = table.Column<string>(type: "varchar(max)", nullable: true),
                    VendorStatus = table.Column<int>(type: "int", nullable: false)
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
                    ProjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "varchar(max)", nullable: true),
                    ProjectDescription = table.Column<string>(type: "varchar(max)", nullable: true),
                    ProjectStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    ProjectEndDate = table.Column<DateTime>(type: "date", nullable: false),
                    ProjectStatus = table.Column<int>(type: "int", nullable: false),
                    VendorID = table.Column<int>(type: "int", nullable: false),
                    ProjectManager = table.Column<string>(type: "varchar(max)", nullable: true),
                    ProjectBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjectActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjectProgress = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectID);
                });

            // ProjectTasks
            migrationBuilder.CreateTable(
                name: "ProjectTasks",
                columns: table => new
                {
                    TaskID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskName = table.Column<string>(type: "varchar(max)", nullable: true),
                    TaskDescription = table.Column<string>(type: "varchar(max)", nullable: true),
                    TaskStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    TaskEndDate = table.Column<DateTime>(type: "date", nullable: false),
                    TaskStatus = table.Column<int>(type: "int", nullable: false),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    TaskAssignee = table.Column<string>(type: "varchar(max)", nullable: true),
                    TaskProgress = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTasks", x => x.TaskID);
                });

            // Workers
            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    WorkerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkerName = table.Column<string>(type: "varchar(max)", nullable: true),
                    WorkerPosition = table.Column<string>(type: "varchar(max)", nullable: true),
                    WorkerDepartment = table.Column<string>(type: "varchar(max)", nullable: true),
                    WorkerEmail = table.Column<string>(type: "varchar(max)", nullable: true),
                    WorkerPhone = table.Column<string>(type: "varchar(max)", nullable: true),
                    WorkerStatus = table.Column<int>(type: "int", nullable: false),
                    VendorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.WorkerID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AmanCorrectionTypes");
            migrationBuilder.DropTable(name: "AmanSources");
            migrationBuilder.DropTable(name: "Amans");
            migrationBuilder.DropTable(name: "Atms");
            migrationBuilder.DropTable(name: "DCUs");
            migrationBuilder.DropTable(name: "Departments");
            migrationBuilder.DropTable(name: "Emails");
            migrationBuilder.DropTable(name: "Jabatan");
            migrationBuilder.DropTable(name: "JenisPekerjaan");
            migrationBuilder.DropTable(name: "Locations");
            migrationBuilder.DropTable(name: "Projects");
            migrationBuilder.DropTable(name: "ProjectTasks");
            migrationBuilder.DropTable(name: "Responsibles");
            migrationBuilder.DropTable(name: "SemarLevels");
            migrationBuilder.DropTable(name: "SemarProducts");
            migrationBuilder.DropTable(name: "Semars");
            migrationBuilder.DropTable(name: "SemarTypes");
            migrationBuilder.DropTable(name: "Vendors");
            migrationBuilder.DropTable(name: "Workers");
        }
    }
} 