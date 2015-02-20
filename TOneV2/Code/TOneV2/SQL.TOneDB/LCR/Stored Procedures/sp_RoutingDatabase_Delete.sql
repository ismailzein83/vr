CREATE PROCEDURE [LCR].[sp_RoutingDatabase_Delete] 
	@ID int
AS
BEGIN
	UPDATE [LCR].[RoutingDatabase]
	SET IsDeleted = 1,
		DeletedTime = GETDATE()
	WHERE ID = @ID
END