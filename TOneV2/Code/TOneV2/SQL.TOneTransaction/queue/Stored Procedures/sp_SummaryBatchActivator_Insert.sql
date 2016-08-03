
CREATE PROCEDURE [queue].[sp_SummaryBatchActivator_Insert]	
	@QueueID int,
	@BatchStart datetime,
	@ActivatorID uniqueidentifier
AS
BEGIN
  INSERT INTO [queue].[SummaryBatchActivator]
           ([QueueID]
           ,[BatchStart]
           ,[ActivatorID])
     VALUES
           (@QueueID
           ,@BatchStart
           ,@ActivatorID)
END