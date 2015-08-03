
CREATE PROCEDURE [dbo].[bp_RT_Part_BuildActionRoutes]
	 @CheckSpecialRequests BIT = 1 
	,@CheckRouteBlocks BIT = 1
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @Message varchar(500); 
	
	SET @Message = CONVERT(varchar, getdate(), 121);
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Start Build Action Tables', @Message; 	
	
	
	IF(@CheckRouteBlocks = 1)
	BEGIN
		
		SET @Message = CONVERT(varchar, getdate(), 121); 
		EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build Block Routes', @Message; 	
		
		EXEC bp_RT_Part_BuildRouteAction_Block;
	END
	
		SET @Message = CONVERT(varchar, getdate(), 121); 
		EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build Override Routes', @Message; 
	
		EXEC bp_RT_Part_BuildRouteAction_Override;
		
	IF(@CheckSpecialRequests = 1)
	BEGIN
		
		SET @Message = CONVERT(varchar, getdate(), 121); 
		EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Build Special Request Routes', @Message; 	
		
		EXEC bp_RT_Part_BuildRouteAction_SpecialRequest;
	END
	
	
	SET @Message = CONVERT(varchar, getdate(), 121); 
	EXEC bp_RT_Full_SetSystemMessages 'PartialBuildRoutes: Finish Build Action Tables', @Message; 	
	
END