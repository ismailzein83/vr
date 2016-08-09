CREATE PROCEDURE [bp].[sp_BPInstance_GetBeforeID]
	@LessThanID BIGINT,
	@NbOfRows INT,
	@DefinitionsId varchar(max),
	@ParentId int,
	@EntityID varchar(50)
AS
BEGIN	
	DECLARE @BPDefinitionIDsTable TABLE (BPDefinitionId int)
            INSERT INTO @BPDefinitionIDsTable (BPDefinitionId)
            select Convert(int, ParsedString) from [bp].[ParseStringList](@DefinitionsId)
            
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
	FROM [BP].[BPInstance]  WITH(NOLOCK)
	WHERE (@EntityID is null or EntityID= @EntityID) and ID < @LessThanID AND  (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable)) 
	AND (@ParentId is null or ParentID = @ParentId)
	ORDER BY ID DESC
END