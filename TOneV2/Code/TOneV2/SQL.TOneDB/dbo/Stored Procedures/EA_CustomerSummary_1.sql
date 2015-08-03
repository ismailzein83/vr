

CREATE PROCEDURE [dbo].[EA_CustomerSummary](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL
	)
with Recompile
	AS 
	
	SET @FromDate = dbo.DateOf(@FromDate)
	SET @ToDate = dbo.DateOf(@ToDate)
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	SELECT 
		bs.SaleZoneID AS SaleZoneID,
		bs.Sale_Rate AS SaleRate,
		bs.Sale_Currency AS SaleCurrency,
		SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		SUM(bs.NumberOfCalls) AS NumberOfCalls, 
		SUM(bs.SaleDuration)/60.0 AS DurationsInMinutes
		--,SUM(bs.Sale_Rate * bs.SaleDuration) AS TotalRate
	FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date, IX_Billing_Stats_Customer))
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  bs.calldate BETWEEN @FromDate AND @ToDate
		AND bs.CustomerID = @CustomerID
	GROUP BY 
		bs.SaleZoneID,
		bs.Sale_Rate,
		bs.Sale_Currency

  RETURN