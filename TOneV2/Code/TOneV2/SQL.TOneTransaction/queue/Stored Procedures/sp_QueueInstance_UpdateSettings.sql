
CREATE PROCEDURE [queue].[sp_QueueInstance_UpdateSettings] 
	@QueueName varchar(255),
	@Settings nvarchar(max)
AS
BEGIN
	
	UPDATE [queue].[QueueInstance]
	SET [Settings] = @Settings
	WHERE Name = @QueueName
END