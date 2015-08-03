CREATE PROCEDURE dbo.bp_GetStatisticalDailySummary(@date DATETIME = NULL)
WITH RECOMPILE
AS
BEGIN
	DECLARE @dateStart AS DATETIME
	DECLARE @dateFinish AS DATETIME
	SET @date = ISNULL(@date, DATEADD(DAY, -1, dbo.DateOf(GETDATE())))
	SET @dateStart = @date
	SET @dateFinish = DATEADD(ms, -3, DATEADD(DAY, +1, @dateStart))

	SELECT 
		'CDR' AS [Source],
		COUNT(*) as Attempts,
		SUM(CASE when c.DurationInSeconds > 0 THEN 1 ELSE 0 END) AS Calls,    
		SUM(c.DurationInSeconds) / 60.0 AS Minutes
	FROM CDR c WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)) WHERE c.AttemptDateTime BETWEEN @dateStart AND @dateFinish
	UNION
	SELECT 
		'Traffic_Stats',
		SUM(ts.Attempts) as Attempts, 
		SUM(ts.SuccessfulAttempts) AS Calls,
		SUM(ts.DurationsInSeconds) / 60.0 AS Minutes 
	FROM TrafficStats ts WITH(NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst)) WHERE ts.FirstCDRAttempt BETWEEN @dateStart AND @dateFinish
	UNION
	SELECT 
		'Billing_Stats_Cost',
		NULL as Attempts, 
		SUM(CASE WHEN bs.Cost_Currency IS NULL then 0 ELSE bs.NumberOfCalls END) AS Calls,
		SUM(bs.CostDuration) / 60.0 AS Minutes 
	FROM Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date)) WHERE bs.CallDate = @date
	UNION
	SELECT 
		'Billing_Stats_Sale',
		NULL as Attempts, 
		SUM(CASE WHEN bs.Sale_Currency IS NULL then 0 ELSE bs.NumberOfCalls END) AS Calls,
		SUM(bs.SaleDuration) / 60.0 AS Minutes 
	FROM Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date)) WHERE bs.CallDate = @date	
END