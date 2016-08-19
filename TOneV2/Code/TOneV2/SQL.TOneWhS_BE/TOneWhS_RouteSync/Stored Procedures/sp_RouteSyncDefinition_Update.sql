Create Procedure [TOneWhS_RouteSync].[sp_RouteSyncDefinition_Update]
	@ID int,
	@Name nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 from TOneWhS_RouteSync.RouteSyncDefinition where ID != @ID AND Name = @Name)
	BEGIN
		update TOneWhS_RouteSync.RouteSyncDefinition
		set Name = @Name,
			Settings = @Settings
		where ID = @ID
	END
END