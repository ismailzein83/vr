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

		DECLARE @MaxGlobalTimeStamp timestamp = (SELECT MAX(timestamp) FROM [BP].[BPInstance] WITH(NOLOCK))
		SELECT TOP(@NbOfRows) [ID],[Title],[ParentID],[DefinitionID],[WorkflowInstanceID],[InputArgument],[CompletionNotifier],[ExecutionStatus],[LastMessage],EntityID,[ViewRequiredPermissionSetId]
				,[CreatedTime],[StatusUpdatedTime],[InitiatorUserId],[ServiceInstanceID],[timestamp]	INTO #temp_table
		FROM	[BP].[BPInstance] WITH(NOLOCK)
		WHERE	EXISTS (select BPDefinitionId from @BPDefinitionIDsTable where BPDefinitionId=[BP].[BPInstance].DefinitionID)
				AND (EntityId in (select EntityId from @EntityIdsTable) or @EntityIDs is null)
				and (ViewRequiredPermissionSetId in (select ViewRequiredPermissionSetId from @ViewRequiredPermissionSetTable) or ViewRequiredPermissionSetId is null)
				AND (ParentID = @ParentId or @ParentId is null) 
		ORDER BY ID DESC
            
		SELECT * FROM #temp_table
      
	  IF EXISTS (SELECT top 1 null from #temp_table)
		SELECT MAX([timestamp]) MaxTimestamp FROM #temp_table
	  ELSE 
		SELECT @MaxGlobalTimeStamp MaxTimestamp
	END	
ELSE
	BEGIN
	SELECT	TOP(@NbOfRows) [ID],[Title],[ParentID],[DefinitionID],[WorkflowInstanceID],[InputArgument], [CompletionNotifier],[ExecutionStatus],[LastMessage],inst.EntityID,inst.[ViewRequiredPermissionSetId]
			,[CreatedTime],[StatusUpdatedTime],[InitiatorUserId],[ServiceInstanceID],[timestamp]	INTO #temp2_table
    FROM	[BP].[BPInstance] inst WITH(NOLOCK) 
			LEFT JOIN @ViewRequiredPermissionSetTable perms ON  inst.ViewRequiredPermissionSetId = perms.ViewRequiredPermissionSetId
    WHERE 
			([timestamp] > @TimestampAfter) --ONLY Updated records
			and  exists (select BPDefinitionId from @BPDefinitionIDsTable where BPDefinitionId=inst.DefinitionID)
			AND (EntityId in (select EntityId from @EntityIdsTable) or @EntityIDs is null)
			AND (@ViewRequiredPermissionSetIds is null OR inst.ViewRequiredPermissionSetId IS NULL OR perms.ViewRequiredPermissionSetId IS NOT NULL)
            AND (ParentID = @ParentId or @ParentId is null)
    ORDER BY [timestamp]
            
    SELECT * FROM #temp2_table
      
	IF((SELECT COUNT(*) FROM #temp2_table) = 0)
			SELECT @TimestampAfter AS MaxTimestamp
		ELSE
			SELECT MAX([timestamp]) MaxTimestamp FROM #temp2_table

	END
END