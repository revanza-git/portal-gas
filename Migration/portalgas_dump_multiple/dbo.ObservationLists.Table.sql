USE [PortalNR]
GO
/****** Object:  Table [dbo].[ObservationLists]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ObservationLists](
	[ObservationListID] [int] IDENTITY(1,1) NOT NULL,
	[Deskripsi] [varchar](100) NULL,
 CONSTRAINT [PK_ObservationLists] PRIMARY KEY CLUSTERED 
(
	[ObservationListID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
