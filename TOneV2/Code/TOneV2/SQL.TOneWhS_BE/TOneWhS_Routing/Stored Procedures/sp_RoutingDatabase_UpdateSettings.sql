CREATE PROCEDURE [TOneWhS_Routing].[sp_RoutingDatabase_UpdateSettings] 
   @ID int,
   @Settings nvarchar(max)
AS
BEGIN
	Update TOneWhS_Routing.[RoutingDatabase] set Settings = @Settings
	where ID = @ID
END