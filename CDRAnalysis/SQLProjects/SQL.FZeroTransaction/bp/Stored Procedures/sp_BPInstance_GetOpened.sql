

CREATE PROCEDURE [bp].[sp_BPInstance_GetOpened]
	@ArrStatus nvarchar(max)
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
	WHERE (@ArrStatus is NULL or bps.ExecutionStatus in (SELECT ParsedString FROM ParseStringList(@ArrStatus) ) ) 
	ORDER BY CreatedTime
END