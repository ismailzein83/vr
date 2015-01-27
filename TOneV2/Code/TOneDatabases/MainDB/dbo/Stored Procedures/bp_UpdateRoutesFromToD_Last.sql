

CREATE PROCEDURE [dbo].[bp_UpdateRoutesFromToD_Last]
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

	WITH 
    -- Find the Effective TODs		
	theTODs(ZoneID,SupplierID,CustomerID,RateType) AS 
	(
	  SELECT t.ZoneID AS ZoneID,  
			 t.SupplierID AS SupplierID,
			 t.CustomerID AS CustomerID,
			 t.RateType AS RateType
	  FROM ToDConsideration t WITH(NOLOCK)
	  WHERE
	      t.CustomerID='SYS'
	  AND t.IsEffective ='Y'
	  AND t.IsActive = 'Y'
	)
	 -- Find the CodeSupply related to theTODs
	 ,CodeSupplyTODs(Code,SupplierID,SupplierZoneID,RateType) AS
	(
		SELECT cs.Code AS Code,
			   cs.SupplierID AS SupplierID, 
			   cs.SupplierZoneID AS SupplierZoneID, 
			   t.RateType AS RateType 
		FROM CodeSupply cs WITH(NOLOCK,INDEX(IX_CodeSupply_Supplier,IX_CodeSupply_Zone))
		    ,theTODs t WITH(NOLOCK)
		WHERE 
			cs.SupplierID =  t.SupplierID
		AND cs.SupplierZoneID = t.ZoneID
	)
	
	-- Find All CodeSupply related to the code effected by TODs
	,CodeSupplyAll(Code,SupplierID,SupplierZoneID,SupplierNormalRate,SupplierOffPeakRate,
				  SupplierWeekendRate,SupplierServicesFlag,IsActive,RateType) AS
	(
		SELECT cs.Code AS Code, 
			   cs.SupplierID AS SupplierID, 
			   cs.SupplierZoneID AS SupplierZoneID, 
			   cs.SupplierNormalRate AS SupplierNormalRate,
			   cs.SupplierOffPeakRate AS SupplierOffPeakRate,  
			   cs.SupplierWeekendRate AS SupplierWeekendRate, 
			   cs.SupplierServicesFlag AS SupplierServicesFlag,
			   (CASE WHEN cs.SupplierID = cst.SupplierID AND cs.SupplierZoneID = cst.SupplierZoneID THEN 'Y' ELSE 'N' END) AS IsActive,
			   (CASE WHEN cs.SupplierID = cst.SupplierID AND cs.SupplierZoneID = cst.SupplierZoneID THEN cst.RateType ELSE 0 END) AS RateType
		FROM CodeSupply cs WITH(NOLOCK,INDEX(IX_CodeSupply_Supplier,IX_CodeSupply_Zone))
		  , CodeSupplyTODs cst WITH(NOLOCK)
		WHERE cs.Code = cst.Code
	)
	
	-- Route Options to insert
	,theOptions(RouteID,SupplierID,SupplierZoneID,SupplierActiveRate
               ,SupplierNormalRate,SupplierOffPeakRate,SupplierWeekendRate,SupplierServicesFlag
			   ,RowNumber,NumberOfTries,[State]) AS 
	(	
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
			[dbo].[Route] rt WITH (NOLOCK, INDEX(IX_Route_Code,IX_Route_Customer))
				--, CarrierAccount s WITH (NOLOCK) -- Supplier
				, CodeSupplyAll cs WITH (NOLOCK)
		WHERE 
				rt.Code = cs.Code COLLATE DATABASE_DEFAULT	
			AND rt.CustomerID <> cs.SupplierID	
--			AND cs.SupplierID = s.CarrierAccountID COLLATE DATABASE_DEFAULT
--			AND s.CarrierAccountID <> rt.CustomerID -- Prevent Looping
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
	)
	--,TheOptionsResult(RouteID,SupplierID,SupplierZoneID,SupplierActiveRate,SupplierNormalRate,SupplierOffPeakRate,SupplierWeekendRate,SupplierServicesFlag,NumberOfTries,[State]) AS
	--(
		SELECT o.RouteID AS RouteID,
			   o.SupplierID AS SupplierID,
			   o.SupplierZoneID AS SupplierZoneID,
			   o.SupplierActiveRate AS SupplierActiveRate,
			   o.SupplierNormalRate AS SupplierNormalRate,
			   o.SupplierOffPeakRate AS SupplierOffPeakRate,
			   o.SupplierWeekendRate AS SupplierWeekendRate,
			   o.SupplierServicesFlag AS SupplierServicesFlag,
			   o.NumberOfTries AS NumberOfTries,
			   o.[State] AS [State]
		INTO #TheOptionsResult
		FROM theOptions o
		GROUP BY RouteID,SupplierID,SupplierZoneID,
				 SupplierActiveRate,SupplierNormalRate,SupplierOffPeakRate,
				 SupplierWeekendRate,SupplierServicesFlag,NumberOfTries,[State]
	--)
	
	-- Delete old Route Option
	DELETE RouteOption 
	FROM RouteOption ro WITH(NOLOCK)
	WHERE ro.RouteID IN (SELECT r.RouteID FROM #TheOptionsResult r) 

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
	FROM #TheOptionsResult o WITH(NOLOCK)
	
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
			
    DELETE RouteOption 
    FROM RouteOption ro WITH(NOLOCK), [ROUTE] r WITH(NOLOCK)
    WHERE r.RouteID = ro.RouteID
    AND ro.SupplierActiveRate > r.OurActiveRate
   
	-- Now Set to Updated the Routes That have the Route Options Updated
	EXEC bp_SignalRouteUpdatesFromOptions @UpdateStamp=@UpdateStamp, @UpdateType='TOD'
	
	DROP TABLE #TheOptionsResult
	COMMIT
	
	IF @Check = 'Y'
	BEGIN
		EXEC bp_SetSystemMessage 'BuildRoutes: Status', @message = NULL
	END