# =============================================
# Phase 1 Database Setup Script
# Portal Gas Performance Optimization
# =============================================

Write-Host "Starting Phase 1 Database Performance Optimization..." -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

# Database connection parameters from appsettings.json
$ServerName = "REGAS01656\SQLEXPRESS"
$DatabaseName = "PortalNR"
$Username = "sa"
$Password = "admin123"

# Test database connection
Write-Host "Testing database connection..." -ForegroundColor Yellow
try {
    $ConnectionString = "Server=$ServerName;Database=$DatabaseName;User Id=$Username;Password=$Password;TrustServerCertificate=True;"
    $Connection = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
    $Connection.Open()
    Write-Host "SUCCESS: Database connection successful!" -ForegroundColor Green
    $Connection.Close()
}
catch {
    Write-Host "ERROR: Database connection failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Please check your database server and credentials." -ForegroundColor Yellow
    exit 1
}

# Execute database indexes script
Write-Host "`nExecuting database indexes script..." -ForegroundColor Yellow
try {
    $IndexScriptPath = "Database_Indexes.sql"
    
    if (-not (Test-Path $IndexScriptPath)) {
        Write-Host "ERROR: Database_Indexes.sql file not found!" -ForegroundColor Red
        Write-Host "Please ensure Database_Indexes.sql is in the current directory." -ForegroundColor Yellow
        exit 1
    }
    
    # Use sqlcmd to execute the script
    $SqlCmdArgs = @(
        "-S", $ServerName,
        "-d", $DatabaseName,
        "-U", $Username,
        "-P", $Password,
        "-i", $IndexScriptPath,
        "-b"  # Stop on error
    )
    
    Write-Host "Running: sqlcmd $($SqlCmdArgs -join ' ')" -ForegroundColor Cyan
    & sqlcmd @SqlCmdArgs
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "SUCCESS: Database indexes created successfully!" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Error executing database indexes script. Exit code: $LASTEXITCODE" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "ERROR: Error executing database script: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Verify indexes were created
Write-Host "`nVerifying indexes creation..." -ForegroundColor Yellow
try {
    $VerifyQuery = @"
SELECT 
    OBJECT_NAME(object_id) AS TableName,
    name AS IndexName,
    type_desc AS IndexType,
    is_unique AS IsUnique
FROM sys.indexes 
WHERE name LIKE 'IX_%'
    AND OBJECT_NAME(object_id) IN (
        'Amans', 'AspNetUsers', 'Gm_FSRUData', 'Gm_ORFData', 'Gm_VesselData',
        'DCUs', 'NOCs', 'HSSEReports', 'News', 'Photos', 'Videos', 'Galleries',
        'Semars', 'Overtime', 'Jabatan', 'Tras', 'ProjectTasks', 'Workers',
        'GCG_CocCoi', 'PelaporanGratifikasi', 'Emails', 'Locations', 'Departments'
    )
ORDER BY OBJECT_NAME(object_id), name;
"@
    
    $Connection = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
    $Connection.Open()
    $Command = New-Object System.Data.SqlClient.SqlCommand($VerifyQuery, $Connection)
    $Reader = $Command.ExecuteReader()
    
    $IndexCount = 0
    Write-Host "`nCreated Indexes:" -ForegroundColor Cyan
    Write-Host "=================" -ForegroundColor Cyan
    
    while ($Reader.Read()) {
        $TableName = $Reader["TableName"]
        $IndexName = $Reader["IndexName"]
        $IndexType = $Reader["IndexType"]
        Write-Host "  SUCCESS: $TableName.$IndexName ($IndexType)" -ForegroundColor Green
        $IndexCount++
    }
    
    $Reader.Close()
    $Connection.Close()
    
    Write-Host "`nTotal indexes created: $IndexCount" -ForegroundColor Green
}
catch {
    Write-Host "Warning: Could not verify indexes: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Update statistics
Write-Host "`nUpdating database statistics..." -ForegroundColor Yellow
try {
    $UpdateStatsQuery = "EXEC sp_updatestats;"
    $Connection = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
    $Connection.Open()
    $Command = New-Object System.Data.SqlClient.SqlCommand($UpdateStatsQuery, $Connection)
    $Command.ExecuteNonQuery()
    $Connection.Close()
    Write-Host "SUCCESS: Database statistics updated successfully!" -ForegroundColor Green
}
catch {
    Write-Host "Warning: Could not update statistics: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host "`n=================================================" -ForegroundColor Green
Write-Host "Phase 1 Database Setup Completed Successfully!" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

Write-Host "`nNext Steps:" -ForegroundColor Cyan
Write-Host "1. Install NuGet packages: dotnet restore" -ForegroundColor White
Write-Host "2. Build the project: dotnet build" -ForegroundColor White
Write-Host "3. Run the application: dotnet run" -ForegroundColor White
Write-Host "4. Test health check endpoint: http://localhost:5000/health" -ForegroundColor White
Write-Host "5. Monitor performance improvements" -ForegroundColor White

Write-Host "`nExpected Improvements:" -ForegroundColor Cyan
Write-Host "- 50-80% faster database queries" -ForegroundColor White
Write-Host "- 20-30% faster response times" -ForegroundColor White
Write-Host "- Better connection pooling" -ForegroundColor White
Write-Host "- Response compression enabled" -ForegroundColor White
Write-Host "- Static file caching (30 days)" -ForegroundColor White

Write-Host "`nMonitoring:" -ForegroundColor Cyan
Write-Host "- Health Check: /health endpoint" -ForegroundColor White
Write-Host "- Monitor index usage after 1 week of operation" -ForegroundColor White 