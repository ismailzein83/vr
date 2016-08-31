

CREATE PROCEDURE [queue].[sp_QueueItemHeader_CreateTempByFiltered2]
	@TempTableName varchar(200),	
	@QueueIDs varchar(max), 
	@QueueStatusIds nvarchar(max),
	@CreatedTimeFrom DateTime,
	@CreatedTimeTo DateTime
AS
BEGIN
IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	    DECLARE @QueueIDsTable TABLE (QueueID int)
		INSERT INTO @QueueIDsTable (QueueID)
		select Convert(int, ParsedString) from [queue].[ParseStringList](@QueueIDs)
		 DECLARE @QueueStatusIDsTable TABLE (StatusID int)
		INSERT INTO @QueueStatusIDsTable (StatusID)
		select Convert(int, ParsedString) from [queue].[ParseStringList](@QueueStatusIds)
			SELECT itemHeader.ItemID,
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
			Where (@QueueIDs  is null or itemHeader.QueueID in(select QueueID from @QueueIDsTable))
			and (@QueueStatusIds  is null or itemHeader.Status in(select StatusID from @QueueStatusIDsTable))and (@CreatedTimeFrom is null or itemHeader.CreatedTime>=@CreatedTimeFrom)
			and (@CreatedTimeTo is null or itemHeader.CreatedTime<=@CreatedTimeTo)
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END                    

	SET NOCOUNT OFF
END