
CREATE PROCEDURE [bp].[sp_BPInstance_GetUpdated]
	@TimestampAfter timestamp,
	@NbOfRows INT,
	@DefinitionsId varchar(max)
AS
BEGIN
	DECLARE @BPDefinitionIDsTable TABLE (BPDefinitionId int)
            INSERT INTO @BPDefinitionIDsTable (BPDefinitionId)
            select Convert(int, ParsedString) from [bp].[ParseStringList](@DefinitionsId)

	IF (@TimestampAfter IS NULL)
	Begin
		Declare @Count int = (Select COUNT(1) from [BP].[BPInstance] WHERE (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable)));
		if(@Count>@NbOfRows)
			SELECT @TimestampAfter = MIN([timestamp])
			FROM (SELECT TOP (@NbOfRows+1) [timestamp] FROM [BP].[BPInstance] WHERE (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable)) ORDER BY ID DESC) LastTestCalls
	END 
	SELECT TOP (@NbOfRows) [ID]
	  ,[Title]
      ,[ParentID]
      ,[DefinitionID]
      ,[WorkflowInstanceID]
      ,[InputArgument]
      ,[ExecutionStatus]
      ,[LockedByProcessID]
      ,[LastMessage]
      ,[RetryCount]
      ,[CreatedTime]
      ,[StatusUpdatedTime]
	  ,[timestamp]
		  
	INTO #Result
	FROM [BP].[BPInstance] 
	WHERE (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable)) 
	AND ([timestamp] > @TimestampAfter or @TimestampAfter is null) --ONLY Updated records
	Order BY ID DESC
	
	SELECT * FROM #Result
	ORDER BY ID DESC
	
	IF((SELECT COUNT(*) FROM #Result) = 0)
		SELECT @TimestampAfter AS MaxTimestamp
	ELSE
		SELECT MAX([timestamp]) MaxTimestamp FROM #Result
	
END