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
	FROM [queue].[QueueSubscription] WITH(NOLOCK) 
	JOIN queue.QueueInstance sourceQueue  WITH(NOLOCK) ON sourceQueue.ID = [QueueID]
	JOIN queue.QueueInstance subscribedQueue  WITH(NOLOCK) ON subscribedQueue.ID = [SubscribedQueueID]
	WHERE sourceQueue.Status = @QueueStatus AND subscribedQueue.Status = @QueueStatus
END