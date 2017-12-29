-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_UpdateStatus]	
	@ID bigint,
	@ExecutionStatus int,
	@AssignmentStatus int,
	@Message nvarchar(max),
	@ClearServiceInstanceId bit,
	@WorkflowInstanceID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	ExecutionStatus = @ExecutionStatus,
		AssignmentStatus = @AssignmentStatus,
		StatusUpdatedTime = GETDATE(),
		LastMessage = ISNULL(@Message, LastMessage),
		ServiceInstanceID = CASE WHEN @ClearServiceInstanceId = 1 THEN NULL ELSE ServiceInstanceID END,
		WorkflowInstanceID = ISNULL(@WorkflowInstanceID, WorkflowInstanceID)
	WHERE ID = @ID
END