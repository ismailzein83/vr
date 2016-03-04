
CREATE PROCEDURE [bp].[sp_BPInstance_SetRunningStatusTerminated]		
	@TerminatedStatus int,
	@RunningStatus int,
	@RunningProcessIDs varchar(max)
AS
BEGIN
	
	DECLARE @RunningProcessIDsTable TABLE (ID int)
	INSERT INTO @RunningProcessIDsTable (ID)
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@RunningProcessIDs)

	UPDATE [bp].[BPInstance] 
	SET [ExecutionStatus] = @TerminatedStatus
	WHERE ExecutionStatus = @RunningStatus
		AND
		(LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDsTable))
END