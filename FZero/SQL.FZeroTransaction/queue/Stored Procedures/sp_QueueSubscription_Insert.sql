-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueSubscription_Insert] 
	@QueueID int,
	@SubscribedQueueID int
AS
BEGIN
	
	INSERT INTO [queue].[QueueSubscription]
           ([QueueID]
           ,[SubscribedQueueID])
	VALUES
           (@QueueID
           ,@SubscribedQueueID)
           
END