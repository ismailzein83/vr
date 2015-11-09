Create PROCEDURE TOneWhS_Routing.[sp_RoutingDatabase_Delete] 
	@ID int
AS
BEGIN
	UPDATE TOneWhS_Routing.[RoutingDatabase]
	SET IsDeleted = 1,
		DeletedTime = GETDATE()
	WHERE ID = @ID
END

SET ANSI_NULLS ON