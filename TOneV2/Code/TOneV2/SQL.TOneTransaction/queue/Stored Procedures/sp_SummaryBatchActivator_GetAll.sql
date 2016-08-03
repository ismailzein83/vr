
CREATE PROCEDURE [queue].[sp_SummaryBatchActivator_GetAll]	
AS
BEGIN
  SELECT QueueID, BatchStart, ActivatorID	
  FROM [queue].[SummaryBatchActivator] WITH(NOLOCK)
END