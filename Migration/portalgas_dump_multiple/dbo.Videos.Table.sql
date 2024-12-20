USE [PortalNR]
GO
/****** Object:  Table [dbo].[Videos]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Videos](
	[VideoID] [int] IDENTITY(1,1) NOT NULL,
	[Keterangan] [varchar](250) NULL,
	[FileName] [varchar](250) NULL,
	[GalleryID] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[Creator] [varchar](50) NULL,
	[Department] [varchar](3) NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_Videos] PRIMARY KEY CLUSTERED 
(
	[VideoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Videos] ADD  CONSTRAINT [DF_Videos_Status]  DEFAULT ((0)) FOR [Status]
GO
