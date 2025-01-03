USE [PortalNR]
GO
/****** Object:  Table [dbo].[Projects]    Script Date: 17/11/2024 02:36:36 ******/
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
