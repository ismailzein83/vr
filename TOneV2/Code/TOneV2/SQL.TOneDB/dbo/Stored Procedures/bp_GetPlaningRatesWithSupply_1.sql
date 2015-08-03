CREATE PROCEDURE [dbo].[bp_GetPlaningRatesWithSupply](
	@CustomerID varchar(10) = NULL, 
	@ExcludedRate float = 10, 
	@ServicesFlag smallint = NULL ,
	@ZoneID int = NULL,
	@CurrencyID varchar(3) = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
    RETURN
	DECLARE @Account_Inactive tinyint
	DECLARE @Account_Blocked tinyint
	DECLARE @Account_BlockedInbound tinyint
	DECLARE @Account_BlockedOutbound tinyint
	
	SELECT @Account_Inactive = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.ActivationStatus' AND Name = 'Inactive'
	SELECT @Account_Blocked = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'Blocked'
	SELECT @Account_BlockedInbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedInbound'	
	SELECT @Account_BlockedOutbound  = Value FROM Enumerations WHERE Enumeration LIKE 'TABS.RoutingStatus' AND Name = 'BlockedOutbound'	

	DECLARE @LastExchangeRate REAL
	
	DECLARE @ZoneIsPending CHAR(1)
	SET @ZoneIsPending  = ISNULL((SELECT 'Y' FROM Zone z WITH(NOLOCK) WHERE z.ZoneID = @ZoneID AND z.IsEffective = 'N'), 'N')
		
	IF @CurrencyID IS NOT NULL 
		SELECT @LastExchangeRate = LastRate FROM Currency WITH(NOLOCK) WHERE CurrencyID = @CurrencyID
	ELSE 
		SET @LastExchangeRate = 1
	
    DECLARE @OurRates TABLE(ZoneID int PRIMARY KEY, Rate real, OffPeakRate real, WeekendRate real, ServicesFlag smallint);
	
	IF @ZoneIsPending = 'N'
	  BEGIN
		INSERT INTO @OurRates (ZoneID, Rate, OffPeakRate, WeekendRate, ServicesFlag)
			SELECT  PR.ZoneID, 
					PR.Rate * @LastExchangeRate, 
					PR.OffPeakRate * @LastExchangeRate, 
					PR.WeekendRate * @LastExchangeRate, 
					PR.ServicesFlag
				FROM PlaningRate PR WITH(NOLOCK), RatePlan RP WITH(NOLOCK) WHERE
						PR.RatePlanID = RP.RatePlanID
					AND RP.CustomerID= @CustomerID 
					AND (@ZoneID IS NULL OR PR.ZoneID = @ZoneID)
					AND PR.Rate < @ExcludedRate; 

			SELECT
				OZ.ZoneID as OurZoneID, OZ.Name AS OurZone,
				R.Rate as OurNormalRate, R.OffPeakRate as OurOffPeakRate, R.WeekendRate AS OurWeekendRate, R.ServicesFlag AS OurServicesFlag,
				SR.SupplierID, ZM.SupplierZoneID AS SupplierZoneID, SZ.Name AS SupplierZone,
				SR.NormalRate * @LastExchangeRate AS SupplierNormalRate, 
				SR.OffPeakRate * @LastExchangeRate AS SupplierOffPeakRate, 
				SR.WeekendRate * @LastExchangeRate AS SupplierWeekendRate, 
				SR.ServicesFlag AS SupplierServicesFlag
				,RateTable.EndEffectiveDate -- EndEffectiveDate
				,null AS DurationsInMinutes
				,null AS ASR
				,null AS ACD
			FROM
				Zone OZ WITH(NOLOCK)
				LEFT JOIN @OurRates R ON OZ.ZoneID = R.ZoneID AND R.Rate IS NOT NULL
				LEFT JOIN ZoneMatch ZM WITH(NOLOCK) ON ZM.OurZoneID = OZ.ZoneID 
					AND ZM.SupplierZoneID IN 
						(
						SELECT SZ.ZoneID FROM Zone SZ WITH(NOLOCK) WHERE SZ.SupplierID IN 
							(
							SELECT ca.CarrierAccountID FROM CarrierAccount ca WITH(NOLOCK)
								WHERE 
									ca.ActivationStatus <> @Account_Inactive 
								AND ca.RoutingStatus <> @Account_Blocked 
								AND ca.RoutingStatus <> @Account_BlockedOutbound
							)						
						) 
				LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = ZM.SupplierZoneID
				LEFT JOIN ZoneRate SR WITH(NOLOCK,INDEX(IX_ZoneRate_Zone)) ON 
							ZM.SupplierZoneID = SR.ZoneID 
						AND SR.SupplierID <> ISNULL(@CustomerID,'')
						AND SR.SupplierID <> 'SYS' 
						-- AND (@AllRates = 'Y' OR SR.NormalRate < R.Rate)
						-- AND SR.ServicesFlag & ISNULL(@ServicesFlag, R.ServicesFlag) = ISNULL(@ServicesFlag, R.ServicesFlag) 
				LEFT JOIN Rate RateTable WITH(NOLOCK, INDEX(IX_Rate_Zone)) ON RateTable.ZoneID = SR.ZoneID AND RateTable.IsEffective = 'Y'
			WHERE 
				OZ.SupplierID = 'SYS'
				AND (ZM.SupplierZoneID IS NULL OR SZ.ZoneID IS NOT NULL)
				AND (@ZoneID IS NULL OR OZ.ZoneID = @ZoneID)		
			ORDER BY OurZone, SupplierNormalRate
			OPTION (RECOMPILE)
	  END
	-- Future Zone 
	ELSE
	  BEGIN
		
		DECLARE @BED DATETIME
		DECLARE @DefaultRate REAL 
		SET @DefaultRate = 0.0  
		SELECT @BED = z.BeginEffectiveDate FROM Zone z WITH(NOLOCK) WHERE z.ZoneID = @ZoneID
		
		CREATE TABLE #ExactMatchZones (SupplierZoneID INT PRIMARY KEY, SupplierID VARCHAR(5))
		CREATE INDEX IX_SupplierID ON #ExactMatchZones (SupplierID)
		
		CREATE TABLE #Codes (Code VARCHAR(15) PRIMARY KEY)
		INSERT INTO #Codes SELECT oc.Code FROM Code oc WITH(NOLOCK) WHERE oc.ZoneID = @ZoneID AND oc.BeginEffectiveDate <= @BED AND (oc.EndEffectiveDate IS NULL OR oc.EndEffectiveDate > @BED)
		
		INSERT INTO #ExactMatchZones
			SELECT DISTINCT sz.ZoneID, sz.SupplierID FROM Zone sz WITH(NOLOCK), Code sc WITH(NOLOCK) 
				WHERE sc.ZoneID = sz.ZoneID 
					AND sz.SupplierID <> 'SYS'  
					AND sz.BeginEffectiveDate <= @BED AND (sz.EndEffectiveDate IS NULL OR sz.EndEffectiveDate > @BED)
					AND sc.BeginEffectiveDate <= @BED AND (sc.EndEffectiveDate IS NULL OR sc.EndEffectiveDate > @BED)
					AND sc.Code COLLATE Latin1_General_BIN IN (SELECT oc.Code COLLATE Latin1_General_BIN FROM #Codes oc)
		
		SELECT		
			@ZoneID as OurZoneID, (SELECT OZ.Name FROM Zone oz WITH(NOLOCK) WHERE oz.ZoneID = @ZoneID) AS OurZone,
			@DefaultRate as OurNormalRate, @DefaultRate as OurOffPeakRate, @DefaultRate AS OurWeekendRate, @ServicesFlag AS OurServicesFlag,
			SZ.SupplierID, SZ.ZoneID AS SupplierZoneID, SZ.Name AS SupplierZone,
			cast(R.Rate * @LastExchangeRate AS REAL) AS SupplierNormalRate, 
			cast(R.OffPeakRate * @LastExchangeRate AS REAL) AS SupplierOffPeakRate, 
			cast(R.WeekendRate * @LastExchangeRate AS REAL) AS SupplierWeekendRate, 
			R.ServicesFlag AS SupplierServicesFlag
			,r.EndEffectiveDate 
			,null --TS.DurationsInMinutes
			,null --TS.ASR
			,null --TS.ACD
		FROM Zone SZ WITH(NOLOCK), Rate r WITH(NOLOCK) 
		WHERE 
				sz.SupplierID <> 'SYS'
			AND sz.BeginEffectiveDate <= @BED AND (sz.EndEffectiveDate IS NULL OR sz.EndEffectiveDate > @BED)
			AND sz.SupplierID NOT IN (SELECT ca.CarrierAccountID FROM CarrierAccount ca WITH(NOLOCK) WHERE ca.RoutingStatus IN (@Account_Blocked, @Account_BlockedOutbound) OR ca.ActivationStatus = @Account_Inactive)
			AND NOT EXISTS (SELECT rb.ZoneID FROM RouteBlock rb WITH(NOLOCK) WHERE rb.SupplierID = sz.SupplierID AND rb.ZoneID = sz.ZoneID AND rb.IsEffective = 'Y')
			AND 
			(
				sz.ZoneID IN (SELECT em.SupplierZoneID FROM #ExactMatchZones em WHERE em.SupplierID = sz.SupplierID COLLATE Latin1_General_BIN)
				OR
				EXISTS
				(
					SELECT * FROM Code sc, #Codes oc 
					WHERE sc.ZoneID = sz.ZoneID
						AND sc.BeginEffectiveDate <= @BED AND (sc.EndEffectiveDate IS NULL OR sc.EndEffectiveDate > @BED)
						AND sc.ZoneID NOT IN (SELECT em.SupplierZoneID FROM #ExactMatchZones em)
						AND 
						(
							(sc.Code LIKE (oc.Code + '%') COLLATE Latin1_General_BIN) 
							OR 
							(oc.Code LIKE (sc.Code + '%') COLLATE Latin1_General_BIN)
						) 	
				) 
			)
			AND r.ZoneID = SZ.ZoneID
			AND r.IsEffective ='Y'
			AND r.BeginEffectiveDate <= @BED AND (r.EndEffectiveDate IS NULL OR r.EndEffectiveDate > @BED)
			-- AND r.ServicesFlag & ISNULL(@ServicesFlag, 0) = ISNULL(@ServicesFlag, 0) 

		ORDER BY SupplierNormalRate ASC
		
		DROP TABLE #ExactMatchZones
		
	  END
END