# Database Migrations

This folder contains Entity Framework Core migrations for the Portal Gas project.

## Migration Files

1. **20241214000000_InitialCreate.cs** - Creates the initial set of tables including:
   - AmanCorrectionTypes, AmanSources, Amans
   - Departments, Locations, Jabatan, JenisPekerjaan
   - DCUs, Atms, Emails, Galleries, Photos, Videos, News
   - Overtime, OvertimeApprovers, Reschedules, PelaporanGratifikasi
   - GCG_CocCoi, GCG_CocCoi_Deleted

2. **20241214000001_AddRemainingTables.cs** - Adds additional tables:
   - Hazards, HSSEReports, NOCs, ObservationLists
   - ClsrLists, UnsafeActionLists, UnsafeConditionLists
   - Responsibles, SemarLevels, SemarTypes, SemarTypeCategories
   - Semars, SemarTemplates, Vendors, Projects, Tras
   - ProjectTasks, Workers, PIC

3. **20241214000002_AddGasMonitoringTables.cs** - Adds Gas Monitoring (Gm_) tables:
   - Gm_Boats, Gm_Vessel, Gm_Line, Gm_Tags, Gm_Cargo
   - Gm_Parameters, Gm_Activities, Gm_CheckingFieldFSRU
   - Gm_DataAcquisition, Gm_FSRUData, Gm_FSRUDataDaily
   - Gm_LetDown, Gm_ORFData, Gm_ORFDataDaily
   - Gm_TUGBoatsData, Gm_VesselData

## How to Apply Migrations

### Using .NET CLI (Recommended)

1. Open a terminal in the project root directory
2. Run the following commands:

```bash
# Add Entity Framework tools (if not already installed)
dotnet tool install --global dotnet-ef

# Apply all migrations to the database
dotnet ef database update

# Or apply a specific migration
dotnet ef database update 20241214000000_InitialCreate
dotnet ef database update 20241214000001_AddRemainingTables
dotnet ef database update 20241214000002_AddGasMonitoringTables
```

### Using Package Manager Console in Visual Studio

1. Open Package Manager Console in Visual Studio
2. Run the following commands:

```powershell
# Apply all migrations
Update-Database

# Or apply a specific migration
Update-Database -Migration 20241214000000_InitialCreate
Update-Database -Migration 20241214000001_AddRemainingTables
Update-Database -Migration 20241214000002_AddGasMonitoringTables
```

## Important Notes

- Make sure your connection string in `appsettings.json` is properly configured
- These migrations will create tables based on the existing database schema
- The Identity tables (ASP.NET Identity) are handled separately by the Identity framework
- Some tables have composite primary keys (e.g., GCG_CocCoi, Gm_Parameters)
- All migrations include proper rollback functionality in the `Down()` method

## Troubleshooting

If you encounter issues:

1. **Check connection string**: Ensure your database connection string is correct
2. **Database permissions**: Make sure the database user has CREATE TABLE permissions
3. **Migration order**: Apply migrations in the correct order (000000, 000001, 000002)
4. **Existing data**: If tables already exist, you may need to drop them or use `--force` flag

## Rollback Migrations

To rollback migrations:

```bash
# Rollback to a specific migration
dotnet ef database update 20241214000001_AddRemainingTables

# Rollback all migrations
dotnet ef database update 0
``` 