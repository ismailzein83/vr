CREATE  PROCEDURE [dbo].[Sp_AccountManagerStatus]
    @FromDate    datetime,
    @ToDate      DATETIME,
    @Type VARCHAR(20)
WITH RECOMPILE
AS
BEGIN   
    SET NOCOUNT ON;
    
    DECLARE @ExchangeRates TABLE(
        Currency VARCHAR(3),
        Date SMALLDATETIME,
        Rate FLOAT
        PRIMARY KEY(Currency, Date)
    )
   
    INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate);
IF(@Type = 'Customers')
  BEGIN  
    WITH TrafficTable AS
    (
    SELECT
        TS.CustomerID AS CarrierAccountID,
        Sum(Attempts) as Attempts,
        Sum(SuccessfulAttempts) AS SuccessfulAttempts,
        Sum(SuccessfulAttempts) * 100.0 / Sum(Attempts) as ASR,
        Sum(DurationsInSeconds /60.0) as DurationsInMinutes,
        case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))  
   WHERE  
        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
    GROUP BY 
        TS.CustomerID
    ),

BillingTable AS 
(
SELECT 
        bs.CustomerID AS CarrierAccountID
       ,AVG(bs.Sale_Rate / ISNULL(ERS.Rate, 1)) AS SaleRate
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet
       ,SUM(bs.SaleDuration / 60.0) AS SaleDuration
       ,AVG(bs.Cost_Rate / ISNULL(ERC.Rate, 1)) AS CostRate
       ,SUM(bs.Cost_Nets /ISNULL(ERC.Rate, 1)) AS CostNet
       ,SUM(bs.CostDuration / 60.0) AS CostDuration
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1) - bs.Cost_Nets / ISNULL(ERC.Rate, 1)) / sum(bs.SaleDuration / 60.0)  AS MarginPerMinute
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1) - bs.Cost_Nets / ISNULL(ERC.Rate, 1))  AS Margin  
FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
WHERE bs.CallDate BETWEEN  @FromDate AND @ToDate  
GROUP BY bs.CustomerID 
)

SELECT * FROM trafficTable T
LEFT JOIN BillingTable B 
ON T.CarrierAccountID = B.CarrierAccountID
 
  END
  
  IF(@Type = 'Suppliers')
  BEGIN  
    WITH TrafficTable AS
    (
    SELECT
        TS.SupplierID AS CarrierAccountID,
        Sum(Attempts) as Attempts,
        Sum(SuccessfulAttempts) AS SuccessfulAttempts,
        Sum(SuccessfulAttempts) * 100.0 / Sum(Attempts) as ASR,
        Sum(DurationsInSeconds /60.0) as DurationsInMinutes,
        case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))  
   WHERE  
        FirstCDRAttempt BETWEEN @FromDate AND @ToDate
    GROUP BY 
        TS.SupplierID
    ),

BillingTable AS 
(
SELECT 
        bs.SupplierID AS CarrierAccountID
       ,AVG(bs.Sale_Rate / ISNULL(ERS.Rate, 1)) AS SaleRate
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet
       ,SUM(bs.SaleDuration / 60.0) AS SaleDuration
       ,AVG(bs.Cost_Rate / ISNULL(ERC.Rate, 1)) AS CostRate
       ,SUM(bs.Cost_Nets /ISNULL(ERC.Rate, 1)) AS CostNet
       ,SUM(bs.CostDuration / 60.0) AS CostDuration
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1) - bs.Cost_Nets / ISNULL(ERC.Rate, 1)) / sum(bs.SaleDuration / 60.0)  AS MarginPerMinute
       ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1) - bs.Cost_Nets / ISNULL(ERC.Rate, 1))  AS Margin  
FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
WHERE bs.CallDate BETWEEN  @FromDate AND @ToDate  
GROUP BY bs.SupplierID 
)

SELECT * FROM trafficTable T
LEFT JOIN BillingTable B 
ON T.CarrierAccountID = B.CarrierAccountID
 
  END
   
END