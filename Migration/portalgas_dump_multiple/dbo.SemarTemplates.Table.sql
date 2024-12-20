USE [PortalNR]
GO
/****** Object:  Table [dbo].[SemarTemplates]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SemarTemplates](
	[SemarTemplateID] [int] IDENTITY(1,1) NOT NULL,
	[SemarTemplateCode] [nvarchar](50) NULL,
	[NamaTemplate] [nvarchar](100) NULL,
	[FileName] [nvarchar](100) NULL,
	[ContentType] [nvarchar](100) NULL,
	[TipeDokumen] [int] NULL,
	[Pengunggah] [nvarchar](50) NULL,
	[Status] [int] NULL,
	[Tahun] [int] NULL,
	[Revisi] [int] NULL,
	[EntryDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK__SemarTem__E271DD1D5EB779E0] PRIMARY KEY CLUSTERED 
(
	[SemarTemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
