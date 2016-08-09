CREATE PROCEDURE [bp].[sp_BPInstance_TryLockAndUpdateWorkflowInstanceID]	
	@ProcessInstanceID bigint,
	@WorkflowInstanceID uniqueidentifier,
	@CurrentRuntimeProcessID int,
	@RunningProcessIDs varchar(max),
	@Statuses varchar(max)
	
AS
BEGIN
	DECLARE @StatusesTable TABLE ([Status] int)
	INSERT INTO @StatusesTable ([Status])
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@Statuses)
	
	DECLARE @RunningProcessIDsTable TABLE (ID int)
	INSERT INTO @RunningProcessIDsTable (ID)
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@RunningProcessIDs)
	
	DECLARE @IsLocked bit
	SET @IsLocked = 0
    UPDATE bp.BPInstance
    SET	LockedByProcessID = @CurrentRuntimeProcessID,
		@IsLocked = 1,
	    WorkflowInstanceID = @WorkflowInstanceID
	WHERE ID = @ProcessInstanceID
		  AND ExecutionStatus IN (SELECT [Status] FROM @StatusesTable)
		  AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDsTable))
	
	SELECT CONVERT(BIT, @IsLocked)
END