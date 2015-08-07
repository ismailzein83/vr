CREATE PROCEDURE [queue].[sp_QueueItemHeader_GetItemsExecutionStatus]
(
	@ItemIds [queue].[ItemIds] READONLY
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    With ExecutionFlowItemIds as (SELECT  itemHeader.[ItemID], itemHeader.[ExecutionFlowTriggerItemID]
                  FROM  [queue].[QueueItemHeader] itemHeader JOIN  @ItemIds ids ON itemHeader.ItemID = ids.ItemID)

	SELECT execFlowIds.[ItemID], execFlowIds.ExecutionFlowTriggerItemID,  itemHeader.Status
  FROM [queue].[QueueItemHeader] itemHeader
  JOIN ExecutionFlowItemIds execFlowIds ON itemHeader.ExecutionFlowTriggerItemID = execFlowIds.ExecutionFlowTriggerItemID
  GROUP BY execFlowIds.ItemID, execFlowIds.ExecutionFlowTriggerItemID,  itemHeader.Status
END