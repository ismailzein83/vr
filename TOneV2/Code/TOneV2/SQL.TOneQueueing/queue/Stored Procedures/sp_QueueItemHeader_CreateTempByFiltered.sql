
CREATE PROCEDURE [queue].[sp_QueueItemHeader_CreateTempByFiltered]
	@TempTableName VARCHAR(200),
	@ItemIds [queue].[ItemIds] READONLY
AS
BEGIN
IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			With ExecutionFlowItemIds as (SELECT  itemHeader.[ItemID], itemHeader.[ExecutionFlowTriggerItemID]
            FROM  [queue].[QueueItemHeader] itemHeader WITH(NOLOCK) JOIN  @ItemIds ids ON itemHeader.ItemID = ids.ItemID)

			SELECT execFlowIds.[ItemID],
				   itemHeader.QueueID,
			       itemHeader.[Description],
			       itemHeader.[Status],
			       itemHeader.ExecutionFlowTriggerItemID,
			       itemHeader.RetryCount,
			       itemHeader.SourceItemID,
			       itemHeader.ErrorMessage,
				   itemHeader.CreatedTime,
				   itemHeader.LastUpdatedTime
			INTO #RESULT
			FROM [queue].[QueueItemHeader] itemHeader WITH(NOLOCK)
			INNER JOIN ExecutionFlowItemIds execFlowIds  WITH(NOLOCK)
			ON   itemHeader.ExecutionFlowTriggerItemID = execFlowIds.ExecutionFlowTriggerItemID
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF
END