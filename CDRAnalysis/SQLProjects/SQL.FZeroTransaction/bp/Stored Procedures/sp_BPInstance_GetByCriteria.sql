CREATE PROCEDURE [bp].[sp_BPInstance_GetByCriteria]	
	@DefinitionID int,
	@DateFrom dateTime,
	@DateTo dateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [ID]
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
	FROM bp.[BPInstance] as bps WITH(NOLOCK)
	WHERE
	bps.DefinitionID = @DefinitionID and bps.CreatedTime BETWEEN  @DateFrom and @DateTo
	ORDER BY CreatedTime
END