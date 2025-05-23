USE [PortalNR]
GO
/****** Object:  Table [dbo].[Semars]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Semars](
	[SemarID] [varchar](10) NOT NULL,
	[Type] [int] NULL,
	[NoDocument] [varchar](250) NULL,
	[Title] [varchar](512) NULL,
	[SemarLevel] [int] NULL,
	[Owner] [varchar](3) NULL,
	[PublishDate] [date] NULL,
	[ExpiredDate] [date] NULL,
	[Description] [varchar](512) NULL,
	[Revision] [varchar](512) NULL,
	[FileName] [varchar](250) NULL,
	[ContentType] [varchar](50) NULL,
	[Classification] [int] NULL,
	[Creator] [varchar](100) NULL,
	[Status] [int] NULL,
	[ExpiredNotification] [int] NULL,
	[Product] [varchar](100) NULL,
 CONSTRAINT [PK_Semars] PRIMARY KEY CLUSTERED 
(
	[SemarID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Semars] ADD  CONSTRAINT [DF_Semars_ExpiredNotification]  DEFAULT ((0)) FOR [ExpiredNotification]
GO
