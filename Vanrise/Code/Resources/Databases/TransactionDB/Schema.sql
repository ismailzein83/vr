
GO
/****** Object:  Schema [bp]    Script Date: 12/17/2014 13:51:38 ******/
CREATE SCHEMA [bp] AUTHORIZATION [dbo]
GO
/****** Object:  Table [bp].[LKUP_ExecutionStatus]    Script Date: 12/17/2014 13:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [bp].[LKUP_ExecutionStatus](
	[ID] [int] NOT NULL,
	[Description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_LKUP_ExecutionStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [bp].[BPDefinition]    Script Date: 12/17/2014 13:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [bp].[BPDefinition](
	[ID] [int] NOT NULL,
	[Title] [varchar](255) NOT NULL,
	[FQTN] [varchar](1000) NOT NULL,
	[Config] [nvarchar](max) NOT NULL,
	[CreatedTime] [datetime] NULL,
 CONSTRAINT [PK_BPDefinition] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [bp].[sp_BPDefinition_GetAll]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE  [bp].[sp_BPDefinition_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   SELECT [ID]
      ,[FQTN]
      ,[Config]
  FROM [bp].[BPDefinition] WITH(NOLOCK)
END
GO
/****** Object:  Table [bp].[BPInstance]    Script Date: 12/17/2014 13:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [bp].[BPInstance](
	[ID] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](1000) NULL,
	[ParentID] [uniqueidentifier] NULL,
	[DefinitionID] [int] NOT NULL,
	[WorkflowInstanceID] [uniqueidentifier] NULL,
	[InputArgument] [nvarchar](max) NULL,
	[ExecutionStatus] [int] NOT NULL,
	[LoadedByRuntime] [bit] NULL,
	[LastMessage] [nvarchar](max) NULL,
	[RetryCount] [int] NULL,
	[CreatedTime] [datetime] NULL,
	[StatusUpdatedTime] [datetime] NULL,
 CONSTRAINT [PK_BPInstance] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_CreatedTime] ON [bp].[BPInstance] 
(
	[CreatedTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_DefinitionID] ON [bp].[BPInstance] 
(
	[DefinitionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_ExecutionStatus] ON [bp].[BPInstance] 
(
	[ExecutionStatus] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_LoadedByRuntime] ON [bp].[BPInstance] 
(
	[LoadedByRuntime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_ParentID] ON [bp].[BPInstance] 
(
	[ParentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [bp].[BPEvent]    Script Date: 12/17/2014 13:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [bp].[BPEvent](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProcessInstanceID] [uniqueidentifier] NULL,
	[Bookmark] [varchar](1000) NOT NULL,
	[Payload] [nvarchar](max) NULL,
	[CreatedTime] [datetime] NULL,
 CONSTRAINT [PK_BPEvent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [bp].[vw_BPInstance]    Script Date: 12/17/2014 13:51:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [bp].[vw_BPInstance]
AS
SELECT     bp.BPInstance.ID, bp.BPInstance.Title, bp.LKUP_ExecutionStatus.Description AS ExecutionStatus, bp.BPDefinition.Title AS Definition, bp.BPInstance.ParentID, 
                      bp.BPInstance.WorkflowInstanceID, bp.BPInstance.InputArgument, bp.BPInstance.LoadedByRuntime, bp.BPInstance.LastMessage, bp.BPInstance.RetryCount, 
                      bp.BPInstance.CreatedTime, bp.BPInstance.StatusUpdatedTime
FROM         bp.BPInstance INNER JOIN
                      bp.BPDefinition ON bp.BPInstance.DefinitionID = bp.BPDefinition.ID LEFT OUTER JOIN
                      bp.LKUP_ExecutionStatus ON bp.BPInstance.ExecutionStatus = bp.LKUP_ExecutionStatus.ID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "BPInstance (bp)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 225
            End
            DisplayFlags = 280
            TopColumn = 8
         End
         Begin Table = "LKUP_ExecutionStatus (bp)"
            Begin Extent = 
               Top = 6
               Left = 263
               Bottom = 95
               Right = 423
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "BPDefinition (bp)"
            Begin Extent = 
               Top = 207
               Left = 331
               Bottom = 326
               Right = 491
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'bp', @level1type=N'VIEW',@level1name=N'vw_BPInstance'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'bp', @level1type=N'VIEW',@level1name=N'vw_BPInstance'
GO
/****** Object:  StoredProcedure [bp].[sp_BPInstance_UpdateWorkflowInstanceID]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_UpdateWorkflowInstanceID]	
	@ID uniqueidentifier,
	@WorkflowInstanceID uniqueidentifier
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	WorkflowInstanceID = @WorkflowInstanceID
	WHERE ID = @ID
	--AND (WorkflowInstanceID IS NULL OR ExecutionStatus > 50) --ExecutionStatus Completed = 50
END
GO
/****** Object:  StoredProcedure [bp].[sp_BPInstance_UpdateStatus]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_UpdateStatus]	
	@ID uniqueidentifier,
	@ExecutionStatus int,
	@Message nvarchar(max),
	@RetryCount int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	ExecutionStatus = @ExecutionStatus,
		StatusUpdatedTime = GETDATE(),
		LastMessage = ISNULL(@Message, LastMessage),
		RetryCount = ISNULL(@RetryCount, RetryCount)
	WHERE ID = @ID
END
GO
/****** Object:  StoredProcedure [bp].[sp_BPInstance_UpdateLoadedFlag]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_UpdateLoadedFlag]
	@ID uniqueidentifier,
	@Loaded bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET LoadedByRuntime = @Loaded
    WHERE ID = @ID
END
GO
/****** Object:  StoredProcedure [bp].[sp_BPInstance_Insert]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_Insert]
	@ID uniqueidentifier,
	@Title nvarchar(1000),
	@ParentID uniqueidentifier,
	@DefinitionID int,
	@InputArguments nvarchar(max),
	@ExecutionStatus int
	
AS
BEGIN
	INSERT INTO [bp].[BPInstance]
           ([ID]
           ,[Title]
           ,[ParentID]
           ,[DefinitionID]
           ,[InputArgument]
           ,[ExecutionStatus]
           ,[StatusUpdatedTime])
     VALUES
           (@ID
           ,@Title
           ,@ParentID
           ,@DefinitionID
           ,@InputArguments
           ,@ExecutionStatus
           ,GETDATE())
END
GO
/****** Object:  StoredProcedure [bp].[sp_BPInstance_GetPendings]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_GetPendings]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [ID]
	  ,[Title]
      ,[ParentID]
      ,[DefinitionID]
      ,[WorkflowInstanceID]
      ,[InputArgument]
      ,[ExecutionStatus]
      ,[LoadedByRuntime]
      ,[LastMessage]
      ,[RetryCount]
      ,[CreatedTime]
      ,[StatusUpdatedTime]
	FROM bp.[BPInstance] WITH(NOLOCK)
	WHERE
	ISNULL(LoadedByRuntime, 0) = 0
	AND ExecutionStatus < 50 --Completed = 50
	ORDER BY ParentID, CreatedTime
END
GO
/****** Object:  StoredProcedure [bp].[sp_BPInstance_ClearLoadedFlag]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_ClearLoadedFlag]
AS
BEGIN
	UPDATE bp.BPInstance
	SET LoadedByRuntime = 0
END
GO
/****** Object:  StoredProcedure [bp].[sp_BPEvent_Insert]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPEvent_Insert]
	@ProcessInstanceID uniqueidentifier,
	@Bookmark varchar(1000),
	@Payload nvarchar(max)
	
AS
BEGIN
	INSERT INTO [bp].[BPEvent]
           ([ProcessInstanceID]
           ,[Bookmark]
           ,[Payload])
     VALUES
           (@ProcessInstanceID
           ,@Bookmark
           ,@Payload)
END
GO
/****** Object:  StoredProcedure [bp].[sp_BPEvent_GetPendings]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPEvent_GetPendings]
AS
BEGIN
	SELECT TOP 1000 e.[ID]
      ,e.[ProcessInstanceID]
      ,i.DefinitionID
      ,e.[Bookmark]
      ,e.[Payload]
  FROM [bp].[BPEvent] e  WITH(NOLOCK)
  JOIN bp.BPInstance i  WITH(NOLOCK) on e.ProcessInstanceID = i.ID
END
GO
/****** Object:  StoredProcedure [bp].[sp_BPEvent_Delete]    Script Date: 12/17/2014 13:51:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPEvent_Delete]
	@ID bigint
	
AS
BEGIN
	DELETE FROM [bp].[BPEvent]
      WHERE ID = @ID
END
GO
/****** Object:  Default [DF_BPDefinition_CreatedTime]    Script Date: 12/17/2014 13:51:37 ******/
ALTER TABLE [bp].[BPDefinition] ADD  CONSTRAINT [DF_BPDefinition_CreatedTime]  DEFAULT (getdate()) FOR [CreatedTime]
GO
/****** Object:  Default [DF_BPEvent_CreatedTime]    Script Date: 12/17/2014 13:51:37 ******/
ALTER TABLE [bp].[BPEvent] ADD  CONSTRAINT [DF_BPEvent_CreatedTime]  DEFAULT (getdate()) FOR [CreatedTime]
GO
/****** Object:  Default [DF_BPInstance_CreatedTime]    Script Date: 12/17/2014 13:51:37 ******/
ALTER TABLE [bp].[BPInstance] ADD  CONSTRAINT [DF_BPInstance_CreatedTime]  DEFAULT (getdate()) FOR [CreatedTime]
GO
/****** Object:  ForeignKey [FK_BPEvent_BPInstance]    Script Date: 12/17/2014 13:51:37 ******/
ALTER TABLE [bp].[BPEvent]  WITH CHECK ADD  CONSTRAINT [FK_BPEvent_BPInstance] FOREIGN KEY([ProcessInstanceID])
REFERENCES [bp].[BPInstance] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [bp].[BPEvent] CHECK CONSTRAINT [FK_BPEvent_BPInstance]
GO
/****** Object:  ForeignKey [FK_BPInstance_BPDefinition]    Script Date: 12/17/2014 13:51:37 ******/
ALTER TABLE [bp].[BPInstance]  WITH CHECK ADD  CONSTRAINT [FK_BPInstance_BPDefinition] FOREIGN KEY([DefinitionID])
REFERENCES [bp].[BPDefinition] ([ID])
GO
ALTER TABLE [bp].[BPInstance] CHECK CONSTRAINT [FK_BPInstance_BPDefinition]
GO
