USE [PortalNR]
GO
/****** Object:  Table [dbo].[Gm_Tags]    Script Date: 17/11/2024 02:36:36 ******/
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
