-- ======================================================================
-- Author:		Fadi Chamieh, Hassan Kheir Eddine
-- Create date: 2007-09-10
-- Description: Get the Supply Rates for the Zones defined in our System
-- ======================================================================
CREATE PROCEDURE [dbo].[bp_GetZoneRates](
	@CustomerID varchar(10) = NULL,
	@codeFilter varchar(30) = NULL, 
	@ExcludedRate float = 10,
	@zoneNameFilter varchar(50) = NULL, 
	@AllRates char(1) = 'Y',
	@ServicesFlag smallint = 0,
	@ZoneID int = NULL,
	@TopZones int = 10,
	@CurrencyID varchar(3) = NULL,
	@Days int = NULL,
	@Mode varchar(10)= NULL,
	@SupplierIDs VARCHAR(MAX) = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LastExchangeRate REAL
	Set @SupplierIDs = @SupplierIDs + ','
	/*DECLARE @SupplierIDsTable AS TABLE (ID VARCHAR(100))
	INSERT INTO @SupplierIDsTable SELECT * FROM [dbo].ParseArray(@SupplierIDs,',')*/
	IF @CurrencyID IS NOT NULL 
		SELECT @LastExchangeRate = LastRate FROM Currency WHERE CurrencyID = @CurrencyID
	ELSE 
		SET @LastExchangeRate = 1
		
	--****************************************************************************
	-- If no Zone is selected, select our zones joined to supplier zones
	--****************************************************************************
	SELECT RB.CustomerID, RB.ZoneID, RB.SupplierID  INTO #Blocks 
		    FROM RouteBlock RB WITH(NOLOCK)
			 WHERE RB.IsEffective = 'Y'
			 AND (rb.CustomerID IS NULL OR rb.CustomerID = @CustomerID)
			AND RB.ZoneID IS NOT NULL
	IF @Mode IS NULL
	BEGIN	
		DECLARE @OurRates TABLE(ZoneID int PRIMARY KEY, Rate real, OffPeakRate real, WeekendRate real, ServicesFlag SMALLINT);
		IF @CustomerID IS NULL
			INSERT INTO @OurRates (ZoneID, Rate, OffPeakRate, WeekendRate, ServicesFlag)
				    SELECT  Z.ZoneID, 
						Avg(ZR.NormalRate) * @LastExchangeRate, 
						Avg(ZR.OffPeakRate) * @LastExchangeRate, 
						Avg(ZR.WeekendRate) * @LastExchangeRate,
						@ServicesFlag
					FROM Zone Z with(nolock) LEFT JOIN ZoneRate ZR  with(nolock)
										ON 
											ZR.ZoneID = Z.ZoneID
										AND ZR.NormalRate > 0
										AND ZR.NormalRate < @ExcludedRate
										AND (@ServicesFlag IS NULL OR ZR.ServicesFlag & @ServicesFlag = @ServicesFlag)	
					WHERE 1=1
						AND	Z.SupplierID = 'SYS'
						AND (@zoneNameFilter IS NULL OR Z.Name LIKE @zoneNameFilter)
						AND (@codeFilter IS NULL OR ZR.ZoneID IN (SELECT C.ZoneID FROM Code C WHERE C.Code LIKE @codeFilter))
					GROUP BY Z.ZoneID;
		ELSE
			BEGIN 
				INSERT INTO @OurRates (ZoneID, Rate, OffPeakRate, WeekendRate, ServicesFlag)
					SELECT  ZR.ZoneID, 
							ZR.NormalRate * @LastExchangeRate, 
							ZR.OffPeakRate * @LastExchangeRate, 
							ZR.WeekendRate * @LastExchangeRate, 
							ZR.ServicesFlag
						FROM ZoneRate ZR
							WHERE
								ZR.CustomerID = @CustomerID 
							AND ZR.NormalRate < @ExcludedRate 
							AND (@zoneNameFilter IS NULL OR ZR.ZoneID IN (SELECT ZoneID FROM Zone WHERE Name LIKE @zoneNameFilter)) 
							AND (@codeFilter IS NULL OR ZR.ZoneID IN (SELECT ZoneID FROM Code WHERE Code LIKE @codeFilter))
			END;
    WITH PrimaryResult AS (
		SELECT
			OZ.ZoneID as OurZoneID, OZ.Name AS OurZone,
			R.Rate as OurNormalRate, R.OffPeakRate as OurOffPeakRate, R.WeekendRate AS OurWeekendRate, R.ServicesFlag AS OurServicesFlag,
			SZ.SupplierID, SZ.ZoneID AS SupplierZoneID, SZ.Name AS SupplierZone,
			SR.NormalRate * @LastExchangeRate AS SupplierNormalRate, 
			SR.OffPeakRate * @LastExchangeRate AS SupplierOffPeakRate, 
			SR.WeekendRate * @LastExchangeRate AS SupplierWeekendRate, 
			SR.ServicesFlag AS SupplierServicesFlag
			--Ra.EndEffectiveDate AS EndEffectiveDate
		FROM
			Zone OZ with(nolock), @OurRates R, ZoneMatch ZM with(nolock), ZoneRate SR with(nolock), Zone SZ with(nolock)
		WHERE 
				OZ.ZoneID = R.ZoneID
			AND ZM.OurZoneID = OZ.ZoneID
			AND ZM.SupplierZoneID = SR.ZoneID
			AND SR.ServicesFlag & R.ServicesFlag = R.ServicesFlag
			AND (@ServicesFlag IS NULL OR SR.ServicesFlag & @ServicesFlag = @ServicesFlag)
			AND SR.ZoneID = SZ.ZoneID
			AND SR.SupplierID <> 'SYS'
			AND (@CustomerID IS  NULL OR SZ.SupplierID <> @CustomerID)
			AND (@AllRates = 'Y' OR (SR.NormalRate <= R.Rate))
			--AND SR.ZoneID NOT IN (SELECT RB.ZoneID FROM #Blocks RB)
		--ORDER BY OurZone, SupplierNormalRate,OZ.EndEffectiveDate ASC
    )
    SELECT 
    P.OurZoneID as OurZoneID, P.OurZone AS OurZone,
			P.OurNormalRate as OurNormalRate, P.OurOffPeakRate as OurOffPeakRate, P.OurWeekendRate AS OurWeekendRate, P.OurServicesFlag AS OurServicesFlag,
			P.SupplierID, P.SupplierZoneID AS SupplierZoneID, P.SupplierZone AS SupplierZone,
			P.SupplierNormalRate AS SupplierNormalRate, 
			P.SupplierOffPeakRate AS SupplierOffPeakRate, 
			P.SupplierWeekendRate AS SupplierWeekendRate, 
			P.SupplierServicesFlag AS SupplierServicesFlag,
    Ra.endeffectivedate
     FROM primaryresult P with(nolock) ,rate ra with(nolock),PriceList pl with(nolock)
    WHERE 
          Ra.ZoneID = P.SupplierZoneID 
			AND Ra.IsEffective = 'Y'
			AND Ra.PriceListID  = pl.PriceListID 
			AND pl.SupplierID = P.SupplierID
			AND (@SupplierIDs Is Null Or @SupplierIDs like '%,' + P.SupplierID + ',%' )
    ORDER BY P.OurZone, P.SupplierNormalRate ASC
	END

	ELSE
				
	--****************************************************************************
	-- If a particular Zone is selected, select only matching supplier zones
	--****************************************************************************
	BEGIN		
	 
	 DECLARE @FromDate datetime
     DECLARE @TillDate datetime

     SELECT @FromDate = CAST(CONVERT(varchar(10),DATEADD(dd,-@Days,GETDATE()),121) AS DATETIME)
     SELECT @TillDate = GETDATE()		
     
     SELECT 
            ZM.OurZoneID, 
			SZ.SupplierID, SZ.ZoneID AS SupplierZoneID, 
			SZ.Name AS SupplierZone, 
			ZR.NormalRate * @LastExchangeRate AS SupplierNormalRate,
			ZR.OffPeakRate * @LastExchangeRate  AS SupplierOffPeakRate,
			ZR.WeekendRate * @LastExchangeRate AS SupplierWeekendRate,
			ZR.ServicesFlag AS SupplierServicesFlag,
			null,
			TS.DurationsInMinutes,
			TS.ASR,
			TS.ACD
     FROM 
		Zone SZ WITH(NOLOCK) 
			INNER JOIN ZoneMatch ZM WITH(NOLOCK) 
							ON ZM.SupplierZoneID = SZ.ZoneID	
			INNER JOIN ZoneRate ZR WITH(NOLOCK) 
							ON ZR.ZoneID = ZM.SupplierZoneID AND (@CustomerID IS NOT NULL OR SZ.SupplierID <> @CustomerID )
			LEFT JOIN GetSupplierZoneStats(@FromDate, @TillDate,NULL) AS TS 
							ON TS.SupplierID = ZR.SupplierID AND TS.OurZoneID = ZM.OurZoneID
		WHERE	
		     ZR.SupplierID <> 'SYS'
			AND ZR.ServicesFlag & @ServicesFlag = @ServicesFlag				
		ORDER BY SupplierNormalRate ASC
		OPTION (RECOMPILE)
	END
END