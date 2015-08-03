
CREATE PROCEDURE [dbo].[GISAD_DashBoard]
	@FromDate datetime ,
	@ToDate datetime 
AS
SET @FromDate = CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)
BEGIN
SET NOCOUNT ON
DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
)
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	SELECT
		bs.CallDate AS Date,
		SUM(bs.NumberOfCalls) AS NumberOfCalls,
		SUM(bs.SaleDuration)/60.0 AS SaleDuration,
		SUM(bs.CostDuration)/60.0 AS CostDuration,
		SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS CostNet,
		SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet
	INTO #billing 			
	FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  bs.CallDate >= @FromDate
	   AND bs.CallDate < @ToDate
	GROUP BY bs.CallDate
				
	SELECT 
		dbo.dateof(ts.FirstCDRAttempt) AS Date,
		Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		Sum(NumberOfCalls) AS NumberOfCalls,
		Sum(deliveredAttempts) AS deliveredAttempts,
		Sum(DurationsInSeconds) AS DurationsInSeconds,
		Sum(Attempts) AS Attempts,
		Avg(PDDinSeconds) as AveragePDD, 
		Max (MaxDurationInSeconds)/60.0 as MaxDuration,
		Max(LastCDRAttempt) as LastAttempt
	INTO #traffic
	FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst)) 
	WHERE  ts.FirstCDRAttempt BETWEEN  @FromDate AND @ToDate  
	GROUP BY dbo.dateof(ts.FirstCDRAttempt)
	
	SELECT 
		Sum(b.NumberOfCalls) AS NumberOfCalls,
		Sum(b.SaleDuration) AS SaleDuration,
		Sum(b.CostDuration) AS CostDuration,
		Sum(b.CostNet) AS CostNet,
		Sum(b.SaleNet) AS SaleNet,
		Sum(b.SaleNet) - Sum(b.CostNet) AS Profit,
		Sum(t.DurationsInMinutes) AS DurationsInMinutes,
		Sum(t.SuccessfulAttempts)*100.0 / Sum(t.NumberOfCalls) as ASR,
		case when Sum(t.SuccessfulAttempts) > 0 then Sum(t.DurationsInSeconds)/(60.0*Sum(t.SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(t.deliveredAttempts) * 100.0 / Sum(t.Attempts) as DeliveredASR, 
		Avg(t.AveragePDD) AS AveragePDD,
		Max(t.MaxDuration) AS MaxDuration,
		Max(t.LastAttempt) AS LastAttempt,
		Sum(t.SuccessfulAttempts) AS SuccessfulAttempts
	FROM #traffic t
	LEFT JOIN #billing b ON t.Date = b.Date

END