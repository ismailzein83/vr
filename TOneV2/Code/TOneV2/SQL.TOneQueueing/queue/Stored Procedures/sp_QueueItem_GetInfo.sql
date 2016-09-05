
CREATE PROCEDURE [queue].[sp_QueueItem_GetInfo]	
	@MaxNbOfRows INT
AS
BEGIN
	SELECT TOP (@MaxNbOfRows) ID, ExecutionFlowTriggerItemID, QueueID, ActivatorID
  FROM [queue].[QueueItem] WITH(NOLOCK)
  WHERE IsNULL(IsSuspended, 0) = 0
   order by [ExecutionFlowTriggerItemID], ID
END