
CREATE PROCEDURE [bp].[sp_BPInstance_GetPendingsByDefinitionId]
	@DefinitionID int,
	@Statuses varchar(max),
	@NbOrRows int,
	@CurrentRuntimeProcessID int,
	@RunningProcessIDs varchar(max)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @StatusesTable TABLE ([Status] int)
	INSERT INTO @StatusesTable ([Status])
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@Statuses)
	
	DECLARE @RunningProcessIDsTable TABLE (ID int)
	INSERT INTO @RunningProcessIDsTable (ID)
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@RunningProcessIDs)

    SELECT TOP(@NbOrRows) bp.[ID]
	  ,[Title]
      ,[ParentID]
      ,[DefinitionID]
      ,[WorkflowInstanceID]
      ,[InputArgument]
      ,[ExecutionStatus]
      ,[LockedByProcessID]
      ,[LastMessage]
      ,[RetryCount]
      ,[CreatedTime]
      ,[StatusUpdatedTime]
	FROM bp.[BPInstance] bp WITH(NOLOCK)
	JOIN @StatusesTable statuses ON bp.ExecutionStatus = statuses.[Status]
	WHERE
	[DefinitionID] = @DefinitionID
	AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDsTable))
	ORDER BY bp.[ID]
END