
CREATE PROCEDURE [bp].[sp_BPInstance_UpdateStatuses]		
	@FromStatus int,
	@ToStatus int,
	@RunningProcessIDs bp.IDIntType readonly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	ExecutionStatus = @ToStatus
	WHERE 
		ExecutionStatus = @FromStatus
		AND
		LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDs)		
END