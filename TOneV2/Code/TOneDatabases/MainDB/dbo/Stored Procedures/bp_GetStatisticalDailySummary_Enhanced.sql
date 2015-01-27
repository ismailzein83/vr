CREATE PROCEDURE [dbo].[bp_GetStatisticalDailySummary_Enhanced](@date DATETIME = NULL)
WITH RECOMPILE
AS
BEGIN
	
	DECLARE @dateStart AS DATETIME
	DECLARE @dateFinish AS DATETIME
	SET @date = ISNULL(@date, DATEADD(DAY, -1, dbo.DateOf(GETDATE())))
	SET @dateStart = @date
	SET @dateFinish = DATEADD(ms, -3, DATEADD(DAY, +1, @dateStart))
	;
	WITH
	CDR_Stats AS (
		SELECT
			'CDR' AS [Source]
		  , COUNT(*) AS Attempts
		  , SUM(CASE WHEN c.DurationInSeconds > 0 THEN 1 ELSE 0 END) AS Calls
		  , SUM(c.DurationInSeconds) / 60.0 AS Minutes
		FROM CDR c WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime))
		WHERE c.AttemptDateTime BETWEEN @dateStart AND @dateFinish
	)
	, Traffic_Stats AS (
		SELECT
			'Traffic_Stats' AS [Source] 
		  , SUM(ts.Attempts) AS Attempts
		  , SUM(ts.SuccessfulAttempts) AS Calls
		  , SUM(ts.DurationsInSeconds) / 60.0 AS Minutes
		FROM TrafficStats ts WITH(NOLOCK)
		WHERE ts.FirstCDRAttempt BETWEEN @dateStart AND @dateFinish
	)
	, Billing_Stats_Cost AS (
		SELECT 
			'Billing_Stats_Cost' AS [Source]
		  , NULL AS Attempts
		  , SUM(CASE WHEN bs.Cost_Currency IS NULL then 0 ELSE bs.NumberOfCalls END) AS Calls
		  , SUM(bs.CostDuration) / 60.0 AS Minutes
		FROM Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
		WHERE bs.CallDate = @date
	)
	, Billing_Stats_Sale AS (
		SELECT 
			'Billing_Stats_Sale' AS [Source]
		  , NULL AS Attempts
		  , SUM(CASE WHEN bs.Sale_Currency IS NULL then 0 ELSE bs.NumberOfCalls END) AS Calls
		  , SUM(bs.SaleDuration) / 60.0 AS Minutes 
	    FROM Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date)) 
	    WHERE bs.CallDate = @date	
	)
	
	SELECT * FROM CDR_Stats 
	UNION 
	SELECT * from Traffic_Stats
	UNION
	SELECT * FROM Billing_Stats_Cost
	UNION 
	SELECT * FROM Billing_Stats_Sale
END