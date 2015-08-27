
CREATE PROCEDURE [Analytics].[sp_DailyReportCall_CreateTempForFiltered]
	@TempTableName VARCHAR(200),
	@ZoneIDs INT = NULL,
	@SupplierIDs VARCHAR(5) = NULL,
	@CustomerIDs VARCHAR(5) = NULL,
	@TargetDate DATETIME
WITH RECOMPILE
AS
BEGIN

IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
BEGIN

	WITH Traffic AS
	(
		SELECT
			OurZoneID AS ZoneID,
			SupplierID AS SupplierID,
			CustomerID AS CustomerID,
			SUM(Attempts) AS Attempts, 
			SUM(SuccessfulAttempts) * 100.0 / SUM(Attempts) AS ASR, 
			CASE WHEN SUM(SuccessfulAttempts) > 0 THEN SUM(DurationsInSeconds) / (60.0 * SUM(SuccessfulAttempts)) ELSE 0 END AS ACD, 
			AVG(PDDinSeconds) AS PDD,
			SUM(DurationsInSeconds/60.) AS DurationInMinutes,
			dbo.dateof(FirstCDRAttempt) AS CallDate
		
		FROM TrafficStats WITH(NOLOCK)
		 
		WHERE
			--OurZoneID IS NOT NULL And CustomerID IS NOT NULL And SupplierID IS NOT NULL
			dbo.DateOf(FirstCDRAttempt) = @TargetDate
			AND CustomerID NOT IN (SELECT grASc.CID FROM dbo.GetRepresentedASSwitchCarriers() grASc)
			--AND OurZoneID IN (SELECT ZoneID FROM @ZoneIDs)
			--AND CustomerID IN (SELECT CarrierAccountID FROM @CustomerIDs)
			--AND SupplierID IN (SELECT CarrierAccountID FROM @SupplierIDs)

		GROUP BY
			dbo.dateof(FirstCDRAttempt),
			SupplierID,
			CustomerID,
			OurZoneID
	),

	Billing AS
	(
		SELECT
			SaleZoneID AS ZoneID,
			SupplierID AS SupplierID,
			CustomerID AS CustomerID,
			AVG(Cost_Rate) AS CostRate,
			AVG(Sale_Rate) AS SaleRate,
			dbo.DateOf(CallDate) AS CallDate
			
		FROM Billing_Stats WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
			
		WHERE
			--SaleZoneID IS NOT NULL And CustomerID IS NOT NULL And SupplierID IS NOT NULL
			dbo.DateOf(CallDate) = @TargetDate
			--AND OurZoneID IN (SELECT ZoneID FROM @ZoneIDs)
			--AND CustomerID IN (SELECT CarrierAccountID FROM @CustomerIDs)
			--AND SupplierID IN (SELECT CarrierAccountID FROM @SupplierIDs)
			
		GROUP BY
			dbo.dateof(CallDate),
			SaleZoneID,
			SupplierID,
			CustomerID
	)

	SELECT
		T.ZoneID,
		T.CustomerID,
		T.SupplierID,
		T.Attempts,
		T.ASR,
		T.ACD,
		T.PDD,
		T.DurationInMinutes,
		B.CostRate,
		B.SaleRate
	
	INTO #RESULT
	
	FROM Traffic T LEFT JOIN Billing B
		ON (T.ZoneID IS NULL OR B.ZoneID = T.ZoneID) 
		AND (T.SupplierID IS NULL OR B.SupplierID = T.SupplierID) 
		AND (T.CustomerID IS NULL OR B.CustomerID =  T.CustomerID)
		AND B.CallDate = T.CallDate
		
	ORDER BY T.CallDate, T.Attempts DESC
	
	DECLARE @sql VARCHAR(1000)
	SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
	EXEC(@sql)
END

END