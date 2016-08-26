CREATE PROCEDURE [bp].[sp_BPEvent_DeleteByProcessInstanceId]
	@ProcessInstanceId bigint
AS
BEGIN	
	DELETE [bp].[BPEvent]
	WHERE [ProcessInstanceID] = @ProcessInstanceId
END