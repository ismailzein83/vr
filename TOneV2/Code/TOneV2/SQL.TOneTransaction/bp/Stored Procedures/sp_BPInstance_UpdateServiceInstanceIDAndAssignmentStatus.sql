-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPInstance_UpdateServiceInstanceIDAndAssignmentStatus]	
	@ID bigint,
	@ServiceInstanceID uniqueidentifier,
	@AssignmentStatus int
AS
BEGIN	
	
    UPDATE bp.BPInstance
    SET	ServiceInstanceID = @ServiceInstanceID,
		AssignmentStatus = @AssignmentStatus
	WHERE ID = @ID
END