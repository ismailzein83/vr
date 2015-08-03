
CREATE PROCEDURE [dbo].[SP_BilateralTrafficStats_CustomerZone]
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = null
AS
BEGIN

SELECT @FromDate = DATEADD(day,0,datediff(day,0,@FromDate))
SELECT @ToDate =  DATEADD(SECOND,-1,DATEADD(day,1,datediff(day,0,@ToDate)))


	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	
Declare @Traffic TABLE (ZoneID int primary KEY, Attempts int, SuccessfulAttempts int, DurationsInSeconds numeric(13,5), NumberOfCalls int,DeliveredAttempts int)
SET NOCOUNT ON 	

	

IF @CustomerID IS NULL
	INSERT INTO @Traffic(ZoneID,NumberOfCalls, Attempts, SuccessfulAttempts, DurationsInSeconds,DeliveredAttempts)
	SELECT
		   ISNULL(TS.OurZoneID, ''), 
		   Sum(NumberofCalls) as NumberOfCalls, 
		   SUM(Attempts) AS Attempts,
		   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		   Sum(DurationsInSeconds) as DurationsInSeconds,
		   SUM(TS.DeliveredAttempts) as DeliveredAttempts
		FROM TrafficStats TS WITH(NOLOCK)
		
		WHERE FirstCDRAttempt BETWEEN @FromDate AND @ToDate 		 
		AND (TS.CustomerID is not null AND TS.CustomerID NOT IN ( SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc))
		GROUP BY TS.OurZoneID--, TS.Attempts, TS.SuccessfulAttempts,TS.NumberOfCalls
ELSE
	INSERT INTO @Traffic(ZoneID,NumberOfCalls, Attempts, SuccessfulAttempts, DurationsInSeconds,DeliveredAttempts)
	SELECT
		   ISNULL(TS.OurZoneID, ''), 
		   Sum(NumberofCalls) as NumberOfCalls, 
		   SUM(Attempts) AS Attempts, 
		   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		   Sum(DurationsInSeconds) as DurationsInSeconds,
		   SUM(TS.DeliveredAttempts) as DeliveredAttempts
		FROM TrafficStats TS WITH(NOLOCK)
		
		WHERE FirstCDRAttempt BETWEEN @FromDate AND @ToDate AND TS.CustomerID = @CustomerID
		--AND ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		--AND (TS.CustomerID is not null AND TS.CustomerID NOT IN ( SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc))	 		 		
		GROUP BY TS.OurZoneID--, TS.Attempts, TS.SuccessfulAttempts,TS.NumberOfCalls


DECLARE @Result TABLE (
		ZoneID INT PRIMARY KEY,Attempts int, SuccessfulAttempts int, DurationsInSeconds numeric(13,5),
		NumberOfCalls int,DeliveredAttempts int
		)

IF @CustomerID IS NULL
	INSERT INTO @Result(ZoneID ,Attempts, SuccessfulAttempts, DurationsInSeconds, NumberOfCalls,DeliveredAttempts )
		SELECT 
			  TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInSeconds, Sum(BS.NumberOfCalls) AS NumberOfCalls ,TS.DeliveredAttempts as DeliveredAttempts     
		FROM @Traffic TS 
			LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) ON TS.ZoneID= BS.SaleZoneID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate)  
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		GROUP BY TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts,TS.NumberOfCalls, TS.DurationsInSeconds,TS.DeliveredAttempts
else
	INSERT INTO @Result(ZoneID ,Attempts, SuccessfulAttempts, DurationsInSeconds, NumberOfCalls,DeliveredAttempts)
		SELECT 
			  TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInSeconds,Sum(BS.NumberOfCalls) AS NumberOfCalls   ,TS.DeliveredAttempts as DeliveredAttempts    
		FROM @Traffic TS 
			LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.ZoneID= BS.SaleZoneID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.CustomerID=@CustomerID) 
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		GROUP BY TS.ZoneID, TS.Attempts,TS.DurationsInSeconds, TS.SuccessfulAttempts,TS.NumberOfCalls,TS.DeliveredAttempts 

--Declare @TotalProfit numeric(13,5)
--SELECT  @TotalProfit = SUM(profit) FROM @Result


--UPDATE  @Result SET Percentage =CASE WHEN @TotalProfit<>0 THEN  (Profit * 100. / @TotalProfit) ELSE 0 END 
--Declare @TotalAttempts bigint
--SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
--Update  @Result SET AttemptPercentage =case WHEN @TotalAttempts<>0 THEN  (Attempts * 100. / @TotalAttempts) ELSE 0 END  

SELECT * FROM @Result ORDER BY DurationsInSeconds DESC 

END