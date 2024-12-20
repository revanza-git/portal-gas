USE [PortalNR]
GO
/****** Object:  Table [dbo].[Gm_TUGBoatsData]    Script Date: 17/11/2024 02:36:36 ******/
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
