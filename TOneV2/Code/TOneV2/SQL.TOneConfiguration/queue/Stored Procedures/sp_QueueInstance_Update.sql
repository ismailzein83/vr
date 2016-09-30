CREATE PROCEDURE [queue].[sp_QueueInstance_Update] 
	@QueueName varchar(255),
	@StageName varchar(255),
	@Title nvarchar(255),
	@Settings nvarchar(max)
AS
BEGIN
	
	UPDATE [queue].[QueueInstance]
	SET 
		StageName = @StageName,
		Title = @Title,
		[Settings] = @Settings
	WHERE Name = @QueueName
END