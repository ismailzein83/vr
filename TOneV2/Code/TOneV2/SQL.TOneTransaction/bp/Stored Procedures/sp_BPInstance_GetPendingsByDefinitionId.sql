CREATE PROCEDURE [bp].[sp_BPInstance_GetPendingsByDefinitionId]
	@DefinitionID uniqueidentifier,
	@Statuses varchar(max),
	@NbOrRows int,
	@ServiceInstanceID uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @StatusesTable TABLE ([Status] int)
	INSERT INTO @StatusesTable ([Status])
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@Statuses)
	
    SELECT TOP(@NbOrRows) bp.[ID]
	  ,[Title]
      ,[ParentID]
      ,[DefinitionID]
      ,[WorkflowInstanceID]
      ,[InputArgument]
      ,[ExecutionStatus]
      ,[LastMessage]
	   ,EntityID
      ,[CreatedTime]
      ,[StatusUpdatedTime]      
      ,[InitiatorUserId]
	FROM bp.[BPInstance] bp WITH(NOLOCK)
	JOIN @StatusesTable statuses ON bp.ExecutionStatus = statuses.[Status]
	WHERE
	[DefinitionID] = @DefinitionID
	AND ServiceInstanceID = @ServiceInstanceID
	ORDER BY bp.[ID]
END