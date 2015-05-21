
CREATE PROCEDURE [bp].[sp_BPInstance_UnLock]	
	@ProcessInstanceID bigint,
	@CurrentRuntimeProcessID int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	LockedByProcessID = NULL
	WHERE ID = @ProcessInstanceID
		AND
		LockedByProcessID = @CurrentRuntimeProcessID
END