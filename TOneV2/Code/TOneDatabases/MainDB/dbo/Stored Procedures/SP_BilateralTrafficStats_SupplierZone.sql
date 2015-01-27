
CREATE PROCEDURE [dbo].[SP_BilateralTrafficStats_SupplierZone]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = NULL,
   @SupplierID Varchar(5) 
AS
BEGIN
	SET @FromDate=     CAST(
(
	STR( YEAR( @FromDate ) ) + '-' +
	STR( MONTH( @FromDate ) ) + '-' +
	STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate= CAST(
(
	STR( YEAR( @ToDate ) ) + '-' +
	STR( MONTH(@ToDate ) ) + '-' +
	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	

Declare @Traffic TABLE (ZoneID int  primary KEY, Attempts int, DurationsInSeconds numeric(13,5), SuccessfulAttempts numeric (13,5), NumberOfCalls int,DeliveredAttempts int)
SET NOCOUNT ON 	
INSERT INTO @Traffic(ZoneID, Attempts, SuccessfulAttempts, DurationsInSeconds,NumberOfCalls,DeliveredAttempts)
SELECT 
	   ISNULL(TS.SupplierZoneID, ''),
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds) as DurationsInSeconds, 
	   Sum(NumberOfCalls) as NumberOfCalls,
	   SUM(TS.DeliveredAttempts) as DeliveredAttempts
    FROM TrafficStats TS WITH(NOLOCK)
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
			--AND (TS.CustomerID = @CustomerID)
			AND TS.SupplierID = @SupplierID
	GROUP BY TS.SupplierZoneID
	
DECLARE @Result TABLE (
		ZoneID int primary KEY,Attempts int, DurationsInSeconds numeric(13,5), SuccessfulAttempts numeric (13,5),NumberOfCalls int,DeliveredAttempts int
		)

INSERT INTO @Result(ZoneID ,Attempts, DurationsInSeconds,SuccessfulAttempts, NumberOfCalls,DeliveredAttempts)
	SELECT 
		  TS.ZoneID , TS.Attempts, TS.DurationsInSeconds,TS.SuccessfulAttempts, Sum(BS.NumberOfCalls) AS NumberOfCalls ,TS.DeliveredAttempts as DeliveredAttempts    
    FROM @Traffic TS 
		LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.ZoneID= BS.CostZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	Where (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) 
	--AND (BS.CustomerID=@CustomerID) 
	AND BS.SupplierID = @SupplierID
	GROUP BY TS.ZoneID, TS.Attempts, TS.DurationsInSeconds,TS.SuccessfulAttempts,TS.NumberOfCalls,TS.DeliveredAttempts
	 
--Declare @TotalProfit numeric(13,5)
--SELECT  @TotalProfit = SUM(profit) FROM @Result
--UPDATE  @Result SET Percentage = CASE WHEN @TotalProfit>0 THEN (Profit * 100. / @TotalProfit) ELSE 0.0 END 
--Declare @TotalAttempts bigint
--SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
--Update  @Result SET AttemptPercentage =CASE WHEN @TotalAttempts>0 THEN  (Attempts * 100. / @TotalAttempts) ELSE 0.0 END 

SELECT * FROM @Result ORDER BY DurationsInSeconds DESC 

END