USE [PortalNR]
GO
/****** Object:  Table [dbo].[Gm_Cargo]    Script Date: 17/11/2024 02:36:36 ******/
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
ALTER TABLE [dbo].[Gm_Cargo] ADD  CONSTRAINT [DF_Gm_Cargo_IsTarget]  DEFAULT ((1)) FOR [IsTarget]
GO
