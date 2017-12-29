CREATE PROCEDURE [bp].[sp_BPInstance_UpdateAssignmentStatus]	
	@ID bigint,
	@AssignmentStatus int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	AssignmentStatus = @AssignmentStatus
	WHERE ID = @ID
END