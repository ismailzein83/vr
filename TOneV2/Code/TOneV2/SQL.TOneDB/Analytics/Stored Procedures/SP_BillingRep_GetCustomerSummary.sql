
CREATE PROCEDURE [Analytics].[SP_BillingRep_GetCustomerSummary](
	@CustomersID VARCHAR(max)=NULL,
	@FromDate Datetime ,
	@ToDate Datetime,
	@CustomerAmuID VARCHAR(max)=NULL,
	@SupplierAmuID VARCHAR(max)=NULL,
	@CurrencyID varchar(3)
)
with Recompile
	AS 
	
	IF(@CurrencyID IS NULL)
	BEGIN
		Select @CurrencyID = CurrencyID From Currency as c where c.IsMainCurrency = 'Y'
	END
	
	DECLARE @MainExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

    INSERT INTO @MainExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

    INSERT INTO @ExchangeRates Select exRate1.Currency , exRate1.Date , exRate1.Rate/ exRate2.Rate as Rate from @MainExchangeRates as exRate1 join @MainExchangeRates as exRate2 on exRate2.Currency = @CurrencyID and exRate1.Date = exRate2.Date

	DECLARE @CustomerIDs TABLE( CarrierAccountID VARCHAR(5) )
	DECLARE @SupplierIDs TABLE( CarrierAccountID VARCHAR(5) )

   IF(@CustomerAMUID IS NOT NULL)
	BEGIN
		INSERT INTO @CustomerIDs (CarrierAccountID)
		select  ParsedString  from [BEntity].[ParseStringList](@CustomerAMUID)	
	END

	IF(@SupplierAMUID IS NOT NULL)
	BEGIN	
		INSERT INTO @SupplierIDs (CarrierAccountID)
		select  ParsedString  from [BEntity].[ParseStringList](@SupplierAmuID)	
	END
	
	DECLARE @CustomersIDs TABLE (CustomerId varchar(10))
	INSERT INTO @CustomersIDs (CustomerId)
	select  ParsedString  from [BEntity].[ParseStringList](@CustomersID)

BEGIN
SELECT bs.CustomerID AS Carrier
	  ,SUM(bs.SaleDuration /60.0) AS SaleDuration
      ,SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet
      ,SUM(bs.CostDuration /60.0) AS CostDuration
      ,SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS CostNet
FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
WHERE  (@CustomersID IS NULL OR  bs.CustomerID IN (SELECT * FROM @CustomersIDs))
 AND  bs.CallDate >= @FromDate AND  bs.CallDate <= @ToDate
 AND (@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
 AND (@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
GROUP BY bs.CustomerID


END

RETURN