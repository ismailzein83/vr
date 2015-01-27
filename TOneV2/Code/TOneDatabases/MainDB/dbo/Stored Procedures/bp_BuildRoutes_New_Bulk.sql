--
-- Build the Route and Route Option Tables
--
CREATE  PROCEDURE [dbo].[bp_BuildRoutes_New_Bulk]
	 @RebuildCodeSupply char(1) = 'Y' -- Y to Rebuild the code supply table
	,@CheckToD char(1) = 'Y' -- Y to Check the ToD Considerations after the build
	,@CheckSpecialRequests char(1) = 'Y' -- Y to Check special requests and alter the tables accordingly
	,@CheckRouteBlocks char(1) = 'Y' -- Y to check route blocks	
	,@MaxSuppliersPerRoute INT = 10
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
	
	-- Make sure to Define Parameters for table creation properly
	IF ISNULL(@SORT_IN_TEMPDB, '') = '' SET @SORT_IN_TEMPDB = 'ON'
	IF @SORT_IN_TEMPDB <> 'ON' SET @SORT_IN_TEMPDB = 'OFF'
	IF ISNULL(@RoutingTableFileGroup, '') = '' SET @RoutingTableFileGroup = 'PRIMARY'
	IF ISNULL(@RoutingIndexesFileGroup, '') = '' SET @RoutingIndexesFileGroup = 'PRIMARY'

	DECLARE @RouteTableCreationSQL nvarchar(max) 
	DECLARE @RouteIndexesCreationSQL nvarchar(max)
	DECLARE @RouteOptionTableCreationSQL nvarchar(max) 
	DECLARE @RouteOptionIndexesCreationSQL nvarchar(max)

	SET @RouteTableCreationSQL = '
		-- Temp Route
		CREATE TABLE [dbo].[TempRoute](
			[RouteID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
			[CustomerID] [varchar](5) NOT NULL,
			[ProfileID] [int] NULL,
			[Code] [varchar](20) NULL,
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
		CREATE NONCLUSTERED INDEX [IX_Route_Code] ON [dbo].[TempRoute]([Code] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IX_Route_Customer] ON [dbo].[TempRoute]([CustomerID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IX_Route_Updated] ON [dbo].[TempRoute]([Updated] DESC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IX_Route_Zone] ON [dbo].[TempRoute]([OurZoneID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
	';

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
		CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[TempRouteOption]([RouteID] ASC)
	';

	SET @RouteOptionIndexesCreationSQL = '
		CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[TempRouteOption]([RouteID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') 
		CREATE NONCLUSTERED INDEX [IDX_RouteOption_SupplierZoneID] ON [dbo].[TempRouteOption]([SupplierZoneID] ASC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
		CREATE NONCLUSTERED INDEX [IDX_RouteOption_Updated] ON [dbo].[TempRouteOption]([Updated] DESC) WITH (SORT_IN_TEMPDB = '+@SORT_IN_TEMPDB+') ON [' + @RoutingIndexesFileGroup + ']
	';

	-- Create Temp Working Route Table
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TempRoute]') AND type in (N'U'))
	BEGIN
		DROP TABLE [dbo].[TempRoute]
	END
	EXEC (@RouteTableCreationSQL);

	  WITH OurZoneRates AS 
    (
    SELECT  a.PROFILEID , zr.ZoneID, zr.SupplierID, zr.CustomerID, zr.NormalRate,
           zr.OffPeakRate, zr.WeekendRate, zr.ServicesFlag,
           ROW_NUMBER() OVER (ORDER BY getdate()) AS RowNumber
    FROM   ZoneRate zr WITH(NOLOCK)
    LEFT JOIN CarrierAccount a WITH(NOLOCK) ON  a.CarrierAccountID = zr.CustomerID 
    WHERE  zr.SupplierID = 'SYS'
     )
	INSERT INTO [dbo].[TempRoute]
	(
		CustomerID,
		ProfileID,
		Code,
		OurZoneID,
		OurActiveRate,
		OurNormalRate,
		OurOffPeakRate,
		OurWeekendRate,
		OurServicesFlag,
		[State],		
		Updated	
	)
	SELECT R.CustomerID,
       R.ProfileID,
       CM.Code,
       CM.SupplierZoneID,
       R.NormalRate,
       R.NormalRate,
       R.OffPeakRate,
       R.WeekendRate,
       R.ServicesFlag,
       @State_Enabled,
       DATEADD(SS,-RowNumber,@UpdateStamp)

FROM   CodeMatch CM WITH (NOLOCK) 
      INNER JOIN OurZoneRates R WITH (NOLOCK) ON  CM.SupplierZoneID = R.ZoneID

	OPTION (RECOMPILE)

	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Routes Inserted', @Message
	
	EXEC (@RouteIndexesCreationSQL)  	
  	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Routes Indexes Re-Created', @Message

	-- Temp Route Option
	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TempRouteOption]') AND type in (N'U'))
	BEGIN
		DROP TABLE [dbo].[TempRouteOption]
	END;
	--EXEC (@RouteOptionTableCreationSQL);

	-- EXECUTE( @TruncateLogSQL )
/*
	DECLARE @CurrentCustomer VARCHAR(5)
	DECLARE @CurrentProfile INT 
	
	DECLARE customersCursor CURSOR LOCAL FAST_FORWARD FOR 
		SELECT 
			DISTINCT CustomerID 
			FROM [dbo].[TempRoute]
			ORDER BY CustomerID;  

	OPEN customersCursor
	FETCH NEXT FROM customersCursor INTO @CurrentCustomer

	WHILE @@FETCH_STATUS = 0
	BEGIN
		BEGIN TRANSACTION; 
		
		SELECT @CurrentProfile = ProfileID FROM CarrierAccount ca WHERE ca.CarrierAccountID = @CurrentCustomer;

		WITH TheOptions AS
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
				@State_Enabled AS [State]
			FROM
				[dbo].[TempRoute] Rt WITH (NOLOCK, INDEX(IX_Route_Customer))
					, CodeSupply CS WITH (NOLOCK, INDEX(IX_CodeSupply_Code))
					, CarrierAccount S WITH (NOLOCK) -- Supplier
				WHERE Rt.CustomerID = @CurrentCustomer
					AND Rt.Code = CS.Code COLLATE DATABASE_DEFAULT	
					AND CS.SupplierID = S.CarrierAccountID COLLATE DATABASE_DEFAULT
					AND	S.ActivationStatus <> @Account_Inactive 
					AND S.RoutingStatus <> @Account_Blocked 
					AND S.RoutingStatus <> @Account_BlockedOutbound
					AND S.ProfileID <> @CurrentProfile -- Prevent Looping
					AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
					AND CS.SupplierNormalRate <= Rt.OurNormalRate				
		)
*/
		-- Build Route Options!
		WITH TheOptions AS
		(
			SELECT 
				Rt.RouteID,
				Rt.CustomerId,
				Rt.Code,
				CS.SupplierID,
				CS.SupplierZoneID,
				CS.SupplierNormalRate AS SupplierActiveRate,
				CS.SupplierNormalRate,
				CS.SupplierOffPeakRate,
				CS.SupplierWeekendRate,
				CS.SupplierServicesFlag,
				(ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.SupplierNormalRate)) as RowNumber,
				ROW_NUMBER() OVER (ORDER BY getdate()) AS RowIdentity,
				1 AS NumberOfTries,
				@State_Enabled AS [State]
			FROM
					[dbo].tempRoute Rt WITH (NOLOCK)
					left join CodeSupply CS WITH (NOLOCK, INDEX(IX_CodeSupply_Code)) on Rt.Code = CS.Code COLLATE DATABASE_DEFAULT	
				WHERE 
					
--					And Rt.Code = CS.Code COLLATE DATABASE_DEFAULT	
					RT.ProfileID <> CS.ProfileID -- Prevent Looping
					AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
					AND CS.SupplierNormalRate <= Rt.OurNormalRate
					
		)
		Select 
			TheOptions.RouteID,
			TheOptions.SupplierID,
			TheOptions.SupplierZoneID,
			TheOptions.SupplierActiveRate,
			TheOptions.SupplierNormalRate,
			TheOptions.SupplierOffPeakRate,
			TheOptions.SupplierWeekendRate,
			TheOptions.SupplierServicesFlag,
			0 Priority,
			TheOptions.NumberOfTries,
			TheOptions.[State],
			DATEADD(MS,- TheOptions.RowIdentity,@UpdateStamp) Updated
		Into TempRouteOption
		FROM TheOptions 
		WHERE RowNumber <= @MaxSuppliersPerRoute 

/*		INSERT INTO [dbo].[TempRouteOption]
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
			GETDATE()
		FROM TheOptions WHERE RowNumber <= @MaxSuppliersPerRoute 
*/		
--		COMMIT
/*		
		SET @Message = CONVERT(varchar, getdate(), 121)
		SET @MessageID = 'BuildRoutes: Customer: ' + @CurrentCustomer
		EXEC bp_SetSystemMessage @MessageID, @Message
		
		FETCH NEXT FROM customersCursor INTO @CurrentCustomer
	END

	CLOSE customersCursor
	DEALLOCATE customersCursor

*/	-- EXECUTE( @TruncateLogSQL )

	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Route Options Inserted', @Message

	EXEC (@RouteOptionIndexesCreationSQL)
	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Indexes Built for RouteOptions', @Message
	
	---- Rename Temp to Route and Route Options
	--BEGIN TRANSACTION		
	--	DROP TABLE [dbo].[Route]
	--	EXEC sp_rename 'TempRoute', 'Route'
	--	-- Message
	--	SET @Message = CONVERT(varchar, getdate(), 121)
	--	EXEC bp_SetSystemMessage 'BuildRoutes: [TempRoutes] Renamed to [Route]', @Message
		
	--	DROP TABLE [dbo].[RouteOption]
	--	EXEC sp_rename 'TempRouteOption', 'RouteOption'
	--	-- Message
	--	SET @Message = CONVERT(varchar, getdate(), 121)
	--	EXEC bp_SetSystemMessage 'BuildRoutes: [TempRouteOptions] Renamed to [RouteOptions]', @Message
	--COMMIT TRANSACTION

	-- Check ToD
/*
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
		EXEC bp_UpdateRoutingFromOverrides_WithPercentage @Check='N'
		
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Special Requests Processed', @Message
	End

	-- Route Overrides
	EXEC bp_UpdateRoutingFromOverrides_With

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
*/		

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



	-- Message
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
		EXEC bp_UpdateRoutesFromSpecialRequests_New @Check='N'
		
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Special Requests Processed', @Message
	End

	-- Route Overrides
	--EXEC bp_UpdateRoutingFromOverrides_WithPercentage_New

	-- Check Route Blocks
	IF @CheckRouteBlocks = 'Y'
	BEGIN
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Processing Route Blocks', @Message

		-- Route Blocks
		EXEC bp_UpdateRoutesFromRouteBlocks_New @Check='N'
		
		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Route Blocks Processed', @Message
	END

 


	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: End', @Message

	-- Get the last changed route of the route build
	SELECT @UpdateStamp = MAX(Updated) FROM [Route]

	-- Set Status to NULL (Not Running)
	EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL

	RETURN;
END

--declare @UpdateStamp datetime 
-- @UpdateStamp output