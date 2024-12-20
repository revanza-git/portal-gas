USE [PortalNR]
GO
/****** Object:  Table [dbo].[Gm_LetDown]    Script Date: 17/11/2024 02:36:36 ******/
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
