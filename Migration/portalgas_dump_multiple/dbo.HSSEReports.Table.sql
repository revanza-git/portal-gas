USE [PortalNR]
GO
/****** Object:  Table [dbo].[HSSEReports]    Script Date: 17/11/2024 02:36:36 ******/
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
ALTER TABLE [dbo].[HSSEReports] ADD  CONSTRAINT [DF_HSSEReports_NumberOfRWC]  DEFAULT ((0)) FOR [NumberOfRWC]
GO
ALTER TABLE [dbo].[HSSEReports] ADD  CONSTRAINT [DF_HSSEReports_Status]  DEFAULT ((1)) FOR [Status]
GO
