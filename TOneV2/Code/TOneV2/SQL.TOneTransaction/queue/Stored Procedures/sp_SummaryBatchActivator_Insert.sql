
CREATE PROCEDURE [queue].[sp_SummaryBatchActivator_Insert]	
	@QueueID int,
	@BatchStart datetime,
	@BatchEnd datetime,
	@ActivatorID uniqueidentifier
AS
BEGIN
  INSERT INTO [queue].[SummaryBatchActivator]
           ([QueueID]
           ,[BatchStart]
		   ,[BatchEnd]
           ,[ActivatorID])
     VALUES
           (@QueueID
           ,@BatchStart
		   ,@BatchEnd
           ,@ActivatorID)
END