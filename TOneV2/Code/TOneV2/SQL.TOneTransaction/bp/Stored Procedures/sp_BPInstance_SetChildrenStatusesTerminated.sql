
CREATE PROCEDURE [bp].[sp_BPInstance_SetChildrenStatusesTerminated]		
	@TerminatedStatus int,
	@OpenStatuses varchar(max),
	@RunningProcessIDs varchar(max)
AS
BEGIN
	DECLARE @OpenStatusesTable TABLE ([Status] int)
	INSERT INTO @OpenStatusesTable ([Status])
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@OpenStatuses)
	
	DECLARE @RunningProcessIDsTable TABLE (ID int)
	INSERT INTO @RunningProcessIDsTable (ID)
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@RunningProcessIDs)

	UPDATE instance 
	SET [ExecutionStatus] = @TerminatedStatus
	FROM [bp].[BPInstance] instance
	JOIN @OpenStatusesTable openStatuses ON instance.ExecutionStatus = openStatuses.Status
	JOIN [bp].[BPInstance] parent ON instance.ParentID = parent.ID
	WHERE (parent.ExecutionStatus NOT IN (SELECT [Status] FROM @OpenStatusesTable))--Parent Process is not Open
	AND (instance.LockedByProcessID IS NULL OR instance.LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDsTable))
END