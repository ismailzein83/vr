-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE queue.sp_QueueSubscription_GetMaxTimestamp
AS
BEGIN
	SELECT MAX(timestamp)
    FROM queue.QueueSubscription
END