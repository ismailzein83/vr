﻿CREATE PROCEDURE [bp].[sp_BPInstance_GetAfterID]
	@GreaterThanID BIGINT,
	@DefinitionId uniqueidentifier
AS
BEGIN	         
	SELECT   [ID],[Title],[ParentID],[DefinitionID],[WorkflowInstanceID],[InputArgument], [CompletionNotifier],[ExecutionStatus], AssignmentStatus,[LastMessage],
			 [EntityID],[ViewRequiredPermissionSetId],[CreatedTime],[StatusUpdatedTime],[InitiatorUserId],[timestamp], [ServiceInstanceID], TaskId, CancellationRequestUserId
	FROM	[BP].[BPInstance] with(nolock)
	WHERE	(@GreaterThanID is null or [ID] > @GreaterThanID) and ([DefinitionID] = @DefinitionId)
END