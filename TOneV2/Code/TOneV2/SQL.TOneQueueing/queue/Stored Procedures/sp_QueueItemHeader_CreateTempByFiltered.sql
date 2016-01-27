-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemHeader_CreateTempByFiltered]
	@TempTableName VARCHAR(200),
	@ItemIds [queue].[ItemIds] READONLY
AS
BEGIN
IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			With ExecutionFlowItemIds as (SELECT  itemHeader.[ItemID], itemHeader.[ExecutionFlowTriggerItemID]
            FROM  [queue].[QueueItemHeader] itemHeader JOIN  @ItemIds ids ON itemHeader.ItemID = ids.ItemID)

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
			FROM [queue].[QueueItemHeader] itemHeader
			JOIN ExecutionFlowItemIds execFlowIds 
			ON   itemHeader.ExecutionFlowTriggerItemID = execFlowIds.ExecutionFlowTriggerItemID
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF
END