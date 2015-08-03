	CREATE PROCEDURE [dbo].[bp_BuildRoutes]
		@RebuildCodeSupply char(1) = 'Y' -- Y to Rebuild the code supply table
	,@CheckToD char(1) = 'Y' -- Y to Check the ToD Considerations after the build
	,@CheckSpecialRequests char(1) = 'Y' -- Y to Check special requests and alter the tables accordingly
	,@CheckRouteBlocks char(1) = 'Y' -- Y to check route blocks	
	,@MaxSuppliersPerRoute INT = 3
	,@IncludeBlockedZones CHAR(1) = 'N'
	,@UpdateStamp datetime output
	,@RoutingTableFileGroup nvarchar(255) = 'PRIMARY'
	,@RoutingIndexesFileGroup nvarchar(255) = 'PRIMARY'
	,@SORT_IN_TEMPDB nvarchar(10) = 'ON' 
WITH RECOMPILE
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @RowCount bigint

	DECLARE @State_Blocked tinyint
	DECLARE @State_Enabled tinyint
	SET @State_Blocked = 0
	SET @State_Enabled = 1

	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	-- If Rebuild Routes is already running error and return
	DECLARE @IsRunning char(1)
	SELECT @IsRunning = 'Y' FROM SystemMessage WHERE MessageID = 'BuildRoutes: Status' AND [Message] IS NOT NULL
	IF @IsRunning = 'Y' 
	BEGIN
		RAISERROR (N'Build Routes is already Runnning', 15, 1); 
		RETURN 
	END 

	DECLARE @MessageID varchar(50) 
	DECLARE @Description varchar(450) 
	DECLARE @Message varchar(500) 

	DELETE FROM SystemMessage WHERE MessageID LIKE 'BuildRoutes: %'
	
	SET @Message = CONVERT(varchar, getdate(), 121) 
	EXEC bp_SetSystemMessage 'BuildRoutes: Start', @Message 	

	SET @Message = ('Build Routes Started at: ' + CONVERT(varchar, getdate(), 121)) 
	EXEC bp_SetSystemMessage 'BuildRoutes: Status', @Message 
	
	SET @UpdateStamp = getdate() 
	DECLARE @TruncateLogSQL varchar(max) 
	SELECT @TruncateLogSQL = 'BACKUP LOG ' + db_name() + ' WITH TRUNCATE_ONLY' 
	
	-- Re-Build Code Supply if required 
	IF @RebuildCodeSupply = 'Y' 
	BEGIN 
		-- Build Zone Rates 
		
		EXEC bp_BuildZoneRates @IncludeBlockedZones=@IncludeBlockedZones 
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Zone Rate Rebuilt', @Message
		-- Fix Unsold Zones 
		EXEC bp_FixUnsoldZonesForRouteBuild
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: fix Unsold Zones', @Message
		 		
		-- Rebuild the Code Supply
		EXEC bp_BuildCodeSupply 

		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Code Supply Rebuilt', @Message
	END
		
		--- If No CodeSupply or ZoneRate
		Declare @CodeSupplyCount int
		Select @CodeSupplyCount=Count(*) from CodeSupply
		if @CodeSupplyCount=0
			BEGIN
				RAISERROR (N'Build Routes Failed due to empty Entities(CodeSupply|ZoneRates)', 15, 1); 
				RETURN 
			END 

	-- Make sure to Define Parameters for table creation properly
	IF ISNULL(@SORT_IN_TEMPDB, '') = '' SET @SORT_IN_TEMPDB = 'ON'
	IF @SORT_IN_TEMPDB <> 'ON' SET @SORT_IN_TEMPDB = 'OFF'
	IF ISNULL(@RoutingTableFileGroup, '') = '' SET @RoutingTableFileGroup = 'PRIMARY'
	IF ISNULL(@RoutingIndexesFileGroup, '') = '' SET @RoutingIndexesFileGroup = 'PRIMARY'

	DECLARE @RouteTableCreationSQL nvarchar(max) 
	DECLARE @RouteIndexesCreationSQL nvarchar(max)
	DECLARE @RouteOptionTableCreationSQL nvarchar(max) 
	DECLARE @RouteOptionIndexesCreationSQL nvarchar(max)
	DECLARE @RouteOptionIndexesCreationSQL1 nvarchar(max)
	DECLARE @RouteOptionIndexesCreationSQL2 nvarchar(max)
	DECLARE @RouteOptionIndexesCreationSQL3 nvarchar(max)


	DECLARE @RouteIndexesCreationSQL1 nvarchar(max)
	DECLARE @RouteIndexesCreationSQL2 nvarchar(max)
	DECLARE @RouteIndexesCreationSQL3 nvarchar(max)
	DECLARE @RouteIndexesCreationSQL4 nvarchar(max)
	DECLARE @RouteIndexesCreationSQL5 nvarchar(max)

	SET @RouteTableCreationSQL = '
		-- Temp Route
		CREATE TABLE [dbo].[TempRoute](
			[RouteID] [int]  NOT NULL,
			[CustomerID] [varchar](5) NOT NULL,
			[ProfileID] [int] NULL,
			[Code] [varchar](15) NULL,
			[OurZoneID] [int] NULL,
			[OurActiveRate] [real] NULL,
			[OurNormalRate] [real] NULL,
			[OurOffPeakRate] [real] NULL,
			[OurWeekendRate] [real] NULL,
			[OurServicesFlag] [smallint] NULL,
			[State] [tinyint] NOT NULL,
			[Updated] [datetime] NULL,
			[IsToDAffected] [char](1) NOT NULL DEFAULT(''N''),
			[IsSpecialRequestAffected] [char](1) NOT NULL DEFAULT(''N''),
			[IsBlockAffected] [char](1) NOT NULL DEFAULT(''N''),
		) ON [' + @RoutingTableFileGroup + ']
	';

	SET @RouteIndexesCreationSQL = '
		CREATE CLUSTERED INDEX [PK_RouteID] ON [dbo].[TempRoute]([RouteID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') 
		CREATE NONCLUSTERED INDEX [IX_Route_Code] ON [dbo].[TempRoute]([Code] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IX_Route_Customer] ON [dbo].[TempRoute]([CustomerID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IX_Route_Updated] ON [dbo].[TempRoute]([Updated] DESC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IX_Route_Zone] ON [dbo].[TempRoute]([OurZoneID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
	';
	
	SET @RouteIndexesCreationSQL1 = 'CREATE CLUSTERED INDEX [PK_RouteID] ON [dbo].[TempRoute]([RouteID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+')'
	SET @RouteIndexesCreationSQL2 = 'CREATE NONCLUSTERED INDEX [IX_Route_Code] ON [dbo].[Route]([Code] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']'
	SET @RouteIndexesCreationSQL3 = 'CREATE NONCLUSTERED INDEX [IX_Route_Customer] ON [dbo].[Route]([CustomerID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']'
	SET @RouteIndexesCreationSQL4 = 'CREATE NONCLUSTERED INDEX [IX_Route_Updated] ON [dbo].[Route]([Updated] DESC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']'
	SET @RouteIndexesCreationSQL5 = 'CREATE NONCLUSTERED INDEX [IX_Route_Zone] ON [dbo].[Route]([OurZoneID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']'
	

	Set @RouteOptionTableCreationSQL = '
 		-- Temp Route Option
		CREATE TABLE [dbo].[TempRouteOption](
			[RouteID] [int] NOT NULL,
			[SupplierID] [varchar](5) NOT NULL,
			[SupplierZoneID] [int] NULL,
			[SupplierActiveRate] [real] NULL,
			[SupplierNormalRate] [real] NULL,
			[SupplierOffPeakRate] [real] NULL,
			[SupplierWeekendRate] [real] NULL,
			[SupplierServicesFlag] [smallint] NULL,
			[Priority] [tinyint] NOT NULL,
			[NumberOfTries] [tinyint] NULL,
			[State] [tinyint] NOT NULL DEFAULT ((0)),
			[Updated] [datetime] NULL,
			[Percentage] [tinyint] NULL
		) ON [' + @RoutingTableFileGroup + ']
	';
	SET @RouteOptionIndexesCreationSQL = '
		CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[TempRouteOption]([RouteID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') 
		CREATE NONCLUSTERED INDEX [IDX_RouteOption_SupplierZoneID] ON [dbo].[TempRouteOption]([SupplierZoneID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IDX_RouteOption_Updated] ON [dbo].[TempRouteOption]([Updated] DESC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
'
	SET @RouteOptionIndexesCreationSQL1 = '
		CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[TempRouteOption]([RouteID] ASC) WITH (ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON,SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') 
	';
	SET @RouteOptionIndexesCreationSQL2 = '
		CREATE NONCLUSTERED INDEX [IDX_RouteOption_SupplierZoneID] ON [dbo].[RouteOption]([SupplierZoneID] ASC) WITH (ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON,SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
	';
	SET @RouteOptionIndexesCreationSQL3 = '
		CREATE NONCLUSTERED INDEX [IDX_RouteOption_Updated] ON [dbo].[RouteOption]([Updated] DESC) WITH (ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON,SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
	'
	



	--DBCC TRACEON (610)
	-- Create Temp Working Route Table
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TempRoute]') AND type in (N'U'))
	BEGIN
		DROP TABLE [dbo].[TempRoute]
	END
	
		-- Temp Route Option
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TempRouteOption]') AND type in (N'U'))
	BEGIN
		DROP TABLE [dbo].[TempRouteOption]
	END

	EXEC (@RouteTableCreationSQL);
	EXEC (@RouteOptionTableCreationSQL);

		WITH OurZoneRatesTemp AS 
		(
			SELECT  zr.ZoneID, 
					zr.SupplierID, 
					zr.CustomerID, 
					zr.NormalRate,
					zr.OffPeakRate, 
					zr.WeekendRate, 
					zr.ServicesFlag
			FROM    ZoneRate zr WITH(NOLOCK) 
			WHERE  zr.SupplierID = 'SYS'
		)
		, CarrierAccountTemp AS 
		(
    		SELECT ca.ProfileID, ca.CarrierAccountID
			FROM CarrierAccount ca WITH(NOLOCK) where ca.ActivationStatus in (1,2)
		)
	    ,OurZoneRates AS
		(
			SELECT  a.PROFILEID , 
					zr.ZoneID, 
					zr.SupplierID, 
					zr.CustomerID, 
					zr.NormalRate,
					zr.OffPeakRate, 
					zr.WeekendRate, 
					zr.ServicesFlag
			FROM	OurZoneRatesTemp zr WITH(NOLOCK)
			LEFT	JOIN CarrierAccountTemp a  ON  a.CarrierAccountID = zr.CustomerID 
			WHERE	zr.SupplierID = 'SYS'
    	)
    	,CodeMatchTemp AS (
    		SELECT * 
    		FROM CodeMatch cm WITH(NOLOCK)
    	)
    	 , RoutesToInsert AS (		
			SELECT
				convert(int,ROW_NUMBER() OVER (ORDER BY getdate())) AS RouteID,
				R.CustomerID CustomerID,
				R.ProfileID ProfileID,
				CM.Code Code,
				CM.SupplierZoneID OurZoneID,
				R.NormalRate OurActiveRate,
				R.NormalRate OurNormalRate,
				R.OffPeakRate OurOffPeakRate,
				R.WeekendRate OurWeekendRate,
				R.ServicesFlag OurServicesFlag,
				@State_Enabled State,
				--@UpdateStamp AS Updated,
				'N' IsToDAffected,
				'N' IsSpecialRequestAffected,
				'N' IsBlockAffected
			FROM   CodeMatchTemp CM WITH (NOLOCK) 
			INNER JOIN OurZoneRates R WITH (NOLOCK) ON  CM.SupplierZoneID = R.ZoneID

    	)
    	
    		Insert INTO TempRoute with(TABLOCK) 
			SELECT
			RouteID
		   ,[CustomerID]
           ,[ProfileID]
           ,[Code]
           ,[OurZoneID]
           ,[OurActiveRate]
           ,[OurNormalRate]
           ,[OurOffPeakRate]
           ,[OurWeekendRate]
           ,[OurServicesFlag]
           ,[State]
           ,DATEADD(MS,CASE WHEN Routeid<1000 THEN -Routeid ELSE -Routeid/1000 End,@UpdateStamp)
           ,[IsToDAffected]
           ,[IsSpecialRequestAffected]
           ,[IsBlockAffected]

			FROM RoutesToInsert 
			order by CustomerID,Code

	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Routes Inserted', @Message;
 		
	EXEC (@RouteIndexesCreationSQL);
--	EXEC (@RouteIndexesCreationSQL1);

	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: INDEX Built for Routes', @Message;
--	EXEC bp_SetSystemMessage 'BuildRoutes: INDEX [PK_RouteID] Built for Routes', @Message;

	with TheOptions AS

	(
		SELECT
			Rt.RouteID,
			CS.SupplierID,
			CS.SupplierZoneID,
			CS.SupplierNormalRate AS SupplierActiveRate,
			CS.SupplierNormalRate,
			CS.SupplierOffPeakRate,
			CS.SupplierWeekendRate,
			CS.SupplierServicesFlag,
			(ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.SupplierNormalRate)) as RowNumber,
			1 AS NumberOfTries,
			1 AS [State]
		FROM
				TempRoute Rt left join CodeSupply CS 
				on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT	
				LEFT JOIN [RoutingPoolCustomer] RPC ON Rt.CustomerID=RPC.CustomerID
				LEFT JOIN [RoutingPoolSupplier] RPS ON CS.SupplierID=RPS.SupplierID and rpc.id=rps.id 
				
			WHERE 
				
				RT.ProfileID <> CS.ProfileID -- Prevent Looping
				AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
				AND CS.SupplierNormalRate <= Rt.OurNormalRate
				AND (RPC.CustomerID IS NULL OR RPC.ID=RPS.ID)
	)

	INSERT INTO [TempRouteOption]  with(TABLOCK) 
	(
		RouteID,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierNormalRate,
		SupplierOffPeakRate,
		SupplierWeekendRate,
		SupplierServicesFlag,
		Priority,
		NumberOfTries,
		[State],
		Updated
	)
	SELECT 
		RouteID,
		SupplierID,
		SupplierZoneID,
		SupplierActiveRate,
		SupplierNormalRate,
		SupplierOffPeakRate,
		SupplierWeekendRate,
		SupplierServicesFlag,
		0,
		NumberOfTries,
		[State],
		DATEADD(MS,CASE WHEN Routeid<1000 THEN -Routeid ELSE -Routeid/1000 End,@UpdateStamp)
	FROM TheOptions
	WHERE RowNumber <= @MaxSuppliersPerRoute 

	--DBCC TRACEOFF (610)
	
	
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Route Options Inserted', @Message

	
	EXEC (@RouteOptionIndexesCreationSQL)
	--EXEC (@RouteOptionIndexesCreationSQL1)
	
	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: INDEX Built for RouteOptions', @Message
--	EXEC bp_SetSystemMessage 'BuildRoutes: INDEX [IDX_RouteOption_RouteID] Built for RouteOptions', @Message
		
	-- Rename Temp to Route and Route Options
	BEGIN TRANSACTION		
		DROP TABLE [dbo].[Route]
		EXEC sp_rename 'TempRoute', 'Route'
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: [TempRoutes] Renamed to [Route]', @Message
		
		DROP TABLE [dbo].[RouteOption]
		EXEC sp_rename 'TempRouteOption', 'RouteOption'
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: [TempRouteOptions] Renamed to [RouteOptions]', @Message
	COMMIT TRANSACTION

	-- Check ToD
	IF @CheckToD = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing ToD', @Message

		EXEC bp_UpdateRoutesFromToD @Check='N'
		-- EXECUTE( @TruncateLogSQL )

		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: ToD Processed', @Message
	End	
		
	-- Check Special Requests
	IF @CheckSpecialRequests = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing Special Requests', @Message
		
		-- Special Requests
		EXEC bp_UpdateRoutesFromSpecialRequests @Check='N'
		
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Special Requests Processed', @Message
	End

	-- Route Overrides
	EXEC bp_UpdateRoutingFromOverrides_WithPercentage

	-- Check Route Blocks
	IF @CheckRouteBlocks = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing Route Blocks', @Message

		-- Route Blocks
		EXEC bp_UpdateRoutesFromRouteBlocks @Check='N'
		
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Route Blocks Processed', @Message
	END

	--EXEC bp_SetSystemMessage 'BuildRoutes: Building Other INDEX for Route', @Message

	--EXEC (@RouteIndexesCreationSQL2);
	--EXEC (@RouteIndexesCreationSQL3)
	--EXEC (@RouteIndexesCreationSQL4);
	--EXEC (@RouteIndexesCreationSQL5);

	--EXEC bp_SetSystemMessage 'BuildRoutes: Building Other INDEX for RouteOptions', @Message
 --	EXEC (@RouteOptionIndexesCreationSQL2)
	--EXEC (@RouteOptionIndexesCreationSQL3)



	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: End', @Message

	-- Get the last changed route of the route build
	SELECT @UpdateStamp = MAX(Updated) FROM [Route]

	-- Set Status to NULL (Not Running)
	EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL

	RETURN;
END