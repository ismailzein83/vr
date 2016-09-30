-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueInstance_UpdateName]
	@QueueName varchar(255),
	@Status int,
	@NewQueueName varchar(255)
AS
BEGIN
	
	UPDATE queue.QueueInstance
	SET Name = @NewQueueName
	WHERE Name = @QueueName AND Status = @Status
	
END