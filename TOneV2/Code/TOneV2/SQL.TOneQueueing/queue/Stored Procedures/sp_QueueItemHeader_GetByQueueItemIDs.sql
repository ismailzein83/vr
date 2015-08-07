-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemHeader_GetByQueueItemIDs]
	@ItemIds [queue].[ItemIds] READONLY
AS
BEGIN
With ExecutionFlowItemIds as (SELECT  itemHeader.[ItemID], itemHeader.[ExecutionFlowTriggerItemID]
                  FROM  [queue].[QueueItemHeader] itemHeader JOIN  @ItemIds ids ON itemHeader.ItemID = ids.ItemID)

SELECT execFlowIds.[ItemID], itemHeader.[Description], itemHeader.[Status], itemHeader.RetryCount, itemHeader.ErrorMessage,
	itemHeader.CreatedTime, itemHeader.LastUpdatedTime
  FROM [queue].[QueueItemHeader] itemHeader
  JOIN ExecutionFlowItemIds execFlowIds ON itemHeader.ExecutionFlowTriggerItemID = execFlowIds.ExecutionFlowTriggerItemID

END