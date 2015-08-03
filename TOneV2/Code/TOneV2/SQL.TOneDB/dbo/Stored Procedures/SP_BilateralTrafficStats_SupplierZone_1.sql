
CREATE PROCEDURE [dbo].[SP_BilateralTrafficStats_SupplierZone]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) 
AS
BEGIN
SELECT @FromDate = DATEADD(day,0,datediff(day,0, @FromDate))
SELECT @ToDate = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDate)))

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
;WITH Traffic As
(
SELECT 
	   ISNULL(TS.SupplierZoneID, '') AS ZoneID,
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds) as DurationsInSeconds, 
	   Sum(NumberOfCalls) as NumberOfCalls,
	   SUM(TS.DeliveredAttempts) as DeliveredAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX = IX_TrafficStats_DateTimeFirst)
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
			AND TS.SupplierID = @SupplierID
	GROUP BY TS.SupplierZoneID
),
CTE_Result AS
(
SELECT		  TS.ZoneID , TS.Attempts, TS.DurationsInSeconds,TS.SuccessfulAttempts, Sum(BS.NumberOfCalls) AS NumberOfCalls ,TS.DeliveredAttempts as DeliveredAttempts    
    FROM Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.ZoneID= BS.CostZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	Where (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate)
	AND BS.SupplierID = @SupplierID
	GROUP BY TS.ZoneID, TS.Attempts, TS.DurationsInSeconds,TS.SuccessfulAttempts,TS.NumberOfCalls,TS.DeliveredAttempts
	 )
SELECT * FROM CTE_Result 
ORDER BY DurationsInSeconds DESC 

END