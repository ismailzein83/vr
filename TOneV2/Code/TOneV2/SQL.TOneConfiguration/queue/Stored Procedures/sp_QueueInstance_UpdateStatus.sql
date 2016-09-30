CREATE PROCEDURE [queue].[sp_QueueInstance_UpdateStatus] 
	@QueueName varchar(255),
	@Status int
AS
BEGIN
	
	UPDATE [queue].[QueueInstance]
	SET Status = @Status
	WHERE [Name] = @QueueName
	
END