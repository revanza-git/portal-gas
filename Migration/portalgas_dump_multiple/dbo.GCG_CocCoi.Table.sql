USE [PortalNR]
GO
/****** Object:  Table [dbo].[GCG_CocCoi]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GCG_CocCoi](
	[Year] [int] NOT NULL,
	[UserID] [varchar](255) NOT NULL,
	[CoI] [bit] NULL,
	[CoC] [bit] NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdated] [datetime] NULL,
	[CoISignedTime] [datetime] NULL,
	[CoCSignedTime] [datetime] NULL,
 CONSTRAINT [PK_GCG_CoiCoc] PRIMARY KEY CLUSTERED 
(
	[Year] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GCG_CocCoi] ADD  CONSTRAINT [DF_GCG_CoiCoc_CoI]  DEFAULT ((0)) FOR [CoI]
GO
ALTER TABLE [dbo].[GCG_CocCoi] ADD  CONSTRAINT [DF_GCG_CoiCoc_CoC]  DEFAULT ((0)) FOR [CoC]
GO
