
CREATE PROCEDURE [queue].[sp_SummaryBatchActivator_GetAll]	
AS
BEGIN
  SELECT QueueID, BatchStart,batchend, ActivatorID	
  FROM [queue].[SummaryBatchActivator] WITH(NOLOCK)
END