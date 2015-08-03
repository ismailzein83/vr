
CREATE PROCEDURE [dbo].[bp_RT_Part_BuildBaseTables]
	 @RebuildOptionPool BIT = 1
    ,@RebuildRoutePool BIT = 1
    ,@RebuildZoneRates BIT = 1
	,@CheckToD BIT = 1
	,@IncludeBlockedZones BIT = 1 
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @Message varchar(500); 
	
	SET @Message = CONVERT(varchar, getdate(), 121); 
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Start Build Base Tables', @Message; 	
	
	
	SET @Message = CONVERT(varchar, getdate(), 121); 
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build Zone Rates', @Message; 
	
	EXEC bp_RT_Part_BuildZoneRates
		@RebuildZoneRates = @RebuildZoneRates,
		@CheckToD = @CheckToD,
		@IncludeBlockedZones = @IncludeBlockedZones;
	
	
	SET @Message = CONVERT(varchar, getdate(), 121); 
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build Route Pool', @Message; 
	
	EXEC bp_RT_Part_BuildRoutePool
		@RebuildRoutePool = @RebuildRoutePool;
		
	SET @Message = CONVERT(varchar, getdate(), 121); 
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build Route Option Pool', @Message; 		
	
	EXEC bp_RT_Part_BuildRouteOptionPool
		@RebuildRouteOptionPool = @RebuildOptionPool;
		
			
	SET @Message = CONVERT(varchar, getdate(), 121); 
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Finish Build Base Tables', @Message; 	
		
END