
CREATE PROCEDURE [queue].[sp_QueueItem_GetInfo]	
	@MaxNbOfRows INT
AS
BEGIN
	SELECT TOP (@MaxNbOfRows) ID, ExecutionFlowTriggerItemID, QueueID, ActivatorID, BatchStart, BatchEnd
  FROM [queue].[QueueItem] WITH(NOLOCK)
  WHERE IsNULL(IsSuspended, 0) = 0
   order by (CASE WHEN ActivatorID IS NOT NULL THEN 1 ELSE 0 END) desc, [ExecutionFlowTriggerItemID], ID
END