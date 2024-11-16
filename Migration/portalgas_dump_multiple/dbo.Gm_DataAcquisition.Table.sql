USE [PortalNR]
GO
/****** Object:  Table [dbo].[Gm_DataAcquisition]    Script Date: 17/11/2024 02:36:36 ******/
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
