USE [PortalNR]
GO
/****** Object:  Table [dbo].[DCUs]    Script Date: 17/11/2024 02:36:36 ******/
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
