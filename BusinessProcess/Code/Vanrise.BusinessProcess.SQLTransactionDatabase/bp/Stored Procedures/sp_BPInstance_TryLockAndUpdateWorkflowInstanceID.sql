
CREATE PROCEDURE [bp].[sp_BPInstance_TryLockAndUpdateWorkflowInstanceID]	
	@ProcessInstanceID bigint,
	@WorkflowInstanceID uniqueidentifier,
	@CurrentRuntimeProcessID int,
	@RunningProcessIDs bp.IDIntType readonly,
	@BPStatuses bp.IDIntType readonly
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	LockedByProcessID = @CurrentRuntimeProcessID,
	    WorkflowInstanceID = @WorkflowInstanceID
	WHERE ID = @ProcessInstanceID
		  AND ExecutionStatus IN (SELECT ID FROM @BPStatuses)
		  AND (LockedByProcessID IS NULL OR LockedByProcessID NOT IN (SELECT ID FROM @RunningProcessIDs))
END