
CREATE PROCEDURE [Analytics].[SP_BillingRep_GetCustomerRouting]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomersID varchar(max),
	@SuppliersID varchar(max),
	@CustomerAmuID varchar(max),
	@SupplierAmuID varchar(max),
	@CurrencyID varchar(3) = NULL
)
WITH Recompile
AS
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
	
	DECLARE @SuppliersIDs TABLE (SupplierId varchar(10))
	INSERT INTO @SuppliersIDs (SupplierId)
	select  ParsedString  from [BEntity].[ParseStringList](@SuppliersID)
		
	DECLARE @CustomersIDs TABLE (CustomerId varchar(10))
	INSERT INTO @CustomersIDs (CustomerId)
	select  ParsedString  from [BEntity].[ParseStringList](@CustomersID)
	
	SELECT 
		bs.CallDate AS CallDate,
		bs.SaleZoneID AS SaleZone, 
		bs.CostZoneID AS CostZone, 
		bs.CustomerID AS CustomerID,
		bs.SupplierID  AS SupplierID,
		bs.Sale_Rate  / ISNULL(ERS.Rate, 1) AS SaleRate,
		bs.Cost_Rate / ISNULL(ERC.Rate, 1) AS CostRate,
		SUM(bs.SaleDuration/60.0) AS SaleDuration,
		SUM(bs.CostDuration/60.0) AS CostDuration,
		SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet         			
    FROM
	    Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer),INDEX(IX_Billing_Stats_Supplier))
	    LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = bs.SaleZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE        
			bs.calldate>=@FromDate AND bs.calldate<@ToDate
		AND (@CustomersID IS NULL OR bs.CustomerID In (SELECT * FROM @CustomersIDs )) 
	    AND (@SuppliersID IS NULL OR bs.SupplierID IN( SELECT * FROM @SuppliersIDs ))
        AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
		AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
        AND bs.Sale_Rate <> 0
	 	AND bs.Cost_Rate <> 0
	GROUP BY 
	     bs.CallDate
	    ,bs.CustomerID
	    ,bs.SupplierID
	    ,bs.SaleZoneID
	    ,bs.CostZoneID
	  	,bs.Sale_Rate  / ISNULL(ERS.Rate, 1) 
		,bs.Cost_Rate / ISNULL(ERC.Rate, 1)
	ORDER BY 
	     bs.CallDate
	    ,bs.CustomerID
	    ,bs.SupplierID
	    ,bs.SaleZoneID
	    ,bs.CostZoneID
		,bs.Sale_Rate  / ISNULL(ERS.Rate, 1) 
		,bs.Cost_Rate / ISNULL(ERC.Rate, 1)
	

RETURN