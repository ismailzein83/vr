-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_UpdateWorkflowInstanceID]	
	@ID bigint,
	@WorkflowInstanceID uniqueidentifier
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	WorkflowInstanceID = @WorkflowInstanceID
	WHERE ID = @ID
	--AND (WorkflowInstanceID IS NULL OR ExecutionStatus > 50) --ExecutionStatus Completed = 50
END