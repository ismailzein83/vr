
CREATE PROCEDURE [bp].[sp_BPInstance_GetByCriterias]
	@ArrDefinitionID nvarchar(max),
	@ArrStatus nvarchar(max),
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
	WHERE (@ArrStatus is NULL or bps.ExecutionStatus in (SELECT ParsedString FROM ParseStringList(@ArrStatus) ) ) and 
	(@ArrDefinitionID is NULL or  bps.DefinitionID in (SELECT ParsedString FROM ParseStringList(@ArrDefinitionID) ) ) and 
	bps.CreatedTime BETWEEN  @DateFrom and @DateTo
	ORDER BY CreatedTime
END