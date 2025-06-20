using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Admin.Migration
{
    /// <inheritdoc />
    public partial class AddGasMonitoringTables : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Gm_Boats
            migrationBuilder.CreateTable(
                name: "Gm_Boats",
                columns: table => new
                {
                    BoatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_Boats", x => x.BoatID);
                });

            // Gm_Vessel
            migrationBuilder.CreateTable(
                name: "Gm_Vessel",
                columns: table => new
                {
                    VesselID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_Vessel", x => x.VesselID);
                });

            // Gm_Line
            migrationBuilder.CreateTable(
                name: "Gm_Line",
                columns: table => new
                {
                    LineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_Line", x => x.LineID);
                });

            // Gm_Tags
            migrationBuilder.CreateTable(
                name: "Gm_Tags",
                columns: table => new
                {
                    TagID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_Tags", x => x.TagID);
                });

            // Gm_Cargo
            migrationBuilder.CreateTable(
                name: "Gm_Cargo",
                columns: table => new
                {
                    CargoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tahun = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "varchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    IsTarget = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_Cargo", x => x.CargoID);
                });

            // Gm_Parameters
            migrationBuilder.CreateTable(
                name: "Gm_Parameters",
                columns: table => new
                {
                    ParameterID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Tahun = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "varchar(max)", nullable: true),
                    Satuan = table.Column<string>(type: "varchar(max)", nullable: true),
                    Value = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_Parameters", x => new { x.ParameterID, x.Tahun });
                });

            // Gm_Activities
            migrationBuilder.CreateTable(
                name: "Gm_Activities",
                columns: table => new
                {
                    ActivityID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Time = table.Column<string>(type: "varchar(max)", nullable: true),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_Activities", x => x.ActivityID);
                });

            // Gm_CheckingFieldFSRU
            migrationBuilder.CreateTable(
                name: "Gm_CheckingFieldFSRU",
                columns: table => new
                {
                    CheckingFieldID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Time = table.Column<string>(type: "varchar(max)", nullable: true),
                    Tag306_PI_021 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PI_006 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PI_001 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PI_003 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PI_002A = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PI_002B = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DewPoint = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RHA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RHB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LevelWaterA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LevelWaterB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag307_FI_020 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NB_001A = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NB_001 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NB_002 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NB_003 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NB_004 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NB_005 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NB_006 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_CheckingFieldFSRU", x => x.CheckingFieldID);
                });

            // Gm_DataAcquisition
            migrationBuilder.CreateTable(
                name: "Gm_DataAcquisition",
                columns: table => new
                {
                    TagID = table.Column<string>(type: "varchar(450)", nullable: false),
                    Value = table.Column<string>(type: "varchar(max)", nullable: true),
                    LineID = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_DataAcquisition", x => x.TagID);
                });

            // Gm_FSRUData
            migrationBuilder.CreateTable(
                name: "Gm_FSRUData",
                columns: table => new
                {
                    FSRUDataID = table.Column<string>(type: "varchar(450)", nullable: false),
                    FSRUID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Time = table.Column<string>(type: "varchar(max)", nullable: true),
                    Pressure = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Flow1 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Flow2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RobLNG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MMSCF = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrainBoosterPump = table.Column<string>(type: "varchar(max)", nullable: true),
                    Tag306_PI_021 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_TI_003 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PDI_010_A = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PDI_010_B = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag307_FI_020 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag307_TI_001 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag311_V_03 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag2003 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag317_LI_001 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_FSRUData", x => x.FSRUDataID);
                });

            // Gm_FSRUDataDaily
            migrationBuilder.CreateTable(
                name: "Gm_FSRUDataDaily",
                columns: table => new
                {
                    FSRUDataDailyID = table.Column<string>(type: "varchar(450)", nullable: false),
                    FSRUID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Pressure = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LNGTankInventory = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BoFM3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BoFKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveredToORFM3 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveredToORFKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_FSRUDataDaily", x => x.FSRUDataDailyID);
                });

            // Gm_LetDown
            migrationBuilder.CreateTable(
                name: "Gm_LetDown",
                columns: table => new
                {
                    LetDownID = table.Column<string>(type: "varchar(450)", nullable: false),
                    LineID = table.Column<string>(type: "varchar(max)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime", nullable: false),
                    GCA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GCB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GCInUse = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PZI_016 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PIC_015 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PIC_014 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PV_014 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tag306_PV_015 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_LetDown", x => x.LetDownID);
                });

            // Gm_ORFData
            migrationBuilder.CreateTable(
                name: "Gm_ORFData",
                columns: table => new
                {
                    ORFDataID = table.Column<string>(type: "varchar(450)", nullable: false),
                    LineID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Time = table.Column<string>(type: "varchar(max)", nullable: true),
                    VolumeA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VolumeB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VolumeC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FlowA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FlowB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FlowC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Flow = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GHV = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Pressure = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_ORFData", x => x.ORFDataID);
                });

            // Gm_ORFDataDaily
            migrationBuilder.CreateTable(
                name: "Gm_ORFDataDaily",
                columns: table => new
                {
                    ORFDataDailyID = table.Column<string>(type: "varchar(450)", nullable: false),
                    LineID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Pressure = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyNet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HeatingValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyEnergy = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CO2 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ethane = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Methane = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Nitrogen = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Propane = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Water = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    iPentane = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    nPentane = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    iButane = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    nButane = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    nHexane = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_ORFDataDaily", x => x.ORFDataDailyID);
                });

            // Gm_TUGBoatsData
            migrationBuilder.CreateTable(
                name: "Gm_TUGBoatsData",
                columns: table => new
                {
                    TUGBoatsDataID = table.Column<string>(type: "varchar(450)", nullable: false),
                    BoatID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    FuelOilConsumption = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FuelOilROB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_TUGBoatsData", x => x.TUGBoatsDataID);
                });

            // Gm_VesselData
            migrationBuilder.CreateTable(
                name: "Gm_VesselData",
                columns: table => new
                {
                    VesselDataID = table.Column<string>(type: "varchar(450)", nullable: false),
                    VesselID = table.Column<int>(type: "int", nullable: false),
                    CargoID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Position = table.Column<string>(type: "varchar(max)", nullable: true),
                    NextPort = table.Column<string>(type: "varchar(max)", nullable: true),
                    ETANextPort = table.Column<string>(type: "varchar(max)", nullable: true),
                    WindSpeedDirection = table.Column<string>(type: "varchar(max)", nullable: true),
                    CargoQuantityOnBoard = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BoilOff = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BunkerROBHFO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BunkerROBMDO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BunkerROBMGO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConsumpHFO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConsumpMDO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConsumpMGO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gm_VesselData", x => x.VesselDataID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Gm_VesselData");
            migrationBuilder.DropTable(name: "Gm_TUGBoatsData");
            migrationBuilder.DropTable(name: "Gm_ORFDataDaily");
            migrationBuilder.DropTable(name: "Gm_ORFData");
            migrationBuilder.DropTable(name: "Gm_LetDown");
            migrationBuilder.DropTable(name: "Gm_FSRUDataDaily");
            migrationBuilder.DropTable(name: "Gm_FSRUData");
            migrationBuilder.DropTable(name: "Gm_DataAcquisition");
            migrationBuilder.DropTable(name: "Gm_CheckingFieldFSRU");
            migrationBuilder.DropTable(name: "Gm_Activities");
            migrationBuilder.DropTable(name: "Gm_Parameters");
            migrationBuilder.DropTable(name: "Gm_Cargo");
            migrationBuilder.DropTable(name: "Gm_Tags");
            migrationBuilder.DropTable(name: "Gm_Line");
            migrationBuilder.DropTable(name: "Gm_Vessel");
            migrationBuilder.DropTable(name: "Gm_Boats");
        }
    }
} 