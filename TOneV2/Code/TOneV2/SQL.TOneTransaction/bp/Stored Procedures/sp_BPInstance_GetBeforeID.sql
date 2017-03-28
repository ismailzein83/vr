CREATE PROCEDURE [bp].[sp_BPInstance_GetBeforeID]
	@LessThanID BIGINT,
	@NbOfRows INT,
	@DefinitionsId varchar(max),
	@ParentId int,
	@EntityID varchar(50),
	@ViewRequiredPermissionSetIds varchar(max)
AS
BEGIN	
	DECLARE @BPDefinitionIDsTable TABLE (BPDefinitionId uniqueidentifier)
            INSERT INTO @BPDefinitionIDsTable (BPDefinitionId)
            select Convert(uniqueidentifier, ParsedString) from [bp].[ParseStringList](@DefinitionsId)
	DECLARE @ViewRequiredPermissionSetTable TABLE (ViewRequiredPermissionSetId int)
            INSERT INTO @ViewRequiredPermissionSetTable (ViewRequiredPermissionSetId)
            select Convert(int, ParsedString) from [bp].[ParseStringList](@ViewRequiredPermissionSetIds)
            
	SELECT TOP(@NbOfRows) [ID],[Title],[ParentID],[DefinitionID],[WorkflowInstanceID],[InputArgument], [CompletionNotifier],[ExecutionStatus],[LastMessage]
			,EntityID,[ViewRequiredPermissionSetId],[CreatedTime],[StatusUpdatedTime],[InitiatorUserId],[timestamp]
	FROM	[BP].[BPInstance] with(nolock)
	WHERE	(@EntityID is null or EntityID= @EntityID)
			and ID < @LessThanID
			AND  (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable))			
			and (ViewRequiredPermissionSetId is null or ViewRequiredPermissionSetId in (select ViewRequiredPermissionSetId from @ViewRequiredPermissionSetTable))
			AND (@ParentId is null or ParentID = @ParentId)

	ORDER BY ID DESC
END