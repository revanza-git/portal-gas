USE [PortalNR]
GO
/****** Object:  Table [dbo].[Gm_Parameters]    Script Date: 17/11/2024 02:36:36 ******/
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
