
CREATE PROCEDURE [dbo].[bp_RT_Full_InitBaseTables]
	@IncludeBlockedZones CHAR(1) = 'Y',
	@SPOT AS NVARCHAR(10) = '01301100',
	@Postfix AS NVARCHAR(10) = 'SYS',
	@CheckTOD CHAR(1) = 'Y'
WITH RECOMPILE
AS
BEGIN
	
	SET NOCOUNT ON

	DECLARE @when datetime
	SET @when = dbo.DateOf(getdate())
	
	DECLARE @Message varchar(500) 


--------- Create Route Codes Pool ------------
      
IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[RoutePool]') AND s.type in (N'U'))
	BEGIN
		 TRUNCATE TABLE RoutePool
	END


------------------------- Build Zone Rate Table ----------------------------------------------
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Zone Rate Started', @Message
	
	EXEC bp_RT_Full_BuildZoneRates
		@ZoneRatesIncludeBlockedZones = @IncludeBlockedZones
		,@CheckTOD = @CheckTOD
	
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Zone Rate Finished', @Message

	EXEC bp_RT_Full_FixUnsoldZones
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: fix Unsold Zones', @Message
	--------------------- Remove Duplicates from Zone Rate -----------------------------------------
	--;with aa as (
 -- select  ROW_NUMBER() over(partition by zr.ZoneID,zr.CustomerID,zr.SupplierID order by zr.ZoneID,zr.CustomerID,zr.SupplierID) RowNbr, * from ZoneRate2 zr 
 -- ) 
 -- delete ZoneRate2 from aa inner join ZoneRate2 zr on zr.ZoneID = aa.ZoneID AND zr.CustomerID = aa.CustomerID AND zr.SupplierID = aa.SupplierID   where aa.RowNbr >1

  
	
	--------------------- Build Code Pools RouteCodePool and Code Supply ---------------------
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Code Pools Started', @Message
	
	EXEC bp_RT_Full_BuildCodePools
		
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Code Pools Finished', @Message	


END