
CREATE PROCEDURE [dbo].[SP_BilateralTrafficStatsDaily_SupplierZone]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) 
AS
BEGIN

--SET @fromDate = DATEADD(day,0,datediff(day,0, @fromDate))
--SET @ToDate = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDate)))

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@fromDate, @ToDate)
	

Declare @Traffic TABLE (ZoneID int, Attempts int, DurationsInSeconds numeric(13,5), SuccessfulAttempts numeric (13,5), NumberOfCalls int,CallDate datetime, DeliveredAttempts int)--  primary KEY
SET NOCOUNT ON 	
INSERT INTO @Traffic(ZoneID, Attempts, SuccessfulAttempts, DurationsInSeconds,NumberOfCalls,CallDate, DeliveredAttempts)
SELECT 
	   ISNULL(TS.SupplierZoneID, ''),
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds) as DurationsInSeconds, 
	   Sum(NumberOfCalls) as NumberOfCalls,
	   CONVERT(varchar(10), TS.FirstCDRAttempt,121) as CallDate,
	   sum(DeliveredAttempts) as dDeliveredAttempts
    FROM TrafficStats TS WITH(NOLOCK)
    WHERE (FirstCDRAttempt >= @fromDate AND FirstCDRAttempt <= @ToDate)
			--AND (TS.CustomerID = @CustomerID)
			AND TS.SupplierID = @SupplierID
	GROUP BY TS.SupplierZoneID,CONVERT(varchar(10), TS.FirstCDRAttempt,121)
	
DECLARE @Result TABLE (
		ZoneID int ,Attempts int, DurationsInSeconds numeric(13,5), SuccessfulAttempts numeric (13,5),NumberOfCalls int,CallDate datetime, DeliveredAttempts int--primary KEY
		)

INSERT INTO @Result(ZoneID ,Attempts, DurationsInSeconds,SuccessfulAttempts, NumberOfCalls,CallDate, DeliveredAttempts)
	SELECT 
		  TS.ZoneID , TS.Attempts, TS.DurationsInSeconds,TS.SuccessfulAttempts, Sum(BS.NumberOfCalls) AS NumberOfCalls  , TS.CallDate, ts.DeliveredAttempts
    FROM @Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.ZoneID= BS.CostZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	Where (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) 
	--AND (BS.CustomerID=@CustomerID) 
	AND BS.SupplierID = @SupplierID
	GROUP BY TS.ZoneID, TS.Attempts, TS.DurationsInSeconds,TS.SuccessfulAttempts,TS.NumberOfCalls,TS.CallDate,ts.DeliveredAttempts
	 
SELECT * FROM @Result ORDER BY DurationsInSeconds DESC 

END