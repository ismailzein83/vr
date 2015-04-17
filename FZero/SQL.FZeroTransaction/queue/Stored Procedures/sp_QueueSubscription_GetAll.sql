-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueSubscription_GetAll]
	@QueueStatus int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	SELECT [QueueID]
		  ,[SubscribedQueueID]
	FROM [queue].[QueueSubscription]
	JOIN queue.QueueInstance sourceQueue ON sourceQueue.ID = [QueueID]
	JOIN queue.QueueInstance subscribedQueue ON subscribedQueue.ID = [SubscribedQueueID]
	WHERE sourceQueue.Status = @QueueStatus AND subscribedQueue.Status = @QueueStatus
END