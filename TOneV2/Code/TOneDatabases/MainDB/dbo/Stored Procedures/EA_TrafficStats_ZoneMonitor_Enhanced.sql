
CREATE PROCEDURE [dbo].[EA_TrafficStats_ZoneMonitor_Enhanced]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10) = NULL,
    @SwitchID	  tinyInt = NULL,   
    @CodeGroup varchar(10) = NULL
WITH RECOMPILE
AS	
BEGIN
	WITH
	RepresentedAsSwitchCarrier AS (
		SELECT grasc.CID AS CarrierAccountID
		FROM dbo.GetRepresentedAsSwitchCarriers() grasc
	),
	OurZOnes AS 
	(
		SELECT z.ZoneID AS ZoneID 
		     , z.Name AS ZoneName 
		     , z.CodeGroup AS CodeGroup 
		FROM Zone z WITH(NOLOCK) 
		WHERE z.SupplierID='SYS' 
		AND (z.BeginEffectiveDate <= @FromDateTime OR z.BeginEffectiveDate BETWEEN @FromDateTime AND @ToDateTime) 
	),
	Traffic_Stats_Data AS (
		SELECT  ts.SwitchId AS SwitchID,
				ts.CustomerID AS CustomerID,
				ts.SupplierID AS SupplierID,
				ts.OurZoneID AS OurZoneID,
				ts.Attempts AS Attempts,
				ts.FirstCDRAttempt AS FirstCDRAttempt,
				ts.DurationsInSeconds AS DurationInSeconds,
				ts.SuccessfulAttempts AS SuccessfulAttempts,
				ts.DeliveredAttempts AS DelieveredAttempts,
				ts.PDDinSeconds AS PDDinSeconds,
				ts.MaxDurationInSeconds AS MaxDurationInSeconds,
				ts.LastCDRAttempt AS LastCDRAttempt
		FROM TrafficStats ts WITH (NOLOCK)
			LEFT JOIN OurZOnes AS oz WITH (NOLOCK) ON ts.OurZoneID = oz.ZoneID
		WHERE   FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND (@CustomerID IS NULL OR (ts.CustomerID = @CustomerID))
				AND (@SupplierID IS NULL OR (ts.SupplierID = @SupplierID))
				AND (@CustomerID IS NULL OR @SupplierID IS NULL)
			
	),
	Traffic_Stats AS (
		SELECT  
				CASE WHEN (@SupplierID IS NULL) THEN dbo.DateOf(ts.FirstCDRAttempt) ELSE NULL END AS AttemptDateTime,
		--		ts.FirstCDRAttempt AS AttemptDateTime,
				MIN(ts.FirstCDRAttempt) AS MinAttemptDateTime,
				ts.SwitchID AS SwitchID,
				ts.OurZoneID AS OurZoneID,
				SUM(ts.Attempts) AS Attempts,
				SUM(ts.DurationInSeconds/60.) AS DurationInMinutes,
				SUM(ts.SuccessfulAttempts)*100.0 / SUM(ts.Attempts) AS ASR,
				CASE WHEN SUM(ts.SuccessfulAttempts) > 0 THEN SUM(ts.DurationInSeconds) / (60.0*SUM(ts.SuccessfulAttempts)) ELSE 0 END AS ACD,
				SUM(ts.DelieveredAttempts) AS DelieveredAttempts,
				SUM(ts.DelieveredAttempts) * 100.0 / SUM(ts.Attempts) AS DelieveredASR,
				AVG(ts.PDDinSeconds) AS AveragePDD,
				MAX(ts.MaxDurationInSeconds)/60 AS MaxDuration,
				MAX(ts.LastCDRAttempt) AS LastAttempt,
				0 AS AttemptPercentage,
				0 AS DurationPercentage,
				SUM(ts.SuccessfulAttempts) AS SuccessfulAttempts,
				SUM(ts.Attempts - ts.SuccessfulAttempts) AS FailedAttempts
		FROM Traffic_Stats_Data ts WITH (NOLOCK)	
		WHERE
			(@SwitchID IS NULL AND NOT EXISTS (SELECT * FROM RepresentedAsSwitchCarrier grac where grac.CarrierAccountID = ts.CustomerID)) OR ts.SwitchID = @SwitchID
		GROUP BY 
				CASE WHEN (@SupplierID IS NULL) THEN dbo.DateOf(ts.FirstCDRAttempt) ELSE NULL END,
				ts.OurZoneID,
				TS.SwitchID
	--),
	--TotalAttempts AS (
	--	SELECT SUM(Attempts) AS Total
	--	FROM Traffic_Stats WITH (NOLOCK)
	),
	Result AS (
		SELECT
			CASE WHEN (@SupplierID IS NULL) THEN ts.AttemptDateTime ELSE ts.MinAttemptDateTime END AS AttemptDateTime,
		--	ts.AttemptDateTime AS AttemptDateTime,
			ts.OurZoneID AS OurZoneID,
			ts.Attempts AS Attempts,
			ts.FailedAttempts AS FailedAttempts,
			ts.DurationInMinutes AS DurationInMinutes,
			ts.ASR AS ASR,
			ts.ACD AS ACD,
			ts.DelieveredAttempts AS DelieveredAttempts,
			ts.DelieveredASR AS DelieveredASR,
			ts.AveragePDD AS AveragePDD,
			ts.MaxDuration AS MaxDuration,
			ts.LastAttempt AS LastAttempt,
		--	(ts.Attempts * 100.0 / ta.Total) AS AttemptPercentage,
			ts.DurationPercentage AS DurationPercentage,
			ts.SuccessfulAttempts AS SuccessfulAttempts
		FROM Traffic_Stats ts WITH (NOLOCK)
	)
	SELECT * FROM Result
	--ORDER BY Attempts DESC,
	--     	DurationInMinutes DESC
			   

END