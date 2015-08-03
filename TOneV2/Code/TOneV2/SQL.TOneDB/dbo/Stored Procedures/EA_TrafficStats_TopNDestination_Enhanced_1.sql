

CREATE PROCEDURE [dbo].[EA_TrafficStats_TopNDestination_Enhanced]
 @TopRecords   INT,
 @FromDate     DateTime,
 @ToDate       DateTime,
 @CustomerID varchar (25)= NULL,
 @SupplierID varchar (25)= NULL,
 @HighestTraffic CHAR(1) = 'Y'	
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
	
INSERT INTO @ExchangeRates
SELECT *
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate);
declare @RepresentedAsSwitchCarriers TABLE ( CarrierAccountID varchar(10))

INSERT INTO @RepresentedAsSwitchCarriers
	SELECT grasc.CID as CarrierAccountID
	FROM dbo.GetRepresentedAsSwitchCarriers() grasc

Declare @Zones TABLE ( OurZoneID INT , Attempts int, DurationsInSeconds numeric(13,5))

SET ROWCOUNT @TopRecords 

INSERT INTO @Zones
	SELECT 
		   TS.OurZoneID AS OurZoneID,
           SUM(Attempts) AS Attempts,
           SUM(TS.DurationsInSeconds) AS DurationsInSeconds
    FROM   TrafficStats ts WITH( NOLOCK)
    WHERE  FirstCDRAttempt BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID)
           AND ( TS.CustomerID IS NOT NULL  AND NOT EXISTS (SELECT * FROM @RepresentedAsSwitchCarriers WHERE CarrierAccountID = ts.CustomerID))
           AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID)
           AND ts.OurZoneID IS NOT NULL 
    GROUP BY 
           TS.OurZoneID
    ORDER BY 
			CASE WHEN (@HighestTraffic = 'Y') THEN SUM(TS.DurationsInSeconds) END DESC,
			CASE WHEN (@HighestTraffic = 'N') THEN SUM(TS.DurationsInSeconds) END ASC,
			Attempts DESC
			
           
SET ROWCOUNT 0;
;
WITH 
 TrafficTable AS 
(
    SELECT dbo.DateOf(TS.FirstCDRAttempt) AS Date,
           TS.OurZoneID,
           SUM(TS.Attempts) AS Attempts,
           SUM(TS.DurationsInSeconds / 60.) AS DurationsInMinutes,
           SUM(TS.SuccessfulAttempts) * 100.0 /SUM(TS.Attempts) AS ASR,
           CASE 
                WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(TS.DurationsInSeconds) 
                     / (60.0 * SUM(TS.SuccessfulAttempts))
                ELSE 0
           END AS ACD,
           SUM(TS.deliveredAttempts) AS deliveredAttempts,
           SUM(Ts.deliveredAttempts) * 100.0 / SUM(Ts.Attempts) AS DeliveredASR,
           AVG(Ts.PDDinSeconds) AS AveragePDD,
           MAX(Ts.MaxDurationInSeconds) / 60.0 AS MaxDuration,
           MAX(Ts.LastCDRAttempt) AS LastAttempt,
           SUM(Ts.SuccessfulAttempts) AS SuccessfulAttempts,
           SUM(Ts.Attempts - Ts.SuccessfulAttempts) AS FailedAttempts
    FROM   TrafficStats ts WITH(NOLOCK)
		    JOIN @Zones z ON  TS.OurZoneID = Z.OurZoneID
    WHERE  Ts.FirstCDRAttempt BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID)
           AND ( TS.CustomerID IS NOT NULL  AND NOT EXISTS (SELECT * FROM @RepresentedAsSwitchCarriers WHERE CarrierAccountID = TS.CustomerID))
           AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID)
		   AND TS.OurZoneID IS NOT NULL
    GROUP BY
           dbo.DateOf(TS.FirstCDRAttempt),
           TS.OurZoneID
)
,BillingTable AS 
(
    SELECT bs.CallDate AS Date,
           bs.SaleZoneID AS OurZoneID,
           AVG(bs.Sale_Rate / ISNULL(ERS.Rate,1)) AS Rate,
           SUM(bs.Sale_Nets / ISNULL(ERS.Rate,1)) AS Amount
    FROM   Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
           LEFT JOIN @ExchangeRates ERS
                ON  ERS.Currency = bs.Sale_Currency
                AND ERS.Date = bs.CallDate
    WHERE  bs.CallDate BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR bs.CustomerID = @CustomerID)
           AND ( BS.CustomerID IS NOT NULL  AND NOT EXISTS (SELECT * FROM @RepresentedAsSwitchCarriers WHERE CarrierAccountID = bs.CustomerID))
           AND (@SupplierID IS NULL OR bs.SupplierID = @SupplierID)
           AND EXISTS (SELECT * FROM @Zones Z WHERE Z.OurZoneId = bs.SaleZoneID)
    GROUP BY
           bs.CallDate,
           bs.SaleZoneID
)
, Result AS (
	SELECT 
		Min(T.Date) AS MinDate,
		Max(T.Date) AS MaxDate,
		T.OurZoneID AS OurZoneID,
		SUM(T.Attempts) AS Attempts,
		Sum(SuccessfulAttempts) AS SuccesfulAttempts,
		SUM(DurationsInMinutes) AS DurationsInMinutes,
		B.Rate AS Rate,
		SUM(B.Amount) AS Amount,
		AVG(T.ASR) AS ASR,
		AVG(T.ACD) AS ACD 
	FROM   
		TrafficTable T WITH (NOLOCK)
		LEFT JOIN BillingTable B ON  T.OurZoneID = B.OurZoneID AND T.Date = B.Date
	GROUP BY 
		T.OurZoneID ,
		B.Rate 
	
)

select * from Result
ORDER BY Attempts DESC
END