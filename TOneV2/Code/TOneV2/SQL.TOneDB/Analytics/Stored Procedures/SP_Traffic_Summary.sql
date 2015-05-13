--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE   PROCEDURE [Analytics].[SP_Traffic_Summary]
    @FromDate	datetime,
	@ToDate		datetime
AS
BEGIN	
	SET NOCOUNT ON
	
	SET @todate = dateadd(dd,1,@todate)
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	DECLARE @RepresentedAsSwitchCarriers TABLE (
CID VARCHAR(5)
)
INSERT INTO  @RepresentedAsSwitchCarriers SELECT ca.CarrierAccountID FROM CarrierAccount ca WITH (NOLOCK)
                 WHERE ca.RepresentsASwitch='Y'
	
	;With BILLING AS(
	
	SELECT
		DATEADD(day,0,datediff(day,0, BS.CallDate)) AS [Day],
		ISNULL(SUM(BS.NumberOfCalls),0) AS Attempts,
		ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
		ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
		ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit
	FROM
		Billing_Stats BS  WITH(NOLOCK,Index(IX_Billing_Stats_Date)) 
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
        LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
    WHERE  (BS.CallDate >= @fromDate AND BS.CallDate < @ToDate)
    GROUP BY DATEADD(day,0,datediff(day,0, BS.CallDate))
	),
	TRAFFIC AS(
	SELECT
        DATEADD(day,0,datediff(day,0, LastCDRAttempt)) AS [Day],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
        INNER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDate AND @ToDate
   
	GROUP BY  DATEADD(day,0,datediff(day,0, LastCDRAttempt))
	)
	
	
	SELECT  SUM(BS.Sale) AS Sales
			, SUM(BS.Cost) AS Purchases
			, SUM(BS.Profit) AS Profit
			, SUM(TS.Attempts) AS Attempts
			, SUM(TS.DurationsInMinutes) AS DurationsInMinutes
			, AVG(BS.Sale) AS AverageSales
			, AVG(BS.Cost) AS AveragePurchases
			, AVG(BS.Profit) AS AverageProfit
			
	FROM Traffic TS
	LEFT JOIN Billing BS ON BS.Day = TS.Day
	--GROUP BY BS.DAY


END