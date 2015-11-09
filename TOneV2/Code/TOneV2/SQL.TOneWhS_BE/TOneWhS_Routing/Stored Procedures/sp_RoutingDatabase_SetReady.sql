
Create PROCEDURE TOneWhS_Routing.[sp_RoutingDatabase_SetReady] 
   @ID int
AS
BEGIN
	UPDATE TOneWhS_Routing.[RoutingDatabase]
    SET IsReady = 1,
		ReadyTime = GETDATE()
	WHERE ID = @ID
END