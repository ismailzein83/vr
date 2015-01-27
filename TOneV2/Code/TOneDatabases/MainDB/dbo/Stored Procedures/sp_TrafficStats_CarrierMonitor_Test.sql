
CREATE PROCEDURE [dbo].[sp_TrafficStats_CarrierMonitor_Test]
	@FromDateTime DATETIME,
	@ToDateTime DATETIME,
	@CustomerID varchar(10) = NULL,
	@SupplierID varchar(10) = NULL,
	@OurZoneID int = NULL,
	@SwitchID tinyint = NULL,
	@GroupBySupplier Char(1) = 'N',
	@IncludeCarrierGroupSummary Char(1) = 'N'
WITH recompile
AS

BEGIN
	WITH
	RepresentedAsSwitchCarriers AS (
		SELECT grac.CID AS CarrierAccountID
		FROM dbo.GetRepresentedAsSwitchCarriers() grac
	)
	, Traffic_Stats_Data AS (
		SELECT
				ts.SwitchId AS SwitchID
			  , ts.CustomerID AS CustomerID
			  , ts.SupplierID AS SupplierID
			  , ts.OurZoneID AS OurZoneID
			  , ts.Attempts AS Attempts
			  , ts.DeliveredAttempts AS DelieveredAttempts
			  , ts.SuccessfulAttempts AS SuccessfulAttempts
			  , ts.FirstCDRAttempt AS FirstCDRAttempt
			  , ts.DurationsInSeconds AS DurationInSeconds
			  , ts.PDDInSeconds AS PDDInSeconds
			  , ts.DeliveredNumberOfCalls AS DelieveredNumberOfCalls
			--  , ts.MaxDurationInSeconds AS MaxDurationInSeconds
			  , ts.NumberOfCalls AS NumberOfCalls
		FROM TrafficStats AS ts WITH(NOLOCK) 
		WHERE
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND (@CustomerID IS NULL OR ts.CustomerID = @CustomerID)
				AND (@SupplierID IS NULL OR ts.SupplierID = @SupplierID)
		--		AND (SwitchID IS NULL OR ts.SwitchID = @SwitchID)
				AND (@OurZoneID IS NULL OR ts.OurZoneID = @OurZoneID)
	)
	
	, Traffic_Stats AS (
		SELECT
				CASE WHEN (@GroupBySupplier = 'Y' OR (@CustomerID IS NULL AND @SupplierID IS NOT NULL)) then ts.SupplierID ELSE ts.CustomerID END AS CarrierAccountID
			  , ts.SwitchID AS SwitchID
			 -- , ts.CustomerID AS CustomerID
			--  , ts.SupplierID AS SupplierID
			  , ts.OurZoneID AS OurZoneID
			  , CASE WHEN (@GroupBySupplier = 'Y' OR (@CustomerID IS NULL AND @SupplierID IS NOT NULL)) THEN SUM(ts.Attempts) ELSE SUM(NumberOfCalls) END AS Attempts
			  , Sum(ts.DelieveredAttempts) AS DelieveredAttempts
			  , Sum(ts.DelieveredNumberOfCalls) AS DelieveredNumberOfCalls
			  , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
			  , Sum(ts.DurationInSeconds) AS DurationInSeconds
			  , Sum(ts.PDDInSeconds) AS PDDInSeconds
			 -- , Sum(ts.NumberOfCalls) AS NumberOfCalls
		FROM Traffic_Stats_Data ts
		GROUP BY
				CASE WHEN (@GroupBySupplier = 'Y' OR (@CustomerID IS NULL AND @SupplierID IS NOT NULL)) then ts.SupplierID ELSE ts.CustomerID END
			  ,	ts.SwitchID
			--  ,	ts.CustomerID
			--  , ts.SupplierID
			  , ts.OurZoneID
	)
	--select * from Traffic_Stats
	, Traffic AS (
		SELECT
			   ts.SwitchID AS SwitchID
			 , ts.CarrierAccountID AS CarrierAccountID
		--	 , ts.CustomerID AS CarrierAccountID
		--	 , ts.SupplierID AS SupplierID
			 , ts.OurZoneID AS OurZoneID
			 , ts.Attempts AS Attempts
			 , ts.DelieveredAttempts AS DelieveredAttempts
			 , ts.DelieveredNumberOfCalls AS DelieveredNumberOfCalls
			 , ts.SuccessfulAttempts AS SuccessfulAttempts
			 , ts.DurationInSeconds AS DurationInSeconds
			 , ts.PDDInSeconds AS PDDInSeconds
			-- , ts.NumberOfCalls AS NumberOfCalls
		FROM Traffic_Stats ts
		WHERE 
			(@SwitchID IS NULL AND NOT EXISTS (SELECT * FROM RepresentedAsSwitchCarriers grac WHERE grac.CarrierAccountID = ts.CarrierAccountID)
				--			   AND NOT EXISTS (SELECT * FROM RepresentedAsSwitchCarriers grac WHERE grac.CarrierAccountID = ts.SupplierID)) 
							   OR ts.SwitchID = @SwitchID)
			
	)
	, FinalTrafficStats AS (
		SELECT
				ts.CarrierAccountID AS CarrierAccountID
			  , Sum(ts.Attempts) AS Attempts
			  , Sum(ts.DurationInSeconds/60.) AS DurationInMinutes
			  , CASE WHEN(Sum(ts.Attempts) > 0 ) Then Sum(ts.SuccessfulAttempts)*100.0/Sum(Attempts) ELSE 0 END AS ASR
			  , CASE WHEN Sum(SuccessfulAttempts) > 0 then Sum(ts.DurationInSeconds)/(60*Sum(SuccessfulAttempts)) ELSE 0 END AS ACD
		FROM 
				Traffic ts
		GROUP BY ts.CarrierAccountID
		--ORDER BY SUM(ts.Attempts) DESC
	)
	
	SELECT * FROM FinalTrafficStats
			ORDER BY Attempts DESC
END