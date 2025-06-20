-- =============================================
-- Database Performance Indexes for Portal Gas
-- Run this script on your PortalNR database
-- =============================================

USE PortalNR;
GO

-- =============================================
-- CRITICAL INDEXES - Apply these first
-- =============================================

-- Amans table indexes (most frequently queried)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Amans_Status_CreationDateTime')
CREATE NONCLUSTERED INDEX IX_Amans_Status_CreationDateTime 
ON Amans (Status, CreationDateTime DESC)
INCLUDE (AmanID, Findings, Department, Location);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Amans_Department_Status')
CREATE NONCLUSTERED INDEX IX_Amans_Department_Status 
ON Amans (Department, Status)
INCLUDE (AmanID, Findings, CreationDateTime);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Amans_Location_Status')
CREATE NONCLUSTERED INDEX IX_Amans_Location_Status 
ON Amans (Location, Status)
INCLUDE (AmanID, Findings, CreationDateTime);
GO

-- Identity tables optimization
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_Email')
CREATE NONCLUSTERED INDEX IX_AspNetUsers_Email 
ON AspNetUsers (NormalizedEmail)
WHERE NormalizedEmail IS NOT NULL;
GO

-- =============================================
-- GAS MONITORING INDEXES - High volume data
-- =============================================

-- FSRU Data indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Gm_FSRUData_Date_FSRUID')
CREATE NONCLUSTERED INDEX IX_Gm_FSRUData_Date_FSRUID 
ON Gm_FSRUData (Date DESC, FSRUID)
INCLUDE (Time, Pressure, Temperature, Flow1, Flow2);
GO

-- ORF Data indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Gm_ORFData_Date_LineID')
CREATE NONCLUSTERED INDEX IX_Gm_ORFData_Date_LineID 
ON Gm_ORFData (Date DESC, LineID)
INCLUDE (Time, Volume, Flow, Temperature, Pressure);
GO

-- Vessel Data indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Gm_VesselData_Date_VesselID')
CREATE NONCLUSTERED INDEX IX_Gm_VesselData_Date_VesselID 
ON Gm_VesselData (Date DESC, VesselID)
INCLUDE (CargoQuantityOnBoard, Position);
GO

-- Daily data indexes for reporting
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Gm_FSRUDataDaily_Date_FSRUID')
CREATE NONCLUSTERED INDEX IX_Gm_FSRUDataDaily_Date_FSRUID 
ON Gm_FSRUDataDaily (Date DESC, FSRUID)
INCLUDE (Rate, Pressure, Temperature, LNGTankInventory);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Gm_ORFDataDaily_Date_LineID')
CREATE NONCLUSTERED INDEX IX_Gm_ORFDataDaily_Date_LineID 
ON Gm_ORFDataDaily (Date DESC, LineID)
INCLUDE (DailyNet, HeatingValue, DailyEnergy);
GO

-- =============================================
-- HEALTH & SAFETY INDEXES
-- =============================================

-- DCU health monitoring
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DCUs_Date_JenisPekerjaan')
CREATE NONCLUSTERED INDEX IX_DCUs_Date_JenisPekerjaan 
ON DCUs (Date DESC, JenisPekerjaan)
INCLUDE (DCUID, Nama, Sistole, Diastole, Nadi);
GO

-- NOC safety observations
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_NOCs_EntryDate_Status')
CREATE NONCLUSTERED INDEX IX_NOCs_EntryDate_Status 
ON NOCs (EntryDate DESC, Status)
INCLUDE (NOCID, Lokasi, Prioritas, DueDate);
GO

-- HSSE Reports
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_HSSEReports_ReportingDate_Status')
CREATE NONCLUSTERED INDEX IX_HSSEReports_ReportingDate_Status 
ON HSSEReports (ReportingDate DESC, Status)
INCLUDE (HSSEReportID, Company, Service);
GO

-- =============================================
-- CONTENT MANAGEMENT INDEXES
-- =============================================

-- News and content
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_News_Status_PublishingDate')
CREATE NONCLUSTERED INDEX IX_News_Status_PublishingDate 
ON News (Status, PublishingDate DESC)
INCLUDE (NewsID, Subject, Author, Counter);
GO

-- Gallery and media
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Photos_GalleryID_Status')
CREATE NONCLUSTERED INDEX IX_Photos_GalleryID_Status 
ON Photos (GalleryID, Status)
INCLUDE (PhotoID, Keterangan, CreatedOn);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Videos_GalleryID_Status')
CREATE NONCLUSTERED INDEX IX_Videos_GalleryID_Status 
ON Videos (GalleryID, Status)
INCLUDE (VideoID, Keterangan, CreatedOn);
GO

-- Galleries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Galleries_CreatedOn_Department')
CREATE NONCLUSTERED INDEX IX_Galleries_CreatedOn_Department 
ON Galleries (CreatedOn DESC, Department)
INCLUDE (GalleryID, Deskripsi, Creator);
GO

-- =============================================
-- DOCUMENT MANAGEMENT INDEXES
-- =============================================

-- Semar documents
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Semars_Status_PublishDate')
CREATE NONCLUSTERED INDEX IX_Semars_Status_PublishDate 
ON Semars (Status, PublishDate DESC)
INCLUDE (SemarID, Title, Type, SemarLevel, ExpiredDate);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Semars_Type_Classification')
CREATE NONCLUSTERED INDEX IX_Semars_Type_Classification 
ON Semars (Type, Classification)
INCLUDE (SemarID, Title, PublishDate);
GO

-- =============================================
-- HR & ADMINISTRATIVE INDEXES
-- =============================================

-- Overtime management
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Overtime_tanggal_status')
CREATE NONCLUSTERED INDEX IX_Overtime_tanggal_status 
ON Overtime (tanggal DESC, status)
INCLUDE (overtimeID, name, Department, totalHours);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Overtime_Department_tanggal')
CREATE NONCLUSTERED INDEX IX_Overtime_Department_tanggal 
ON Overtime (Department, tanggal DESC)
INCLUDE (overtimeID, name, status, totalHours);
GO

-- Departments and positions
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Jabatan_Department_Hide')
CREATE NONCLUSTERED INDEX IX_Jabatan_Department_Hide 
ON Jabatan (Department, Hide)
INCLUDE (JabatanID, Deskripsi, JabatanCode);
GO

-- =============================================
-- PROJECT MANAGEMENT INDEXES
-- =============================================

-- TRA and project tasks
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tras_Date_Status')
CREATE NONCLUSTERED INDEX IX_Tras_Date_Status 
ON Tras (Date DESC, Status)
INCLUDE (TraID, Company, Project, PenanggungJawab);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectTasks_TraID')
CREATE NONCLUSTERED INDEX IX_ProjectTasks_TraID 
ON ProjectTasks (TraID)
INCLUDE (ProjectTaskID, Hazard, InitialRisk, ResidualRisk);
GO

-- Workers and projects
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Workers_TraID')
CREATE NONCLUSTERED INDEX IX_Workers_TraID 
ON Workers (TraID)
INCLUDE (WorkerID, WorkerName);
GO

-- =============================================
-- COMPLIANCE & GOVERNANCE INDEXES
-- =============================================

-- GCG Compliance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GCG_CocCoi_Year_UserID')
CREATE NONCLUSTERED INDEX IX_GCG_CocCoi_Year_UserID 
ON GCG_CocCoi (Year DESC, UserID)
INCLUDE (CoI, CoC, CreatedOn, LastUpdated);
GO

-- Gratification reporting
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PelaporanGratifikasi_Year_Month')
CREATE NONCLUSTERED INDEX IX_PelaporanGratifikasi_Year_Month 
ON PelaporanGratifikasi (Year DESC, Month DESC)
INCLUDE (UserID, CreatedOn, LastUpdated);
GO

-- =============================================
-- EMAIL & NOTIFICATIONS INDEXES
-- =============================================

-- Email queue
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Emails_Status_Schedule')
CREATE NONCLUSTERED INDEX IX_Emails_Status_Schedule 
ON Emails (Status, Schedule)
INCLUDE (EmailID, Receiver, Subject, CreatedOn);
GO

-- =============================================
-- LOOKUP TABLES INDEXES
-- =============================================

-- Reference data tables (small but frequently joined)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Locations_Deskripsi')
CREATE NONCLUSTERED INDEX IX_Locations_Deskripsi 
ON Locations (Deskripsi);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Departments_Induk_IsDepartment')
CREATE NONCLUSTERED INDEX IX_Departments_Induk_IsDepartment 
ON Departments (Induk, IsDepartment)
INCLUDE (DepartmentID, Deskripsi);
GO

-- =============================================
-- STATISTICS UPDATE
-- =============================================

-- Update statistics for all tables after index creation
EXEC sp_updatestats;
GO

-- =============================================
-- INDEX USAGE MONITORING QUERY
-- Run this periodically to monitor index effectiveness
-- =============================================

/*
SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates,
    s.last_user_seek,
    s.last_user_scan,
    s.last_user_lookup
FROM sys.dm_db_index_usage_stats s
INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
WHERE OBJECTPROPERTY(s.object_id, 'IsUserTable') = 1
    AND s.database_id = DB_ID()
ORDER BY s.user_seeks + s.user_scans + s.user_lookups DESC;
*/

PRINT 'Database indexes created successfully!';
PRINT 'Total indexes created: Check sys.indexes for complete list';
PRINT 'Run the commented monitoring query periodically to check index usage'; 