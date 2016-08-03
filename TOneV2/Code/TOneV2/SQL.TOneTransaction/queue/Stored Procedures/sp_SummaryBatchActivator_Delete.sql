
CREATE PROCEDURE [queue].[sp_SummaryBatchActivator_Delete]	
	@QueueID int,
	@BatchStart datetime
AS
BEGIN
  DELETE [queue].[SummaryBatchActivator]
  WHERE QueueID = @QueueID AND BatchStart = @BatchStart
END