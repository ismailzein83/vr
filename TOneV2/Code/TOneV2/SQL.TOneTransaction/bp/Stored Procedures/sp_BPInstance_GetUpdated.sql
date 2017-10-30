CREATE PROCEDURE [bp].[sp_BPInstance_GetUpdated]
	@TimestampAfter timestamp,
	@NbOfRows INT,
	@DefinitionsId varchar(max),
	@ParentId int,
	@EntityIDs varchar(max),
	@ViewRequiredPermissionSetIds varchar(max)
AS
BEGIN
	DECLARE @BPDefinitionIDsTable TABLE (BPDefinitionId uniqueidentifier)
            INSERT INTO @BPDefinitionIDsTable (BPDefinitionId)
            select Convert(uniqueidentifier, ParsedString) from [bp].[ParseStringList](@DefinitionsId)
	DECLARE @ViewRequiredPermissionSetTable TABLE (ViewRequiredPermissionSetId int)
            INSERT INTO @ViewRequiredPermissionSetTable (ViewRequiredPermissionSetId)
            select Convert(int, ParsedString) from [bp].[ParseStringList](@ViewRequiredPermissionSetIds)
 DECLARE @EntityIdsTable TABLE (EntityId varchar(10))
            INSERT INTO @EntityIdsTable (EntityId)
            select Convert(varchar(10), ParsedString) from [bp].[ParseStringList](@EntityIDs) 
IF (@TimestampAfter IS NULL)
	BEGIN
	SELECT TOP(@NbOfRows) [ID]
	  ,[Title]
      ,[ParentID]
      ,[DefinitionID]
      ,[WorkflowInstanceID]
      ,[InputArgument]
	  ,[CompletionNotifier]
      ,[ExecutionStatus]
      ,[LastMessage]
	   ,EntityID
      ,[ViewRequiredPermissionSetId]
      ,[CreatedTime]
      ,[StatusUpdatedTime]
      ,[InitiatorUserId]
	  ,[ServiceInstanceID]
	  ,[timestamp]
            INTO #temp_table
            FROM [BP].[BPInstance] WITH(NOLOCK)
            WHERE (@EntityIDs is null or EntityId in (select EntityId from @EntityIdsTable))
			and (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable))
			and (ViewRequiredPermissionSetId is null  or ViewRequiredPermissionSetId in (select ViewRequiredPermissionSetId from @ViewRequiredPermissionSetTable))
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
	  , [CompletionNotifier]
      ,[ExecutionStatus]
      ,[LastMessage]
	  ,EntityID
      ,[ViewRequiredPermissionSetId]
      ,[CreatedTime]
      ,[StatusUpdatedTime]
      ,[InitiatorUserId]
	  ,[ServiceInstanceID]
	  ,[timestamp]
            INTO #temp2_table
            FROM [BP].[BPInstance] WITH(NOLOCK) 
            WHERE ([timestamp] > @TimestampAfter) --ONLY Updated records
            AND (@EntityIDs is null or EntityId in (select EntityId from @EntityIdsTable)) and (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable))  
		    AND (ViewRequiredPermissionSetId is null  or ViewRequiredPermissionSetId in (select ViewRequiredPermissionSetId from @ViewRequiredPermissionSetTable))
            AND (@ParentId is null or ParentID = @ParentId)
            ORDER BY [timestamp]
            
            SELECT * FROM #temp2_table
      
IF((SELECT COUNT(*) FROM #temp2_table) = 0)
		SELECT @TimestampAfter AS MaxTimestamp
	ELSE
		SELECT MAX([timestamp]) MaxTimestamp FROM #temp2_table

	END
	END