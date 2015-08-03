
CREATE PROCEDURE [dbo].[bp_RT_Full_BuildRouteActionTables]
	 @CheckSpecialRequests char(1) = 'Y' 
	,@CheckRouteBlocks char(1) = 'Y' 
WITH RECOMPILE
AS
BEGIN

	DECLARE @Message varchar(500)


	IF(@CheckRouteBlocks = 'Y')
	BEGIN
		SET @Message = CONVERT(varchar, getdate(), 121) 
		EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Block Actions Started', @Message 
		
		EXEC bp_RT_Full_BuildRouteAction_Block
			
		SET @Message = CONVERT(varchar, getdate(), 121) 
		EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Block Actions Finished', @Message 
	END


	SET @Message = CONVERT(varchar, getdate(), 121) 
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Override Actions Started', @Message 

	EXEC bp_RT_Full_BuildRouteAction_Override

	SET @Message = CONVERT(varchar, getdate(), 121) 
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Override Actions Finished', @Message 

	IF(@CheckSpecialRequests = 'Y')
	BEGIN
		SET @Message = CONVERT(varchar, getdate(), 121) 
		EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Special request Actions Started', @Message 
		
		EXEC bp_RT_Full_BuildRouteAction_SpecialRequest
		
		SET @Message = CONVERT(varchar, getdate(), 121) 
		EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Special Request Actions Finished', @Message 
	END	


END