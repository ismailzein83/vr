

CREATE PROCEDURE [bp].[sp_BPInstance_GetRecent]
	@StatusUpdatedAfter DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF(@StatusUpdatedAfter IS NULL)
		SELECT @StatusUpdatedAfter = MIN(bps.[StatusUpdatedTime]) 
		FROM bp.[BPInstance] as bps WITH(NOLOCK)
		JOIN bp.LKUP_ExecutionStatus bpsta ON bps.ExecutionStatus = bpsta.ID
		WHERE bpsta.IsOpened = 1
	
	
    SELECT bps.ID
	  ,bps.[Title]
      ,bps.[ParentID]
      ,bps.[DefinitionID]
      ,bps.[WorkflowInstanceID]
      ,bps.[InputArgument]
      ,bps.[ExecutionStatus]
      ,bps.[LockedByProcessID]
      ,bps.[LastMessage]
      ,bps.[RetryCount]
      ,bps.[CreatedTime]
      ,bps.[StatusUpdatedTime]
	FROM bp.[BPInstance] as bps WITH(NOLOCK)
	WHERE (@StatusUpdatedAfter IS NOT NULL AND bps.[StatusUpdatedTime] >= @StatusUpdatedAfter)
	ORDER BY [ID] DESC
	
END