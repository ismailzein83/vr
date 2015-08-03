

CREATE PROCEDURE [dbo].[bp_RT_Full_BuildRouteComplete]
     @RebuildCodeSupply char(1) = 'Y'
	,@CheckToD char(1) = 'Y' 
	,@CheckSpecialRequests char(1) = 'Y' 
	,@CheckRouteBlocks char(1) = 'Y' 
	,@MaxSuppliersPerRoute INT = 10
	,@IncludeBlockedZones CHAR(1) = 'N'
	,@UpdateStamp datetime output
	,@RoutingTableFileGroup nvarchar(255) = 'PRIMARY'
	,@RoutingIndexesFileGroup nvarchar(255) = 'PRIMARY'
	,@SORT_IN_TEMPDB nvarchar(10) = 'ON' 
	,@ApplySaleMarketPrice CHAR(1) = 'N' 
WITH RECOMPILE
AS
BEGIN

SET NOCOUNT ON


IF NOT EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'dbo.[SystemMessage]') AND s.type in (N'U'))
BEGIN
	
		CREATE TABLE [dbo].[SystemMessage](
	[MessageID] [varchar](500) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[Message] [varchar](max) NULL,
	[Updated] [datetime] NOT NULL,
	[timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_SystemMessages] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[SystemMessage] ADD  CONSTRAINT [DF_SystemMessages_Updated]  DEFAULT (getdate()) FOR [Updated]
END

	-- If Rebuild Routes is already running error and return
	DECLARE @IsRunning char(1)
	SELECT @IsRunning = 'Y' FROM SystemMessage WHERE MessageID = 'BuildRoutes: Status' AND [Message] IS NOT NULL
	IF @IsRunning = 'Y' 
	BEGIN
		RAISERROR (N'Build Routes is already Runnning', 15, 1); 
		RETURN 
	END 

	DECLARE @MessageID varchar(500) 
	DECLARE @Description varchar(450) 
	DECLARE @Message varchar(500) 

	DELETE FROM SystemMessage WHERE MessageID LIKE 'BuildRoutes: %'
	
	SET @Message = CONVERT(varchar, getdate(), 121) 
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Start', @Message 	

	SET @Message = ('Started') 
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Status ', @Message 
	
	SET @UpdateStamp = getdate() 
	DECLARE @TruncateLogSQL varchar(max) 
	SELECT @TruncateLogSQL = 'BACKUP LOG ' + db_name() + ' WITH TRUNCATE_ONLY' 

	SET @Message = CONVERT(varchar, getdate(), 121) 
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Checking Prerequisite Tables', @Message 	
	EXEC bp_RT_Full_PreparePrerequisiteTables
	
	IF @RebuildCodeSupply = 'Y' 
	BEGIN 
			------------------------------ Init Base Tables -----------------------------------------
			-- Prepare base tables for Route Build --------------------------------------------------
			-- Zone Rate, RouteCodePool and Code Supply ----------------------------------------------
			SET @Message = CONVERT(varchar, getdate(), 121)
			EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Init Base Tables Started', @Message
			
			EXEC bp_RT_Full_InitBaseTables
			 @IncludeBlockedZones = @IncludeBlockedZones
			SET @Message = CONVERT(varchar, getdate(), 121)
			EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Init Base Tables Finished', @Message
			

	END
	

		Declare @CodeSupplyCount int
		Select @CodeSupplyCount=Count(*) from CodeSupply
		if @CodeSupplyCount=0
			BEGIN
				RAISERROR (N'Build Routes Failed due to empty Entities(CodeSupply|ZoneRates)', 15, 1); 
				RETURN 
			END 
	--------------------------- Build Action Tables ----------------------------------------
	-- Prepare Action Tables as Route and Route options for --------------------------------
	-- Block, Override and Special Request -------------------------------------------------
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Action tables Started', @Message
	
    EXEC bp_RT_Full_BuildRouteActionTables
     @CheckSpecialRequests  = @CheckSpecialRequests
	,@CheckRouteBlocks  = @CheckRouteBlocks 
    
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Action Tables Finished', @Message
	
	------------------------- Build Non LCR Route and Route Option -----------------------
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Non LCR Started', @Message
	
    EXEC bp_RT_Full_BuildNonLCR
    
    SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Non LCR Finished', @Message
    
	------------------------- Build LCR Route and Route Option -----------------------   
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build LCR Started', @Message
	
    EXEC bp_RT_Full_BuildLCR_Route
    @RoutingTableFileGroup = @RoutingTableFileGroup
	,@RoutingIndexesFileGroup = @RoutingIndexesFileGroup
	,@SORT_IN_TEMPDB = @SORT_IN_TEMPDB
	,@IncludeBlockedZones = @IncludeBlockedZones
	,@CheckRouteBlocks = @CheckRouteBlocks
	,@UpdateStamp = @UpdateStamp
	
	
       EXEC bp_RT_Full_CreateRoutesIndexes
       
    EXEC bp_RT_Full_BuildLCR_Options
    @CheckRouteBlocks = @CheckRouteBlocks
    ,@MaxSuppliersPerRoute  = @MaxSuppliersPerRoute
    ,@UpdateStamp = @UpdateStamp
    ,@ApplySaleMarketPrice = @ApplySaleMarketPrice
        
     
     exec bp_RT_Full_SwapTempRouteTables
      
 --     	IF(@CheckSpecialRequests = 'Y')
	--BEGIN
	--	SET @Message = CONVERT(varchar, getdate(), 121) 
	--	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Special request Actions Started', @Message 
		
	--	EXEC bp_RT_Full_BuildRouteAction_SpecialRequestV2
		
	--	SET @Message = CONVERT(varchar, getdate(), 121) 
	--	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build Special Request Actions Finished', @Message 
	--END	
           
    SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Build LCR Finished', @Message 
      
    ------------------------- Swap Tables -------------------------------------------    
 
    
    --EXEC bp_CleanTables
    
    DECLARE @CheckSpecialRequestsBit AS BIT =  CASE WHEN @CheckSpecialRequests = 'Y' THEN 1 ELSE 0 END
    DECLARE @CheckRouteBlocksBit AS BIT = CASE WHEN @CheckRouteBlocks = 'Y' THEN 1 ELSE 0 END
INSERT INTO [RouteBuildBatch]
           ([CheckSpecialRequests]
           ,[CheckRouteBlocks]
           ,[CheckTOD]
           ,[IncludeBlockedZones]
           ,[Type]
           ,[Targets]
           ,[TargetCustomers]
           ,[IsSynched]
           ,[MaxSuppliersPerRoute])
     VALUES
           (CASE WHEN @CheckSpecialRequests = 'Y' THEN 1 ELSE 0 END
           ,CASE WHEN @CheckRouteBlocks = 'Y' THEN 1 ELSE 0 END
           ,CASE WHEN @CheckToD = 'Y' THEN 1 ELSE 0 END
           ,CASE WHEN @IncludeBlockedZones = 'Y' THEN 1 ELSE 0 END
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,@MaxSuppliersPerRoute)
    
    	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: End', @Message


	-- Set Status to NULL (Not Running)
	EXEC bp_RT_Full_SetSystemMessages 'BuildRoutes: Status', @message = NULL
    
END