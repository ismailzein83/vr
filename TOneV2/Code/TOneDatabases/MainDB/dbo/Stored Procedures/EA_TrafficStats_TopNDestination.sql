


CREATE PROCEDURE [dbo].[EA_TrafficStats_TopNDestination]
 @TopRecords   INT,
 @FromDate     DateTime,
 @ToDate       DateTime,
 @CustomerID varchar (25)= NULL,
 @SupplierID varchar (25)= NULL,
 @HighestTraffic CHAR(1) = 'Y',
 @AllAccounts varchar(max) = NULL	
AS
BEGIN
SET @FromDate = DATEADD(day,0,datediff(day,0, @FromDate))
SET @ToDate = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDate)))
DECLARE @ExchangeRates TABLE(
            Currency VARCHAR(3),
            Date SMALLDATETIME,
            Rate FLOAT
            PRIMARY KEY(Currency, Date)
        )
	
INSERT INTO @ExchangeRates
SELECT *
FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate);

Declare @Zones TABLE ( OurZoneID INT , Attempts int, DurationsInSeconds numeric(13,5), CeiledDuration bigint)

SET ROWCOUNT @TopRecords

IF(@HighestTraffic = 'Y')
INSERT INTO @Zones
    SELECT TS.OurZoneID AS OurZoneID,
           SUM(Attempts) AS Attempts,
           SUM(TS.DurationsInSeconds) AS DurationsInSeconds,
           SUM(TS.CeiledDuration) AS CeiledDuration
    FROM   TrafficStats ts WITH( NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst), INDEX(IX_TrafficStats_Customer))
    LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
    WHERE  FirstCDRAttempt BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR TS.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))  )
           AND ( TS.CustomerID IS NOT NULL  AND CA.RepresentsASwitch='N')
           AND (@SupplierID IS NULL OR TS.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
           AND ts.OurZoneID IS NOT NULL 
    GROUP BY
           TS.OurZoneID
    ORDER BY
           DurationsInSeconds DESC,
           Attempts DESC
ELSE
INSERT INTO @Zones
    SELECT TS.OurZoneID AS OurZoneID,
           SUM(Attempts) AS Attempts,
           SUM(TS.DurationsInSeconds) AS DurationsInSeconds,
           SUM(TS.CeiledDuration) AS CeiledDuration
    FROM   TrafficStats ts WITH( NOLOCK, INDEX(IX_TrafficStats_DateTimeFirst), INDEX(IX_TrafficStats_Customer))
    LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
    WHERE  FirstCDRAttempt BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR TS.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
           AND ( TS.CustomerID IS NOT NULL  AND CA.RepresentsASwitch='N')
           AND (@SupplierID IS NULL OR TS.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
           AND ts.OurZoneID IS NOT NULL 
    GROUP BY
           TS.OurZoneID
    ORDER BY
           DurationsInSeconds ASC,
           Attempts DESC
	
SET ROWCOUNT 0;

With TrafficTable AS 
(
    SELECT DATEADD(day,0,datediff(day,0, TS.FirstCDRAttempt))  AS Date,
           TS.OurZoneID,
           SUM(TS.Attempts) AS Attempts,
           SUM(TS.DurationsInSeconds / 60.) AS DurationsInMinutes,
           SUM(TS.CeiledDuration / 60.) AS CeiledDuration,
           SUM(TS.SuccessfulAttempts) * 100.0 / SUM(TS.Attempts) AS ASR,
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
    FROM   TrafficStats ts WITH(
               NOLOCK,
               INDEX(IX_TrafficStats_DateTimeFirst),
               INDEX(IX_TrafficStats_Customer)
           )
           JOIN @Zones z
                ON  TS.OurZoneID = Z.OurZoneID
                LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
    WHERE  Ts.FirstCDRAttempt BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR TS.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
           AND ( TS.CustomerID IS NOT NULL  AND CA.RepresentsASwitch='N')
           AND (@SupplierID IS NULL OR TS.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
    GROUP BY
           DATEADD(day,0,datediff(day,0, TS.FirstCDRAttempt)),
           TS.OurZoneID
),BillingTable AS 
(
    SELECT bs.CallDate AS Date,
           bs.SaleZoneID AS OurZoneID,
           AVG(bs.Sale_Rate / ISNULL(ERS.Rate,1)) AS Rate,
           SUM(bs.Sale_Nets / ISNULL(ERS.Rate,1)) AS Amount
    FROM   Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
           LEFT JOIN @ExchangeRates ERS
                ON  ERS.Currency = bs.Sale_Currency
                AND ERS.Date = bs.CallDate
                LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON bS.CustomerID = CA.CarrierAccountID
    WHERE  bs.CallDate BETWEEN @FromDate AND @ToDate
           AND (@CustomerID IS NULL OR bs.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
           AND ( BS.CustomerID IS NOT NULL  AND CA.RepresentsASwitch='N')
           AND (@SupplierID IS NULL OR bs.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
           AND EXISTS (SELECT * FROM @Zones Z WHERE Z.OurZoneId = bs.SaleZoneID)
    GROUP BY
           bs.CallDate,
           bs.SaleZoneID
)

SELECT Min(T.Date) AS MinDate,
       Max(T.Date) AS MaxDate,
       T.OurZoneID AS OurZoneID,
       SUM(T.Attempts) AS Attempts,
       Sum(SuccessfulAttempts) AS SuccesfulAttempts,
       SUM(DurationsInMinutes) AS DurationsInMinutes,
       SUM(CeiledDuration) AS CeiledDuration,
       B.Rate AS Rate,
       SUM(B.Amount) AS Amount,
       AVG(T.ASR) AS ASR,
       AVG(T.ACD) AS ACD 
FROM   TrafficTable T
       LEFT JOIN BillingTable B ON  T.OurZoneID = B.OurZoneID AND T.Date = B.Date
GROUP BY T.OurZoneID ,B.Rate
ORDER BY Attempts DESC 

END