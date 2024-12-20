USE [PortalNR]
GO
/****** Object:  Table [dbo].[ProjectTasks]    Script Date: 17/11/2024 02:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectTasks](
	[ProjectTaskID] [varchar](10) NOT NULL,
	[TraID] [varchar](7) NULL,
	[SequenceOfBasicJobSteps] [varchar](512) NULL,
	[Hazard] [varchar](512) NULL,
	[Consequence] [varchar](512) NULL,
	[InitialRisk] [varchar](4) NULL,
	[RecommendedAction] [varchar](512) NULL,
	[RoleResponsibility] [varchar](512) NULL,
	[ResidualRisk] [varchar](4) NULL,
	[ALARP] [varchar](1) NULL,
	[AC] [varchar](1) NULL,
 CONSTRAINT [PK_ProjectTasks] PRIMARY KEY CLUSTERED 
(
	[ProjectTaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
