
CREATE PROCEDURE [Analytics].[SP_BillingRep_GetCarrierLostReport]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomersID VARCHAR(max)=NULL,
	@SuppliersID VARCHAR(max)=NULL,
	@Margin int,
	@CustomerAmuID VARCHAR(max)=NULL,
	@SupplierAmuID VARCHAR(max)=NULL,
	@CurrencyID varchar(3)
)
WITH RECOMPILE
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
	
	DECLARE @SuppliersIDs TABLE (SupplierId varchar(10))
		INSERT INTO @SuppliersIDs (SupplierId)
		select  ParsedString  from [BEntity].[ParseStringList](@SuppliersID)
		
	DECLARE @CustomersIDs TABLE (CustomerId varchar(10))
		INSERT INTO @CustomersIDs (CustomerId)
		select  ParsedString  from [BEntity].[ParseStringList](@CustomersID)
		
	SELECT  bs.CustomerID AS CustomerID,
			bs.SaleZoneID AS SaleZoneID,
			bs.CostZoneID AS CostZoneID,
			bs.SupplierID AS SupplierID,
			SUM(bs.SaleDuration)/60.0 AS Duration,
			SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS CostNet,
			SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet
		FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		JOIN CarrierAccount cac ON cac.CarrierAccountID = bs.CustomerID AND cac.IsPassThroughCustomer ='N' 
		JOIN CarrierAccount cas ON cas.CarrierAccountID = bs.SupplierID AND cas.IsPassThroughSupplier ='N'
	    WHERE	  bs.CallDate >= @FromDate
			AND	  bs.CallDate <= @ToDate
			AND  (@CustomersID IS NULL OR  bs.CustomerID IN (SELECT * FROM @CustomersIDs))
			AND  (@SuppliersID IS NULL OR  bs.SupplierID IN (SELECT * FROM @SuppliersIDs))
			AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
		    AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))	
		GROUP BY
		bs.CustomerID, bs.SaleZoneID, bs.CostZoneID, bs.SupplierID
	HAVING  ( 1 - (sum(bs.cost_nets / ISNULL(ERC.Rate, 1)) / sum(bs.sale_nets / ISNULL(ERS.Rate, 1))) ) * 100  < @Margin
RETURN