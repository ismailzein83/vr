
CREATE PROCEDURE [queue].[sp_SummaryBatchActivator_GetByActivator]	
	@ActivatorId uniqueidentifier
AS
BEGIN
  SELECT QueueID, BatchStart, ActivatorID	
  FROM [queue].[SummaryBatchActivator] WITH(NOLOCK)
  WHERE ActivatorID = @ActivatorId
END