-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_UpdateStatus]	
	@ID bigint,
	@ExecutionStatus int,
	@Message nvarchar(max),
	@WorkflowInstanceID uniqueidentifier
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	ExecutionStatus = @ExecutionStatus,
		StatusUpdatedTime = GETDATE(),
		LastMessage = ISNULL(@Message, LastMessage),
		WorkflowInstanceID = ISNULL(@WorkflowInstanceID, WorkflowInstanceID)
	WHERE ID = @ID
END