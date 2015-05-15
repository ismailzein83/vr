-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_GetPendings]
	@ExcludeProcessInstanceIds bp.IDBigIntType readonly,
	@BPStatuses bp.IDIntType readonly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT bp.[ID]
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
	FROM bp.[BPInstance] bp WITH(NOLOCK)
	JOIN @BPStatuses statuses ON bp.ExecutionStatus = statuses.ID
	LEFT JOIN @ExcludeProcessInstanceIds excludedIds ON bp.ID = excludedIds.ID
	WHERE
	excludedIds.ID IS NULL
	ORDER BY ParentID, CreatedTime
END