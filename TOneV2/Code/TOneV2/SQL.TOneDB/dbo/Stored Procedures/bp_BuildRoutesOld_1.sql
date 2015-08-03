--
-- Build the Route and Route Option Tables
--
CREATE PROCEDURE [dbo].[bp_BuildRoutesOld]
	 @RebuildCodeSupply char(1) = 'Y' -- Y to Rebuild the code supply table
	,@CheckToD char(1) = 'Y' -- Y to Check the ToD Considerations after the build
	,@CheckSpecialRequests char(1) = 'Y' -- Y to Check special requests and alter the tables accordingly
	,@CheckRouteBlocks char(1) = 'Y' -- Y to check route blocks	
	,@MaxSuppliersPerRoute INT = 10
	,@IncludeBlockedZones CHAR(1) = 'N'
	,@UpdateStamp datetime output
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

		-- Fix Unsold Zones 
		EXEC bp_FixUnsoldZonesForRouteBuild 
		
		-- Rebuild the Code Supply
		EXEC bp_BuildCodeSupply 

		-- Message
		SET @Message = CONVERT(varchar, getdate(), 121)
		EXEC bp_SetSystemMessage 'BuildRoutes: Code Supply Rebuilt', @Message
	END
	
	ALTER TABLE [Route] NOCHECK CONSTRAINT ALL
	TRUNCATE TABLE [Route]

	-- DROP Route INDEXES
	DROP INDEX [IX_Route_Code] ON [Route]
	DROP INDEX [IX_Route_Customer] ON [Route]
	DROP INDEX [IX_Route_Zone] ON [Route]
	DROP INDEX [IX_Route_ServicesFlag] ON [Route]
	DROP INDEX [IX_Route_Updated] ON [Route]

	INSERT INTO Route 
	(
		CustomerID,
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
	SELECT 
		R.CustomerID, 
		CM.Code, 
		CM.SupplierZoneID,
		R.NormalRate,
		R.NormalRate,
		R.OffPeakRate,
		R.WeekendRate,
		R.ServicesFlag,
		@State_Enabled,
		@UpdateStamp
	FROM
		CarrierAccount A WITH (NOLOCK, index(PK_CarrierAccount))
		INNER JOIN ZoneRate R WITH (NOLOCK, index(IX_ZoneRate_Customer, IX_ZoneRate_Supplier, IX_ZoneRate_Zone)) ON A.CarrierAccountID = R.CustomerID
		INNER JOIN CodeMatch CM WITH (NOLOCK, index(IDX_CodeMatch_Zone)) ON CM.SupplierZoneID = R.ZoneID AND R.SupplierID = 'SYS'
	WHERE
			A.ActivationStatus <> @Account_Inactive
		AND A.RoutingStatus <> @Account_Blocked
		AND A.RoutingStatus <> @Account_BlockedInbound
	ORDER BY R.CustomerID, CM.Code
	OPTION (RECOMPILE)

	ALTER TABLE [Route] CHECK CONSTRAINT ALL
	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Routes Inserted', @Message

	CREATE INDEX [IX_Route_Code] ON [Route]([Code] ASC) -- ON INDEXES
	CREATE INDEX [IX_Route_Zone] ON [Route]([OurZoneID] ASC) -- ON INDEXES
	CREATE INDEX [IX_Route_Customer] ON [Route]([CustomerID] ASC) -- ON INDEXES
	CREATE INDEX [IX_Route_ServicesFlag] ON [Route]([OurServicesFlag] ASC) -- ON INDEXES
	CREATE INDEX [IX_Route_Updated] ON [Route](Updated DESC) -- ON INDEXES
  	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Routes Indexes Re-Created', @Message

	-- Clear Route Options
	TRUNCATE TABLE [RouteOption]
	-- DROP INDEX IDX_RouteOption_RouteID ON RouteOption
	DROP INDEX IDX_RouteOption_SupplierZoneID ON RouteOption
	DROP INDEX IDX_RouteOption_Updated ON RouteOption
	ALTER TABLE [RouteOption] NOCHECK CONSTRAINT ALL

	-- EXECUTE( @TruncateLogSQL )
	DECLARE @CurrentCustomer VARCHAR(5)
	DECLARE @CurrentProfile INT 
	
	DECLARE customersCursor CURSOR LOCAL FAST_FORWARD FOR 
		SELECT 
			DISTINCT CustomerID 
			FROM [Route] 
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
				[Route] Rt WITH (NOLOCK, INDEX(IX_Route_Customer))
					, CodeSupply CS WITH (NOLOCK, INDEX(IX_CodeSupply_Code, IX_CodeSupply_Supplier))
					, CarrierAccount S WITH (NOLOCK) -- Supplier
				WHERE Rt.CustomerID = @CurrentCustomer
					AND Rt.Code = CS.Code	
					AND CS.SupplierID = S.CarrierAccountID
					AND	S.ActivationStatus <> @Account_Inactive 
					AND S.RoutingStatus <> @Account_Blocked 
					AND S.RoutingStatus <> @Account_BlockedOutbound
					AND S.ProfileID <> @CurrentProfile -- Prevent Looping
					AND (CS.SupplierServicesFlag & Rt.OurServicesFlag) = Rt.OurServicesFlag
					AND CS.SupplierNormalRate < Rt.OurNormalRate				
		)

		-- Build Route Options!
		INSERT INTO RouteOption
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
		
		COMMIT
		
		SET @Message = CONVERT(varchar, getdate(), 121)
		SET @MessageID = 'BuildRoutes: Customer: ' + @CurrentCustomer
		EXEC bp_SetSystemMessage @MessageID, @Message
		
		FETCH NEXT FROM customersCursor INTO @CurrentCustomer
	END

	CLOSE customersCursor
	DEALLOCATE customersCursor

	-- EXECUTE( @TruncateLogSQL )

	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: Route Options Inserted', @Message

	ALTER TABLE [RouteOption] CHECK CONSTRAINT ALL

	-- CREATE INDEX IDX_RouteOption_RouteID ON RouteOption(RouteID) -- ON INDEXES
	-- Message
	-- SET @Message = CONVERT(varchar, getdate(), 121)
	-- EXEC bp_SetSystemMessage 'BuildRoutes: RouteID Index Built for RouteOptions', @Message

	CREATE INDEX IDX_RouteOption_SupplierZoneID ON RouteOption(SupplierZoneID) -- ON INDEXES
	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: SupplierZoneID Index Built for RouteOptions', @Message

	CREATE INDEX IDX_RouteOption_Updated ON RouteOption(Updated) -- ON INDEXES
	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: [Updated] Index Built for RouteOptions', @Message
	
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
	EXEC bp_UpdateRoutingFromOverrides

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

	-- Message
	SET @Message = CONVERT(varchar, getdate(), 121)
	EXEC bp_SetSystemMessage 'BuildRoutes: End', @Message

	-- Get the last changed route of the route build
	SELECT @UpdateStamp = MAX(Updated) FROM [Route]

	-- Set Status to NULL (Not Running)
	EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL

	RETURN;
END