CREATE    PROCEDURE [dbo].[SA_Traffic_TrafficBilling]
    @FromDate    datetime,
    @ToDate        datetime,
    @CustomerID        varchar(10) = NULL,
    @SupplierID        varchar(10) = NULL,
    @SwitchID        tinyInt = NULL,
    @OurZoneID         int = NULL ,
    @PeriodType VARCHAR(10)
WITH RECOMPILE
AS
BEGIN   
    SET NOCOUNT ON
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
   
    INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate);
   
    WITH TrafficTable AS
    (
    SELECT
        Period = CASE @PeriodType
        WHEN 'Hourly'   THEN datepart(hour,FirstCDRAttempt)
        WHEN 'Daily'  THEN datepart(dd,FirstCDRAttempt)
        WHEN 'Weekly' THEN datepart(ww,FirstCDRAttempt)
        ELSE NULL END,
        TS.OurZoneID AS OurZoneID,
        dateadd(dd,0, datediff(dd,0,FirstCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
        Sum(SuccessfulAttempts) AS SuccessfulAttempts,
        Sum(SuccessfulAttempts) * 100.0 / Sum(Attempts) as ASR,
        AVG(PDDInSeconds / 60.0) AS AgvPDD,
        Sum(DurationsInSeconds /60.0) as DurationsInMinutes,
        case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))  
   WHERE  
        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
        AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID)
        AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID)
        AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
        AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
    GROUP BY  CASE @PeriodType
        WHEN 'Hourly'   THEN datepart(hour,FirstCDRAttempt)
        WHEN 'Daily'  THEN datepart(dd,FirstCDRAttempt)
        WHEN 'Weekly' THEN datepart(ww,FirstCDRAttempt)
        ELSE NULL END, dateadd(dd,0, datediff(dd,0,FirstCDRAttempt))
        ,TS.OurZoneID
    ),

BillingTable AS 
(
SELECT 
        Period = CASE @PeriodType
        WHEN 'Hourly'   THEN datepart(hour,bcm.Attempt)
        WHEN 'Daily'  THEN datepart(dd,bcm.Attempt)
        WHEN 'Weekly' THEN datepart(ww,bcm.Attempt)
        ELSE NULL END
       ,bcm.OurZoneID AS OurZoneID
       ,dateadd(dd,0, datediff(dd,0,bcm.Attempt)) AS [Date]
       ,AVG(bcs.RateValue) AS SaleRate
       ,SUM(bcs.Net) AS SaleNet
       ,SUM(bcs.DurationInSeconds / 60.0) AS SaleDuration
       ,AVG(bcc.RateValue) AS CostRate
       ,SUM(bcc.Net) AS CostNet
FROM Billing_CDR_Main bcm  WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))
   LEFT JOIN Billing_CDR_Sale bcs WITH(nolock) ON bcs.ID = bcm.ID  
   LEFT JOIN Billing_CDR_Cost bcc WITH(nolock) ON bcc.ID = bcm.ID
WHERE bcm.attempt BETWEEN @FromDate AND @ToDate 
        AND (@CustomerID IS NULL OR bcm.CustomerID = @CustomerID)
        AND (@SupplierID IS NULL OR bcm.SupplierID = @SupplierID)
        AND (@SwitchID IS NULL OR bcm.SwitchID = @SwitchID)
        AND (@OurZoneID IS NULL OR bcm.OurZoneID = @OurZoneID) 
GROUP BY  CASE @PeriodType
      WHEN 'Hourly'   THEN datepart(hour,bcm.Attempt)
      WHEN 'Daily'  THEN datepart(dd,bcm.Attempt)
      WHEN 'Weekly' THEN datepart(ww,bcm.Attempt)
      ELSE NULL END, dateadd(dd,0, datediff(dd,0,bcm.Attempt))
     ,bcm.OurZoneID
)

SELECT * FROM trafficTable T
LEFT JOIN BillingTable B 
ON T.OurZoneID = B.OurZoneID 
AND T.Date = B.Date 
AND T.Period = B.Period
END