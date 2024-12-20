USE [PortalNR]
GO
/****** Object:  Table [dbo].[Overtime]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Overtime](
	[overtimeID] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NULL,
	[posisi] [int] NULL,
	[Department] [int] NULL,
	[tanggal] [date] NULL,
	[jamMulaiKerja] [time](0) NULL,
	[jamSelesaiKerja] [time](0) NULL,
	[jamAwalLembur] [time](0) NULL,
	[jamAkhirLembur] [time](0) NULL,
	[workDescription] [nvarchar](50) NULL,
	[assigner] [nvarchar](50) NULL,
	[totalHours] [float] NULL,
	[status] [int] NULL,
	[description] [nvarchar](250) NULL,
	[CreationDateTime] [datetime] NULL,
 CONSTRAINT [PK_Overtime] PRIMARY KEY CLUSTERED 
(
	[overtimeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
