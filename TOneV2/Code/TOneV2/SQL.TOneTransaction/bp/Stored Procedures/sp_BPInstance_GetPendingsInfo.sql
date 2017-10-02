CREATE PROCEDURE [bp].[sp_BPInstance_GetPendingsInfo]
	@Statuses varchar(max)
AS
BEGIN
	DECLARE @StatusesTable TABLE ([Status] int)
	INSERT INTO @StatusesTable ([Status])
	SELECT Convert(int, ParsedString) FROM bp.[ParseStringList](@Statuses)

    SELECT bp.[ID]
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
	FROM bp.[BPInstance] bp-- WITH(NOLOCK)
	JOIN @StatusesTable statuses ON bp.ExecutionStatus = statuses.[Status]
	ORDER BY ID
END