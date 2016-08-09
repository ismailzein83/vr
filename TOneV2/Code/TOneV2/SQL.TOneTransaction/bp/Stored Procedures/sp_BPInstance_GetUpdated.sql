CREATE PROCEDURE [bp].[sp_BPInstance_GetUpdated]
	@TimestampAfter timestamp,
	@NbOfRows INT,
	@DefinitionsId varchar(max),
	@ParentId int,
	@EntityID varchar(50)
AS
BEGIN
	DECLARE @BPDefinitionIDsTable TABLE (BPDefinitionId int)
            INSERT INTO @BPDefinitionIDsTable (BPDefinitionId)
            select Convert(int, ParsedString) from [bp].[ParseStringList](@DefinitionsId)

IF (@TimestampAfter IS NULL)
	BEGIN
	SELECT TOP(@NbOfRows) [ID]
	  ,[Title]
      ,[ParentID]
      ,[DefinitionID]
      ,[WorkflowInstanceID]
      ,[InputArgument]
      ,[ExecutionStatus]
      ,[LockedByProcessID]
      ,[LastMessage]
      ,[RetryCount]
	   ,EntityID
      ,[CreatedTime]
      ,[StatusUpdatedTime]
      ,[InitiatorUserId]
	  ,[timestamp]
            INTO #temp_table
            FROM [BP].[BPInstance] WITH(NOLOCK)
            WHERE (@EntityID is null or EntityID = @EntityID) and (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable))
            AND (@ParentId is null or ParentID = @ParentId) 
            ORDER BY ID DESC
            
            SELECT * FROM #temp_table
      
            SELECT MAX([timestamp]) MaxTimestamp FROM #temp_table
	END
	
	ELSE
	BEGIN
	SELECT TOP(@NbOfRows) [ID]
	  ,[Title]
      ,[ParentID]
      ,[DefinitionID]
      ,[WorkflowInstanceID]
      ,[InputArgument]
      ,[ExecutionStatus]
      ,[LockedByProcessID]
      ,[LastMessage]
      ,[RetryCount]
	  ,EntityID
      ,[CreatedTime]
      ,[StatusUpdatedTime]
      ,[InitiatorUserId]
	  ,[timestamp]
            INTO #temp2_table
            FROM [BP].[BPInstance] WITH(NOLOCK) 
            WHERE (@EntityID is null or EntityID = @EntityID) and (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable))  AND
            ([timestamp] > @TimestampAfter) --ONLY Updated records
            AND (@ParentId is null or ParentID = @ParentId)
            ORDER BY [timestamp]
            
            SELECT * FROM #temp2_table
      
IF((SELECT COUNT(*) FROM #temp2_table) = 0)
		SELECT @TimestampAfter AS MaxTimestamp
	ELSE
		SELECT MAX([timestamp]) MaxTimestamp FROM #temp2_table

	END
	END