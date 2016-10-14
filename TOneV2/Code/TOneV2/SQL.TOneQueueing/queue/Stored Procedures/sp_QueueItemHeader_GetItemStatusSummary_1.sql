
-- =============================================
-- Description:	<Get Count of Queue Intances per Status>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemHeader_GetItemStatusSummary]
AS
BEGIN
  SELECT QueueID, Status, COUNT(*) as Count
  FROM [queue].[QueueItemHeader] WITH(NOLOCK)
  WHERE Status <> 30
  GROUP BY QueueID, Status
END