USE [PortalNR]
GO
/****** Object:  Table [dbo].[JenisPekerjaan]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JenisPekerjaan](
	[jenis_pekerjaanID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](50) NULL,
 CONSTRAINT [PK_JenisPekerjaan] PRIMARY KEY CLUSTERED 
(
	[jenis_pekerjaanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
