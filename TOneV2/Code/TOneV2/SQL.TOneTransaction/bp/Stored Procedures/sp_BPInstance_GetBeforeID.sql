CREATE PROCEDURE [bp].[sp_BPInstance_GetBeforeID]
	@LessThanID BIGINT,
	@NbOfRows INT,
	@DefinitionsId varchar(max)
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
      ,[CreatedTime]
      ,[StatusUpdatedTime]
	  ,[timestamp]
	FROM [BP].[BPInstance] 
	WHERE ID < @LessThanID AND  (@DefinitionsId is null or DefinitionID in (select BPDefinitionId from @BPDefinitionIDsTable)) 
	ORDER BY ID DESC
END