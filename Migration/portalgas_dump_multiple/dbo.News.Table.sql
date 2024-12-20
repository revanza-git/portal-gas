USE [PortalNR]
GO
/****** Object:  Table [dbo].[News]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[News](
	[NewsID] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [varchar](100) NULL,
	[Content] [text] NULL,
	[Department] [varchar](3) NULL,
	[Author] [varchar](100) NULL,
	[CreatedOn] [datetime] NULL,
	[PublishingDate] [datetime] NULL,
	[Counter] [int] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_News] PRIMARY KEY CLUSTERED 
(
	[NewsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[News] ADD  CONSTRAINT [DF_News_Counter]  DEFAULT ((0)) FOR [Counter]
GO
ALTER TABLE [dbo].[News] ADD  CONSTRAINT [DF_News_Status]  DEFAULT ((0)) FOR [Status]
GO
