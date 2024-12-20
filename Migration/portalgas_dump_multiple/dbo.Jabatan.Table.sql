USE [PortalNR]
GO
/****** Object:  Table [dbo].[Jabatan]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Jabatan](
	[JabatanID] [int] IDENTITY(1,1) NOT NULL,
	[JabatanCode] [varchar](20) NULL,
	[Deskripsi] [varchar](250) NULL,
	[Department] [int] NULL,
	[Atasan] [varchar](100) NULL,
	[IsDriver] [bit] NULL,
	[IsSecretary] [bit] NULL,
	[IsDirector] [bit] NULL,
	[Hide] [bit] NULL,
 CONSTRAINT [PK_Jabatan] PRIMARY KEY CLUSTERED 
(
	[JabatanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
