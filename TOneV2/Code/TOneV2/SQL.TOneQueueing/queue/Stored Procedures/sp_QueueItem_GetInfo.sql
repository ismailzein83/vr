
CREATE PROCEDURE [queue].[sp_QueueItem_GetInfo]	
AS
BEGIN
	SELECT ID, QueueID, ActivatorID
  FROM [queue].[QueueItem] WITH(NOLOCK)
  WHERE IsNULL(IsSuspended, 0) = 0
END