
CREATE PROCEDURE dbo.bp_UpdateRoutesFromToD_New
	@Check char(1) = 'Y'

AS

	SET NOCOUNT ON

	IF @Check = 'Y' 
	BEGIN	
		DECLARE @IsRunning char(1)
		EXEC bp_IsRouteBuildRunning @IsRunning output
		IF @IsRunning = 'Y' 
		BEGIN
			PRINT 'Build Routes is already Runnning'
			RETURN 
		END 
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = 'Updating From ToD Considerations'
		EXEC bp_ResetRoutes @UpdateType = 'TOD'
	END	

	DECLARE @UpdateStamp datetime
	SET @UpdateStamp = getdate()

	BEGIN TRANSACTION

	-- Update Rates for Routes that have a ToD active
	UPDATE [Route]
		SET OurActiveRate = 
			CASE 
				WHEN t.RateType = 1 THEN [Route].OurOffPeakRate -- OffPeak
				WHEN t.RateType = 2 THEN [Route].OurWeekendRate -- Weekend 
				WHEN t.RateType = 4 THEN [Route].OurWeekendRate -- Holiday
				ELSE [Route].OurNormalRate
			END
			, Updated = @UpdateStamp
			, IsToDAffected = 'Y'
	FROM [Route], ToDConsideration t
		WHERE 
			t.IsEffective='Y'
			AND t.IsActive='Y'
			AND t.ZoneID=[Route].OurZoneID
			AND t.CustomerID = [Route].CustomerID;
			
	-- Route Options to insert
	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	-- Set Enumerations
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	
	
	DECLARE @theTODs TABLE (ZoneID INT,SupplierID VARCHAR(5),CustomerID VARCHAR(5),RateType TINYINT)
	INSERT INTO @theTODs
	SELECT t.ZoneID AS ZoneID,  
			 t.SupplierID AS SupplierID, 
			 t.CustomerID AS CustomerID,
			 t.RateType AS RateType
	FROM ToDConsideration t WITH(NOLOCK)
	WHERE 
	      t.CustomerID='SYS'
	  AND t.IsEffective ='Y' 
	  AND t.IsActive = 'Y'
	  
	
    DECLARE @CodeSupplyFlagged TABLE (Code VARCHAR(15),SupplierID VARCHAR(5),SupplierZoneID INT
								  ,SupplierNormalRate REAL,SupplierOffPeakRate REAL,SupplierWeekendRate REAL
								  ,SupplierServicesFlag SMALLINT,IsActive CHAR(1))
	INSERT INTO @CodeSupplyFlagged
	SELECT cs.Code, 
		cs.SupplierID, 
		cs.SupplierZoneID, 
		cs.SupplierNormalRate,
		cs.SupplierOffPeakRate, 
		cs.SupplierWeekendRate, 
		cs.SupplierServicesFlag,
		(CASE WHEN cs.SupplierID = tod.SupplierID AND cs.SupplierZoneID = tod.zoneid THEN 'Y' ELSE 'N' END) AS IsActive
	FROM CodeSupply cs WITH (NOLOCK)
	LEFT JOIN @theTODs  tod ON tod.SupplierID = cs.SupplierID AND cs.SupplierZoneID = tod.ZoneID
	
	DECLARE @theOptions TABLE (RouteID INT,SupplierID VARCHAR(5),SupplierZoneID INT
							,SupplierActiveRate REAL,SupplierNormalRate REAL,SupplierOffPeakRate REAL,SupplierWeekendRate REAL
							,SupplierServicesFlag SMALLINT,RowNumber TINYINT,NumberOfTries TINYINT,[State] TINYINT)
	INSERT INTO @theOptions
	SELECT
		rt.RouteID AS RouteID,
		cs.SupplierID AS SupplierID,
		cs.SupplierZoneID AS SupplierZoneID, 
		cs.SupplierNormalRate AS SupplierActiveRate,
		cs.SupplierNormalRate AS SupplierNormalRate,
		cs.SupplierOffPeakRate AS SupplierOffPeakRate,
		cs.SupplierWeekendRate AS SupplierWeekendRate,
		cs.SupplierServicesFlag AS SupplierServicesFlag,
		(ROW_NUMBER() OVER (PARTITION BY Rt.RouteID ORDER BY CS.SupplierNormalRate)) as RowNumber,
		1 AS NumberOfTries,
		1 AS [State]
	FROM
	[dbo].[Route] rt WITH (NOLOCK)--, INDEX(IX_Route_Customer))
		, CarrierAccount s WITH (NOLOCK) -- Supplier
		, @CodeSupplyFlagged cs
	WHERE 
			rt.Code = cs.Code COLLATE DATABASE_DEFAULT	
		AND cs.SupplierID = s.CarrierAccountID COLLATE DATABASE_DEFAULT
		AND s.CarrierAccountID <> rt.CustomerID -- Prevent Looping
		AND	S.ActivationStatus <> @Account_Inactive 
		AND S.RoutingStatus <> @Account_Blocked 
		AND S.RoutingStatus <> @Account_BlockedOutbound
		AND (cs.SupplierServicesFlag & rt.OurServicesFlag) = rt.OurServicesFlag
		AND
		( cs.SupplierNormalRate < rt.OurActiveRate
		OR 
		 (
			 cs.IsActive = 'Y'
		  AND
			(	
				cs.SupplierOffPeakRate < rt.OurActiveRate 
				OR
				cs.SupplierWeekendRate < rt.OurActiveRate
			)	
		 )
	)		
	
	DELETE RouteOption 
	--FROM RouteOption WITH(NOLOCK,INDEX(IDX_RouteOption_RouteID))
	WHERE RouteID IN (SELECT distinct o.RouteID 
					  FROM @theOptions o)	

	-- Insert new options
	INSERT INTO RouteOption(RouteID,
							SupplierID,
							SupplierZoneID,
							SupplierActiveRate,
							SupplierNormalRate,
							SupplierOffPeakRate,
							SupplierWeekendRate,
							SupplierServicesFlag,
							Priority,
							NumberOfTries,
							[State])
	SELECT 
			o.RouteID AS RouteID,
			o.SupplierID AS SupplierID,
			o.SupplierZoneID AS SupplierZoneID,  
			o.SupplierActiveRate AS SupplierActiveRate,
			o.SupplierNormalRate AS SupplierNormalRate,
			o.SupplierOffPeakRate AS SupplierOffPeakRate,
			o.SupplierWeekendRate AS SupplierWeekendRate,
			o.SupplierServicesFlag AS SupplierServicesFlag,
			0 AS Priority,
			o.NumberOfTries AS NumberOfTries,
			o.[State] AS State 
	FROM @theOptions o
	
	-- Update Rates for Route Options that have a ToD active
	UPDATE [RouteOption]
		SET SupplierActiveRate = 
			CASE 
				WHEN t.RateType = 1 THEN [RouteOption].SupplierOffPeakRate -- OffPeak
				WHEN t.RateType = 2 THEN [RouteOption].SupplierWeekendRate -- Weekend 
				WHEN t.RateType = 4 THEN [RouteOption].SupplierWeekendRate -- Holiday
				ELSE [RouteOption].SupplierNormalRate
			END
			, Updated = @UpdateStamp
	FROM [RouteOption], ToDConsideration t
		WHERE 
			t.IsEffective='Y'
			AND t.IsActive='Y'
			AND t.ZoneID=[RouteOption].SupplierZoneID

	-- Now Set to Updated the Routes That have the Route Options Updated
	EXEC bp_SignalRouteUpdatesFromOptions @UpdateStamp=@UpdateStamp, @UpdateType='TOD'
	
	COMMIT
	
	IF @Check = 'Y'
	BEGIN
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL
	END