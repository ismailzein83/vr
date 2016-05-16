
CREATE  PROCEDURE [TOneWhS_Billing].[SP_BillingRep_GetZoneProfitsV2](
	@FromDate Datetime ,
	@ToDate Datetime,
	@CustomersID varchar(max),
	@SuppliersID varchar(max),
	@CurrencyID INT
)with Recompile
	AS
	
	IF(@CurrencyID IS NULL)
	BEGIN
		Select @CurrencyID =  ID From Common.Currency as c 
	END
	
	DECLARE @MainExchangeRates TABLE(
		Currency INT,
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	
	DECLARE @ExchangeRates TABLE(
		Currency INT,
		[Date] SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	
    INSERT INTO @MainExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

    INSERT INTO @ExchangeRates Select exRate1.Currency , exRate1.Date , exRate1.Rate/ exRate2.Rate as Rate from @MainExchangeRates as exRate1 join @MainExchangeRates as exRate2 on exRate2.Currency = @CurrencyID and exRate1.Date = exRate2.Date
		
	DECLARE @CustomerIDs TABLE( CarrierAccountID VARCHAR(5) )
	DECLARE @SupplierIDs TABLE( CarrierAccountID VARCHAR(5) )
	
	DECLARE @SuppliersIDs TABLE (SupplierId varchar(10))
		INSERT INTO @SuppliersIDs (SupplierId)
		select  ParsedString  from [ParseStringList](@SuppliersID)
		
	DECLARE @CustomersIDs TABLE (CustomerId varchar(10))
		INSERT INTO @CustomersIDs (CustomerId)
		select  ParsedString  from [ParseStringList](@CustomersID)	
		
		
	BEGIN
    SELECT 
		bs.SupplierID  AS SupplierID, 
		bs.SaleZoneID  AS SaleZoneID, 
		bs.SupplierZoneID  AS SupplierZoneID, 
		SUM(bs.NumberOfCalls) AS Calls,
		SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
		SUM(bs.CostDuration/60.0) AS CostDuration,
		SUM(bs.SaleDuration/60.0) AS SaleDuration, 
		SUM(bs.CostNets / ISNULL(ERC.Rate, 1)) AS  CostNet, 
		SUM(bs.SaleNets / ISNULL(ERS.Rate, 1)) AS SaleNet
	 FROM
	     [TOneWhS_Analytic].[BillingStats] bs WITH(NOLOCK,INDEX(IX_BillingStats_ID))
	    LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.CostCurrency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.SaleCurrency AND ERS.Date = bs.CallDate
		
	WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
		 	  AND (@CustomersID IS NULL OR bs.CustomerID In (SELECT * FROM @CustomersIDs )) 
          AND (@SuppliersID IS NULL OR bs.SupplierID IN( SELECT * FROM @SuppliersIDs ))
	GROUP BY bs.SupplierID, bs.SaleZoneID, bs.SupplierZoneID
	ORDER BY bs.SupplierID ASC 
	END
RETURN