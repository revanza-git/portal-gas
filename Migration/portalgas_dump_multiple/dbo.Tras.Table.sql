USE [PortalNR]
GO
/****** Object:  Table [dbo].[Tras]    Script Date: 17/11/2024 02:36:36 ******/
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
