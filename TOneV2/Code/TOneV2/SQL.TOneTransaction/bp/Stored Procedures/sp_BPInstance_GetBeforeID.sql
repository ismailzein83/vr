CREATE PROCEDURE [bp].[sp_BPInstance_GetBeforeID]
	@LessThanID BIGINT,
	@NbOfRows INT,
	@DefinitionsId varchar(max),
	@ParentId int,
	@EntityID varchar(50)
AS
BEGIN	
	DECLARE @BPDefinitionIDsTable TABLE (BPDefinitionId uniqueidentifier)
            INSERT INTO @BPDefinitionIDsTable (BPDefinitionId)
            select Convert(uniqueidentifier, ParsedString) from [bp].[ParseStringList](@DefinitionsId)
            
	SELECT TOP(@NbOfRows) [ID],[Title],[ParentID],[DefinitionID],[WorkflowInstanceID],[InputArgument],[ExecutionStatus],[LastMessage]
			,EntityID,[CreatedTime],[StatusUpdatedTime],[InitiatorUserId],[timestamp]
	FROM	[BP].[BPInstance] with(nolock)
	WHERE	(@EntityID is null or EntityID= @EntityID) and ID < @LessThanID AND  (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable)) 
			AND (@ParentId is null or ParentID = @ParentId)
	ORDER BY ID DESC
END