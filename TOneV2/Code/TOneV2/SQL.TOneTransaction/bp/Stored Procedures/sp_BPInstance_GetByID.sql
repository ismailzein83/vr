CREATE PROCEDURE [bp].[sp_BPInstance_GetByID]	
	@ID bigint
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ID_Local bigint
	SELECT @ID_Local = @ID
	
    SELECT	[ID],[Title],[ParentID],[DefinitionID],[WorkflowInstanceID],[InputArgument], [CompletionNotifier],[ExecutionStatus], AssignmentStatus,[LastMessage],EntityID
      ,[ViewRequiredPermissionSetId],[CreatedTime],[StatusUpdatedTime],[InitiatorUserId], [ServiceInstanceID], TaskId
	FROM	bp.[BPInstance] WITH(NOLOCK)
	WHERE	ID = @ID_Local
END