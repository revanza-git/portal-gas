USE [PortalNR]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[Name] [nvarchar](max) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[Department] [varchar](3) NULL,
	[GCG] [bit] NOT NULL,
	[GCGAdmin] [bit] NOT NULL,
	[CodeOfConduct] [bit] NOT NULL,
	[ConflictOfInterest] [bit] NOT NULL,
	[CodeOfConductDt] [datetime] NULL,
	[ConflictOfInterestDt] [datetime] NULL,
	[NIP] [nvarchar](200) NULL,
	[Jabatan] [int] NULL,
	[IsTkjp] [bit] NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Jabatan]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Jabatan](
	[JabatanID] [int] IDENTITY(1,1) NOT NULL,
	[JabatanCode] [varchar](20) NULL,
	[Deskripsi] [varchar](250) NULL,
	[Department] [int] NULL,
	[Atasan] [varchar](100) NULL,
	[IsDriver] [bit] NULL,
	[IsSecretary] [bit] NULL,
	[IsDirector] [bit] NULL,
	[Hide] [bit] NULL,
 CONSTRAINT [PK_Jabatan] PRIMARY KEY CLUSTERED 
(
	[JabatanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[DepartmentID] [varchar](3) NOT NULL,
	[Deskripsi] [varchar](250) NULL,
	[Induk] [varchar](3) NULL,
	[IsDepartment] [char](1) NOT NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[DepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[Atasan]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Atasan]
AS
SELECT b.Id, b.UserName, b.Name, 
                  CASE WHEN c.Deskripsi LIKE '%Departemen%' THEN 'Kepala ' + c.Deskripsi WHEN c.Deskripsi LIKE '%Divisi%' THEN 'Kepala ' + c.Deskripsi WHEN c.Deskripsi LIKE '%Seksi%' THEN 'Kepala ' + c.Deskripsi ELSE REPLACE(c.Deskripsi, 
                  'Direktorat', 'Direktur') END AS Department, c.DepartmentID
FROM     dbo.Jabatan AS a INNER JOIN
                  dbo.AspNetUsers AS b ON a.Atasan = b.UserName AND a.Atasan = b.UserName INNER JOIN
                  dbo.Departments AS c ON b.Department = c.DepartmentID
GROUP BY b.Id, a.Atasan, b.UserName, b.Name, c.Deskripsi, c.DepartmentID
GO
/****** Object:  Table [dbo].[Overtime]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Overtime](
	[overtimeID] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NULL,
	[posisi] [int] NULL,
	[Department] [int] NULL,
	[tanggal] [date] NULL,
	[jamMulaiKerja] [time](0) NULL,
	[jamSelesaiKerja] [time](0) NULL,
	[jamAwalLembur] [time](0) NULL,
	[jamAkhirLembur] [time](0) NULL,
	[workDescription] [nvarchar](50) NULL,
	[assigner] [nvarchar](50) NULL,
	[totalHours] [float] NULL,
	[status] [int] NULL,
	[description] [nvarchar](250) NULL,
	[CreationDateTime] [datetime] NULL,
 CONSTRAINT [PK_Overtime] PRIMARY KEY CLUSTERED 
(
	[overtimeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[OvertimeView]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OvertimeView]
AS
SELECT CAST(dbo.Overtime.overtimeID AS varchar) AS overtimeID, dbo.Overtime.name, CAST(dbo.Overtime.posisi AS varchar) AS posisi, CAST(dbo.Overtime.Department AS varchar) AS Department, CAST(dbo.Overtime.tanggal AS varchar) 
                  AS tanggal, CAST(dbo.Overtime.jamMulaiKerja AS varchar) AS jamMulaiKerja, CAST(dbo.Overtime.jamSelesaiKerja AS varchar) AS jamSelesaiKerja, CAST(dbo.Overtime.jamAwalLembur AS varchar) AS jamAwalLembur, 
                  CAST(dbo.Overtime.jamAkhirLembur AS varchar) AS jamAkhirLembur, CAST(dbo.Overtime.workDescription AS varchar) AS workDescription, dbo.Atasan.Name AS assigner, CAST(ROUND(dbo.Overtime.totalHours, 2) AS varchar) 
                  AS totalHours, CAST(dbo.Overtime.status AS varchar) AS status, dbo.Overtime.description, CAST(dbo.Overtime.CreationDateTime AS varchar) AS CreationDateTime
FROM     dbo.Overtime INNER JOIN
                  dbo.Atasan ON dbo.Overtime.assigner = dbo.Atasan.UserName
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AmanCorrectionTypes]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AmanCorrectionTypes](
	[AmanCorrectionTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](250) NULL,
 CONSTRAINT [PK_AmanActionTypes] PRIMARY KEY CLUSTERED 
(
	[AmanCorrectionTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Amans]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Amans](
	[AmanID] [varchar](10) NOT NULL,
	[Classification] [int] NOT NULL,
	[EndDate] [date] NULL,
	[Findings] [nvarchar](max) NULL,
	[Location] [int] NOT NULL,
	[Priority] [int] NOT NULL,
	[Recommendation] [nvarchar](max) NULL,
	[Responsible] [varchar](256) NOT NULL,
	[Source] [int] NOT NULL,
	[CorrectionType] [int] NULL,
	[StartDate] [date] NULL,
	[Status] [int] NOT NULL,
	[Verifier] [varchar](256) NULL,
	[Progress] [int] NOT NULL,
	[FileName] [varchar](256) NULL,
	[ContentType] [varchar](256) NULL,
	[Department] [varchar](3) NULL,
	[Creator] [varchar](256) NULL,
	[CreationDateTime] [datetime] NULL,
	[ClosingDate] [date] NULL,
	[OverdueNotif] [int] NULL,
	[Auditors] [varchar](256) NULL,
	[NOCID] [varchar](10) NULL,
 CONSTRAINT [PK_Amans] PRIMARY KEY CLUSTERED 
(
	[AmanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AmanSources]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AmanSources](
	[AmanSourceID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](250) NULL,
 CONSTRAINT [PK_AmanSources] PRIMARY KEY CLUSTERED 
(
	[AmanSourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IPAddress] [nvarchar](max) NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClsrLists]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClsrLists](
	[ClsrID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](100) NULL,
 CONSTRAINT [PK_ClsrLists] PRIMARY KEY CLUSTERED 
(
	[ClsrID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DCUs]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DCUs](
	[DCUID] [varchar](50) NOT NULL,
	[Date] [datetime] NULL,
	[Nama] [varchar](50) NULL,
	[JenisPekerjaan] [int] NULL,
	[Sistole] [varchar](10) NULL,
	[Diastole] [varchar](10) NULL,
	[Nadi] [varchar](10) NULL,
	[Suhu] [varchar](10) NULL,
	[Keluhan] [varchar](1024) NULL,
	[Foto] [varchar](250) NULL,
	[ContentType] [varchar](50) NULL,
	[DeskripsiPekerjaan] [varchar](50) NULL,
	[NamaPerusahaan] [varchar](100) NULL,
	[Other] [bit] NULL,
 CONSTRAINT [PK_DCU] PRIMARY KEY CLUSTERED 
(
	[DCUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Emails]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Emails](
	[EmailID] [int] IDENTITY(1,1) NOT NULL,
	[Receiver] [varchar](250) NULL,
	[Subject] [varchar](250) NULL,
	[Message] [text] NULL,
	[Status] [int] NULL,
	[Schedule] [datetime] NULL,
	[CreatedOn] [datetime] NULL,
 CONSTRAINT [PK_Emails] PRIMARY KEY CLUSTERED 
(
	[EmailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Galleries]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Galleries](
	[GalleryID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](250) NULL,
	[CreatedOn] [datetime] NULL,
	[Creator] [varchar](250) NULL,
	[Department] [varchar](3) NULL,
 CONSTRAINT [PK_Galleries] PRIMARY KEY CLUSTERED 
(
	[GalleryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GCG_CocCoi]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GCG_CocCoi](
	[Year] [int] NOT NULL,
	[UserID] [varchar](255) NOT NULL,
	[CoI] [bit] NULL,
	[CoC] [bit] NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CoISignedTime] [datetime] NULL,
	[CoCSignedTime] [datetime] NULL,
 CONSTRAINT [PK_GCG_CoiCoc] PRIMARY KEY CLUSTERED 
(
	[Year] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GCG_CocCoi_Deleted]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GCG_CocCoi_Deleted](
	[Year] [int] NOT NULL,
	[UserID] [varchar](255) NOT NULL,
	[CoI] [bit] NULL,
	[CoC] [bit] NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CoISignedTime] [datetime] NULL,
	[CoCSignedTime] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_Activities]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_Activities](
	[ActivityID] [varchar](15) NOT NULL,
	[Date] [date] NULL,
	[Time] [varchar](250) NULL,
	[Source] [int] NULL,
	[Remark] [varchar](250) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Gm_Activities] PRIMARY KEY CLUSTERED 
(
	[ActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_Boats]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_Boats](
	[BoatID] [int] NOT NULL,
	[Name] [varchar](100) NULL,
 CONSTRAINT [PK_Gm_Boats] PRIMARY KEY CLUSTERED 
(
	[BoatID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_Cargo]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_Cargo](
	[CargoID] [int] IDENTITY(1,1) NOT NULL,
	[Tahun] [int] NOT NULL,
	[Code] [varchar](50) NULL,
	[Date] [date] NULL,
	[IsTarget] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Gm_Cargo] PRIMARY KEY CLUSTERED 
(
	[CargoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_CheckingFieldFSRU]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_CheckingFieldFSRU](
	[CheckingFieldID] [varchar](14) NOT NULL,
	[Date] [date] NULL,
	[Time] [varchar](5) NULL,
	[Tag306_PI_021] [decimal](18, 3) NULL,
	[Tag306_PI_006] [decimal](18, 3) NULL,
	[Tag306_PI_001] [decimal](18, 3) NULL,
	[Tag306_PI_003] [decimal](18, 3) NULL,
	[Tag306_PI_002A] [decimal](18, 3) NULL,
	[Tag306_PI_002B] [decimal](18, 3) NULL,
	[DewPoint] [decimal](18, 3) NULL,
	[RHA] [decimal](18, 3) NULL,
	[RHB] [decimal](18, 3) NULL,
	[LevelWaterA] [decimal](18, 3) NULL,
	[LevelWaterB] [decimal](18, 3) NULL,
	[Tag307_FI_020] [decimal](18, 3) NULL,
	[NB_001A] [decimal](18, 3) NULL,
	[NB_001] [decimal](18, 3) NULL,
	[NB_002] [decimal](18, 3) NULL,
	[NB_003] [decimal](18, 3) NULL,
	[NB_004] [decimal](18, 3) NULL,
	[NB_005] [decimal](18, 3) NULL,
	[NB_006] [decimal](18, 3) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Gm_CheckingFieldFSRU] PRIMARY KEY CLUSTERED 
(
	[CheckingFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_DataAcquisition]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_DataAcquisition](
	[TagID] [varchar](50) NULL,
	[Value] [varchar](50) NULL,
	[LineID] [int] NULL,
	[CreatedOn] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_FSRUData]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_FSRUData](
	[FSRUDataID] [varchar](14) NOT NULL,
	[FSRUID] [int] NULL,
	[Date] [date] NULL,
	[Time] [varchar](5) NULL,
	[Pressure] [decimal](18, 3) NULL,
	[Temperature] [decimal](18, 3) NULL,
	[Flow1] [decimal](18, 3) NULL,
	[Flow2] [decimal](18, 3) NULL,
	[RobLNG] [decimal](18, 3) NULL,
	[MMSCF] [decimal](18, 3) NULL,
	[TrainBoosterPump] [varchar](100) NULL,
	[Tag306_PI_021] [decimal](18, 3) NULL,
	[Tag306_TI_003] [decimal](18, 3) NULL,
	[Tag306_PDI_010_A] [decimal](18, 3) NULL,
	[Tag306_PDI_010_B] [decimal](18, 3) NULL,
	[Tag307_FI_020] [decimal](18, 3) NULL,
	[Tag307_TI_001] [decimal](18, 3) NULL,
	[Tag311_V_03] [decimal](18, 3) NULL,
	[Tag2003] [decimal](18, 3) NULL,
	[Tag317_LI_001] [decimal](18, 3) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Gm_FSRUData] PRIMARY KEY CLUSTERED 
(
	[FSRUDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_FSRUDataDaily]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_FSRUDataDaily](
	[FSRUDataDailyID] [varchar](10) NOT NULL,
	[FSRUID] [int] NULL,
	[Date] [date] NULL,
	[Rate] [decimal](18, 3) NULL,
	[Pressure] [decimal](18, 3) NULL,
	[Temperature] [decimal](18, 3) NULL,
	[LNGTankInventory] [decimal](18, 3) NULL,
	[BoFM3] [decimal](18, 3) NULL,
	[BoFKg] [decimal](18, 3) NULL,
	[DeliveredToORFM3] [decimal](18, 3) NULL,
	[DeliveredToORFKg] [decimal](18, 3) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](100) NULL,
	[LastUpdatedBy] [varchar](100) NULL,
 CONSTRAINT [PK_Gm_FSRUDataDaily] PRIMARY KEY CLUSTERED 
(
	[FSRUDataDailyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_LetDown]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_LetDown](
	[LetDownID] [varchar](14) NOT NULL,
	[LineID] [varchar](2) NULL,
	[Time] [datetime] NULL,
	[GCA] [decimal](18, 3) NULL,
	[GCB] [decimal](18, 3) NULL,
	[GCInUse] [decimal](18, 3) NULL,
	[Tag306_PZI_016] [decimal](18, 3) NULL,
	[Tag306_PIC_015] [decimal](18, 3) NULL,
	[Tag306_PIC_014] [decimal](18, 3) NULL,
	[Tag306_PV_014] [decimal](18, 3) NULL,
	[Tag306_PV_015] [decimal](18, 3) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdatedBy] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_Line]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_Line](
	[LineID] [int] NOT NULL,
	[Name] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_ORFData]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_ORFData](
	[ORFDataID] [varchar](14) NOT NULL,
	[LineID] [int] NULL,
	[Date] [date] NULL,
	[Time] [varchar](5) NULL,
	[VolumeA] [decimal](18, 3) NULL,
	[VolumeB] [decimal](18, 3) NULL,
	[VolumeC] [decimal](18, 3) NULL,
	[Volume] [decimal](18, 3) NULL,
	[FlowA] [decimal](18, 3) NULL,
	[FlowB] [decimal](18, 3) NULL,
	[FlowC] [decimal](18, 3) NULL,
	[Flow] [decimal](18, 3) NULL,
	[GHV] [decimal](18, 3) NULL,
	[Temperature] [decimal](18, 3) NULL,
	[Pressure] [decimal](18, 3) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Gm_ORFData] PRIMARY KEY CLUSTERED 
(
	[ORFDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_ORFDataDaily]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_ORFDataDaily](
	[ORFDataDailyID] [varchar](10) NOT NULL,
	[LineID] [int] NULL,
	[Date] [date] NULL,
	[Pressure] [decimal](18, 2) NULL,
	[Temperature] [decimal](18, 2) NULL,
	[DailyNet] [decimal](18, 3) NULL,
	[HeatingValue] [decimal](18, 3) NULL,
	[DailyEnergy] [decimal](18, 3) NULL,
	[CO2] [decimal](18, 4) NULL,
	[Ethane] [decimal](18, 4) NULL,
	[Methane] [decimal](18, 4) NULL,
	[Nitrogen] [decimal](18, 4) NULL,
	[Propane] [decimal](18, 4) NULL,
	[Water] [decimal](18, 4) NULL,
	[iPentane] [decimal](18, 4) NULL,
	[nPentane] [decimal](18, 4) NULL,
	[iButane] [decimal](18, 4) NULL,
	[nButane] [decimal](18, 4) NULL,
	[nHexane] [decimal](18, 4) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Gm_ORFDataDaily] PRIMARY KEY CLUSTERED 
(
	[ORFDataDailyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_Parameters]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_Parameters](
	[ParameterID] [varchar](50) NOT NULL,
	[Tahun] [int] NOT NULL,
	[Label] [varchar](100) NULL,
	[Satuan] [varchar](100) NULL,
	[Value] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Gm_Parameters_1] PRIMARY KEY CLUSTERED 
(
	[ParameterID] ASC,
	[Tahun] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_Tags]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_Tags](
	[TagID] [varchar](50) NOT NULL,
	[Description] [varchar](100) NULL,
	[Unit] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_TUGBoatsData]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_TUGBoatsData](
	[TUGBoatsDataID] [varchar](10) NOT NULL,
	[BoatID] [int] NULL,
	[Date] [date] NULL,
	[FuelOilConsumption] [decimal](18, 3) NULL,
	[FuelOilROB] [decimal](18, 3) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](100) NULL,
	[LastUpdatedBy] [varchar](100) NULL,
 CONSTRAINT [PK_Gm_TUGBoatsData] PRIMARY KEY CLUSTERED 
(
	[TUGBoatsDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_Vessel]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_Vessel](
	[VesselID] [int] NOT NULL,
	[Name] [varchar](100) NULL,
 CONSTRAINT [PK_Gm_Vessel] PRIMARY KEY CLUSTERED 
(
	[VesselID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gm_VesselData]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gm_VesselData](
	[VesselDataID] [varchar](10) NOT NULL,
	[VesselID] [int] NULL,
	[CargoID] [int] NOT NULL,
	[Date] [date] NULL,
	[Position] [varchar](100) NULL,
	[NextPort] [varchar](100) NULL,
	[ETANextPort] [varchar](100) NULL,
	[WindSpeedDirection] [varchar](100) NULL,
	[CargoQuantityOnBoard] [decimal](18, 3) NULL,
	[BoilOff] [decimal](18, 3) NULL,
	[BunkerROBHFO] [decimal](18, 1) NULL,
	[BunkerROBMDO] [decimal](18, 1) NULL,
	[BunkerROBMGO] [decimal](18, 1) NULL,
	[ConsumpHFO] [decimal](18, 1) NULL,
	[ConsumpMDO] [decimal](18, 1) NULL,
	[ConsumpMGO] [decimal](18, 1) NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CreatedBy] [varchar](50) NULL,
	[LastUpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Gm_VesselData] PRIMARY KEY CLUSTERED 
(
	[VesselDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hazards]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hazards](
	[HazardID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](250) NULL,
 CONSTRAINT [PK_Hazards] PRIMARY KEY CLUSTERED 
(
	[HazardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HSSEReports]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HSSEReports](
	[HSSEReportID] [varchar](15) NOT NULL,
	[Company] [varchar](10) NULL,
	[Service] [varchar](250) NULL,
	[PersonOnBoard] [int] NULL,
	[SafemanHours] [int] NULL,
	[NumberOfFatalityCase] [int] NULL,
	[NumberOfLTICase] [int] NULL,
	[NumberOfMTC] [int] NULL,
	[NumberOfRWC] [int] NOT NULL,
	[NumberOfFirstAid] [int] NULL,
	[NumberOfOilSpill] [int] NULL,
	[NumberOfSafetyMeeting] [int] NULL,
	[DokumentasiSafetyMeeting] [varchar](100) NULL,
	[NumberOfToolboxMeeting] [int] NULL,
	[DokumentasiToolboxMeeting] [varchar](100) NULL,
	[NumberOfEmergencyDrill] [int] NULL,
	[DokumentasiEmergencyDrill] [varchar](100) NULL,
	[NumberOfManagementVisit] [int] NULL,
	[DokumentasiManagementVisit] [varchar](100) NULL,
	[ReportedBy] [varchar](50) NULL,
	[ReportingDate] [datetime] NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_HSSEReports] PRIMARY KEY CLUSTERED 
(
	[HSSEReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JenisPekerjaan]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JenisPekerjaan](
	[jenis_pekerjaanID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](50) NULL,
 CONSTRAINT [PK_JenisPekerjaan] PRIMARY KEY CLUSTERED 
(
	[jenis_pekerjaanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Locations]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Locations](
	[LocationID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](250) NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[LocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[News]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[News](
	[NewsID] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [varchar](100) NULL,
	[Content] [text] NULL,
	[Department] [varchar](3) NULL,
	[Author] [varchar](100) NULL,
	[CreatedOn] [datetime] NULL,
	[PublishingDate] [datetime] NULL,
	[Counter] [int] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_News] PRIMARY KEY CLUSTERED 
(
	[NewsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NOCs]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NOCs](
	[NOCID] [varchar](10) NOT NULL,
	[Photo] [varchar](250) NULL,
	[ContentType] [varchar](50) NULL,
	[Date] [varchar](20) NULL,
	[Time] [varchar](10) NULL,
	[Lokasi] [int] NULL,
	[DaftarPengamatan] [int] NULL,
	[Deskripsi] [varchar](1024) NULL,
	[Tindakan] [varchar](1024) NULL,
	[Rekomendasi] [varchar](1024) NULL,
	[Prioritas] [int] NULL,
	[Status] [int] NULL,
	[EntryDate] [datetime] NULL,
	[DueDate] [datetime] NULL,
	[NamaObserver] [varchar](30) NULL,
	[DivisiObserver] [varchar](3) NULL,
	[UnsafeAction] [int] NULL,
	[UnsafeCondition] [int] NULL,
	[Clsr] [int] NULL,
 CONSTRAINT [PK_NOCs] PRIMARY KEY CLUSTERED 
(
	[NOCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ObservationLists]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ObservationLists](
	[ObservationListID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](100) NULL,
 CONSTRAINT [PK_ObservationLists] PRIMARY KEY CLUSTERED 
(
	[ObservationListID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OvertimeApprovers]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OvertimeApprovers](
	[OvertimeApproverID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](100) NULL,
	[Department] [varchar](100) NULL,
	[Role] [varchar](100) NULL,
 CONSTRAINT [PK_OvertimeApprovers] PRIMARY KEY CLUSTERED 
(
	[OvertimeApproverID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PelaporanGratifikasi]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PelaporanGratifikasi](
	[PelaporanGratifikasiID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [varchar](225) NULL,
	[Year] [int] NULL,
	[Month] [int] NULL,
	[AdaPenerimaanGratifikasi] [int] NOT NULL,
	[AdaPemberianGratifikasi] [int] NOT NULL,
	[AdaPermintaanGratifikasi] [int] NOT NULL,
	[DeskripsiPenerimaanGratifikasi] [text] NULL,
	[DeskripsiPemberianGratifikasi] [text] NULL,
	[DeskripsiPermintaanGratifikasi] [text] NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
 CONSTRAINT [PK_PelaporanGratifikasi] PRIMARY KEY CLUSTERED 
(
	[PelaporanGratifikasiID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Photos]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Photos](
	[PhotoID] [int] IDENTITY(1,1) NOT NULL,
	[Keterangan] [varchar](250) NULL,
	[GalleryID] [int] NULL,
	[FileName] [varchar](250) NULL,
	[Status] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[Creator] [varchar](250) NULL,
	[Department] [varchar](3) NULL,
 CONSTRAINT [PK_Photos] PRIMARY KEY CLUSTERED 
(
	[PhotoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Projects]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Projects](
	[ProjectID] [varchar](7) NOT NULL,
	[ProjectName] [varchar](250) NULL,
	[VendorID] [varchar](4) NULL,
	[SponsorPekerjaan] [varchar](250) NULL,
	[HSSE] [varchar](250) NULL,
	[PemilikWilayah] [varchar](250) NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectTasks]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectTasks](
	[ProjectTaskID] [varchar](10) NOT NULL,
	[TraID] [varchar](7) NULL,
	[SequenceOfBasicJobSteps] [varchar](512) NULL,
	[Hazard] [varchar](512) NULL,
	[Consequence] [varchar](512) NULL,
	[InitialRisk] [varchar](4) NULL,
	[RecommendedAction] [varchar](512) NULL,
	[RoleResponsibility] [varchar](512) NULL,
	[ResidualRisk] [varchar](4) NULL,
	[ALARP] [varchar](1) NULL,
	[AC] [varchar](1) NULL,
 CONSTRAINT [PK_ProjectTasks] PRIMARY KEY CLUSTERED 
(
	[ProjectTaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reschedules]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reschedules](
	[RescheduleID] [int] IDENTITY(1,1) NOT NULL,
	[AmanID] [varchar](10) NULL,
	[OldEndDate] [date] NULL,
	[NewEndDate] [date] NULL,
	[Reason] [varchar](250) NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Reschedules] PRIMARY KEY CLUSTERED 
(
	[RescheduleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Responsibles]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Responsibles](
	[ResponsibleID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](250) NULL,
 CONSTRAINT [PK_Responsibles] PRIMARY KEY CLUSTERED 
(
	[ResponsibleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SemarLevels]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SemarLevels](
	[SemarLevelID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](100) NULL,
 CONSTRAINT [PK_SemarLevel] PRIMARY KEY CLUSTERED 
(
	[SemarLevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Semars]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Semars](
	[SemarID] [varchar](10) NOT NULL,
	[Type] [int] NULL,
	[NoDocument] [varchar](250) NULL,
	[Title] [varchar](512) NULL,
	[SemarLevel] [int] NULL,
	[Owner] [varchar](3) NULL,
	[PublishDate] [date] NULL,
	[ExpiredDate] [date] NULL,
	[Description] [varchar](512) NULL,
	[Revision] [varchar](512) NULL,
	[FileName] [varchar](250) NULL,
	[ContentType] [varchar](50) NULL,
	[Classification] [int] NULL,
	[Creator] [varchar](100) NULL,
	[Status] [int] NULL,
	[ExpiredNotification] [int] NULL,
	[Product] [varchar](100) NULL,
 CONSTRAINT [PK_Semars] PRIMARY KEY CLUSTERED 
(
	[SemarID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SemarTemplates]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SemarTemplates](
	[SemarTemplateID] [int] IDENTITY(1,1) NOT NULL,
	[SemarTemplateCode] [nvarchar](50) NULL,
	[NamaTemplate] [nvarchar](100) NULL,
	[FileName] [nvarchar](100) NULL,
	[ContentType] [nvarchar](100) NULL,
	[TipeDokumen] [int] NULL,
	[Pengunggah] [nvarchar](50) NULL,
	[Status] [int] NULL,
	[Tahun] [int] NULL,
	[Revisi] [int] NULL,
	[EntryDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK__SemarTem__E271DD1D5EB779E0] PRIMARY KEY CLUSTERED 
(
	[SemarTemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SemarTypes]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SemarTypes](
	[SemarTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](100) NULL,
	[Type] [varchar](100) NULL,
 CONSTRAINT [PK_SemarType] PRIMARY KEY CLUSTERED 
(
	[SemarTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tras]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tras](
	[TraID] [varchar](7) NOT NULL,
	[Company] [varchar](4) NULL,
	[Project] [varchar](7) NULL,
	[PenanggungJawab] [varchar](250) NULL,
	[Posisi] [varchar](250) NULL,
	[DocNo] [varchar](250) NULL,
	[Date] [date] NULL,
	[Status] [int] NULL,
	[TeamLeader] [varchar](250) NULL,
	[SponsorPekerjaan] [varchar](250) NULL,
	[HSSE] [varchar](250) NULL,
	[PimpinanPemilikWilayah] [varchar](250) NULL,
 CONSTRAINT [PK_Tras] PRIMARY KEY CLUSTERED 
(
	[TraID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnsafeActionLists]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnsafeActionLists](
	[UnsafeActionID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](100) NULL,
 CONSTRAINT [PK_UnsafeActionLists] PRIMARY KEY CLUSTERED 
(
	[UnsafeActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnsafeConditionLists]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnsafeConditionLists](
	[UnsafeConditionID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](100) NULL,
 CONSTRAINT [PK_UnsafeCondition] PRIMARY KEY CLUSTERED 
(
	[UnsafeConditionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendors](
	[VendorID] [varchar](4) NOT NULL,
	[VendorName] [varchar](250) NULL,
	[Email] [varchar](100) NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED 
(
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Videos]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Videos](
	[VideoID] [int] IDENTITY(1,1) NOT NULL,
	[Keterangan] [varchar](250) NULL,
	[FileName] [varchar](250) NULL,
	[GalleryID] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[Creator] [varchar](50) NULL,
	[Department] [varchar](3) NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Videos] PRIMARY KEY CLUSTERED 
(
	[VideoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Workers]    Script Date: 17/11/2024 02:41:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Workers](
	[WorkerID] [varchar](10) NOT NULL,
	[WorkerName] [varchar](250) NULL,
	[TraID] [varchar](7) NULL,
 CONSTRAINT [PK_Workers] PRIMARY KEY CLUSTERED 
(
	[WorkerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Amans] ADD  CONSTRAINT [DF_Amans_Classification]  DEFAULT ((1)) FOR [Classification]
GO
ALTER TABLE [dbo].[Amans] ADD  CONSTRAINT [DF_Amans_Priority]  DEFAULT ((1)) FOR [Priority]
GO
ALTER TABLE [dbo].[Amans] ADD  CONSTRAINT [DF_Amans_Responsible]  DEFAULT ((1)) FOR [Responsible]
GO
ALTER TABLE [dbo].[Amans] ADD  CONSTRAINT [DF_Amans_Progress]  DEFAULT ((0)) FOR [Progress]
GO
ALTER TABLE [dbo].[Amans] ADD  CONSTRAINT [DF_Amans_OverdueNotif]  DEFAULT ((0)) FOR [OverdueNotif]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  CONSTRAINT [DF_AspNetUsers_GCG]  DEFAULT ((1)) FOR [GCG]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  CONSTRAINT [DF_AspNetUsers_GCGAdmin]  DEFAULT ((0)) FOR [GCGAdmin]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  CONSTRAINT [DF_AspNetUsers_CodeOfConduct]  DEFAULT ((0)) FOR [CodeOfConduct]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  CONSTRAINT [DF_AspNetUsers_ConflictOfInterest]  DEFAULT ((0)) FOR [ConflictOfInterest]
GO
ALTER TABLE [dbo].[Emails] ADD  CONSTRAINT [DF_Emails_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[GCG_CocCoi] ADD  CONSTRAINT [DF_GCG_CoiCoc_CoI]  DEFAULT ((0)) FOR [CoI]
GO
ALTER TABLE [dbo].[GCG_CocCoi] ADD  CONSTRAINT [DF_GCG_CoiCoc_CoC]  DEFAULT ((0)) FOR [CoC]
GO
ALTER TABLE [dbo].[Gm_Cargo] ADD  CONSTRAINT [DF_Gm_Cargo_IsTarget]  DEFAULT ((1)) FOR [IsTarget]
GO
ALTER TABLE [dbo].[Gm_VesselData] ADD  CONSTRAINT [DF_Gm_VesselData_CargoID]  DEFAULT ((0)) FOR [CargoID]
GO
ALTER TABLE [dbo].[HSSEReports] ADD  CONSTRAINT [DF_HSSEReports_NumberOfRWC]  DEFAULT ((0)) FOR [NumberOfRWC]
GO
ALTER TABLE [dbo].[HSSEReports] ADD  CONSTRAINT [DF_HSSEReports_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[News] ADD  CONSTRAINT [DF_News_Counter]  DEFAULT ((0)) FOR [Counter]
GO
ALTER TABLE [dbo].[News] ADD  CONSTRAINT [DF_News_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[PelaporanGratifikasi] ADD  CONSTRAINT [DF_PelaporanGratifikasi_AdaPenerimaanGratifikasi]  DEFAULT ((0)) FOR [AdaPenerimaanGratifikasi]
GO
ALTER TABLE [dbo].[PelaporanGratifikasi] ADD  CONSTRAINT [DF_PelaporanGratifikasi_AdaPemberianGratifikasi]  DEFAULT ((0)) FOR [AdaPemberianGratifikasi]
GO
ALTER TABLE [dbo].[PelaporanGratifikasi] ADD  CONSTRAINT [DF_PelaporanGratifikasi_AdaPermintaanGratifikasi]  DEFAULT ((0)) FOR [AdaPermintaanGratifikasi]
GO
ALTER TABLE [dbo].[Photos] ADD  CONSTRAINT [DF_Photos_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Semars] ADD  CONSTRAINT [DF_Semars_ExpiredNotification]  DEFAULT ((0)) FOR [ExpiredNotification]
GO
ALTER TABLE [dbo].[Videos] ADD  CONSTRAINT [DF_Videos_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[22] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "a"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 331
               Right = 242
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "b"
            Begin Extent = 
               Top = 12
               Left = 356
               Bottom = 340
               Right = 616
            End
            DisplayFlags = 280
            TopColumn = 12
         End
         Begin Table = "c"
            Begin Extent = 
               Top = 7
               Left = 664
               Bottom = 170
               Right = 858
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 4212
         Width = 1200
         Width = 1200
         Width = 6324
         Width = 2508
         Width = 3300
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1356
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Atasan'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Atasan'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Overtime"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 429
               Right = 264
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Atasan"
            Begin Extent = 
               Top = 7
               Left = 312
               Bottom = 170
               Right = 506
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 18
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 2028
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 2688
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OvertimeView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OvertimeView'
GO
