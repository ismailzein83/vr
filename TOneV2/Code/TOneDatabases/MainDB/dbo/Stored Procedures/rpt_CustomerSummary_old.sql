





CREATE PROCEDURE [dbo].[rpt_CustomerSummary_old](
	@CustomerID varchar(10) = NULL,
	@FromDate Datetime ,
	@ToDate Datetime 
)
with Recompile
	AS 
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)


BEGIN
SELECT bs.CustomerID AS Carrier
	  ,SUM(bs.SaleDuration /60.0) AS SaleDuration
      ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet
      ,SUM(bs.CostDuration /60.0) AS CostDuration
      ,SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS CostNet
FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
WHERE  (@CustomerID IS NULL OR  bs.CustomerID = @CustomerID)
 AND  bs.CallDate >= @FromDate AND  bs.CallDate <= @ToDate
GROUP BY bs.CustomerID

DECLARE @NumberOfDays INT 
SET @NumberOfDays = DATEDIFF(dd,@FromDate,@ToDate)

--SELECT isnull(SUM(ca.Services + ca.ConnectionFees) * @NumberOfDays,0) AS Services
--    FROM CarrierAccount ca 
--WHERE ca.CarrierAccountID IN 
--(
--SELECT DISTINCT bs.SupplierID	
--  FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
--WHERE  (@CustomerID IS NULL OR  bs.CustomerID = @CustomerID)
--  AND  bs.CallDate >= @FromDate
--  AND  bs.CallDate <= @ToDate
--)

--SELECT ((SUM(isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ERS.Rate,1))*@NumberOfDays) AS Services
--FROM CarrierAccount ca 
--LEFT JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
--LEFT JOIN @ExchangeRates ERS ON ERS.Currency = cp.CurrencyID AND ERS.Date = getdate()

SELECT ((SUM((isnull(ca.Services,0) +isnull(ca.ConnectionFees,0))/ISNULL(ER.Rate, 1)))*@NumberOfDays) AS Services
FROM CarrierAccount ca 
JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
LEFT JOIN @ExchangeRates ER ON ER.Currency = cp.CurrencyID AND ER.Date = @FromDate
END

RETURN