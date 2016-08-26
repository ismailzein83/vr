CREATE PROCEDURE [bp].[sp_BPEvent_GetDistinctProcessInstanceIds]
AS
BEGIN
	
	SELECT [ProcessInstanceID]
	FROM [bp].[BPEvent] e  WITH(NOLOCK)
	GROUP BY [ProcessInstanceID]
END