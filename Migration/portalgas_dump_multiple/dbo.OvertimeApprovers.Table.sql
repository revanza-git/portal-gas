USE [PortalNR]
GO
/****** Object:  Table [dbo].[OvertimeApprovers]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OvertimeApprovers](
	[OvertimeApproverID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](100) NULL,
	[Department] [varchar](100) NULL,
	[Role] [varchar](100) NULL,
 CONSTRAINT [PK_OvertimeApprovers] PRIMARY KEY CLUSTERED 
(
	[OvertimeApproverID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
