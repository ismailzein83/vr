
CREATE PROCEDURE [Analytics].[SP_BillingRep_GetDetailedCarrierSummary](
	@FromDate DATETIME,
	@ToDate DATETIME,
	@CustomersID varchar(max),
	@SuppliersID varchar(max),
	@CustomerAmuID varchar(max),
	@SupplierAmuID varchar(max),
	@CurrencyID varchar(3) = NULL
)
	
AS
BEGIN
	SET NOCOUNT ON;
	
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
	
	;WITH Billing AS (
		SELECT bs.CallDate
				, bs.CustomerID
				, bs.SupplierID
				, bs.CostZoneID
				, bs.SaleZoneID
				, bs.Cost_Currency
				, bs.Sale_Currency
				, bs.Cost_Nets / ISNULL(ERC.Rate, 1) AS Cost_Nets
				, bs.Sale_Nets / ISNULL(ERS.Rate, 1) AS Sale_Nets
				, bs.Sale_Rate / ISNULL(ERS.Rate, 1) AS Sale_Rate
				, bs.Cost_Rate / ISNULL(ERC.Rate, 1) AS Cost_Rate
				, bs.SaleDuration
				, bs.CostDuration
		FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		WHERE bs.CallDate BETWEEN @FromDate AND @ToDate
			 AND (@CustomersID IS NULL OR bs.CustomerID In (SELECT * FROM @CustomersIDs )) 
	         AND (@SuppliersID IS NULL OR bs.SupplierID IN( SELECT * FROM @SuppliersIDs ))
	         AND (@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
	)

	SELECT bs.CustomerID AS CustomerID
		   , sz.ZoneID AS SaleZoneID
		   , sz.Name AS SaleZoneName
		   , SUM(bs.SaleDuration)/60. AS SaleDuration
		   , bs.Sale_Rate as SaleRate
		   , sr.change as SaleRateChange
		   , sr.BeginEffectiveDate as SaleRateEffectiveDate
		   , SUM(bs.sale_nets/ISNULL(ERS.Rate, 1)) SaleAmount
		   , bs.SupplierID 
		   , cz.ZoneID As CostZoneID
		   , cz.Name AS CostZoneName
		   , SUM(bs.CostDuration)/60. as CostDuration
		   , bs.Cost_Rate AS CostRate
		   , cr.change as CostRateChange
		   , cr.begineffectivedate as CostRateEffectiveDate
		   , SUM(bs.cost_nets/ISNULL(ERC.Rate, 1)) AS CostAmount
		   , SUM(bs.sale_nets/ISNULL(ERS.Rate, 1) - bs.cost_nets/ISNULL(ERC.Rate, 1)) as Profit
	FROM Billing bs
		JOIN Zone sz ON bs.salezoneid = sz.zoneid
		JOIN Zone cz ON bs.costzoneid = cz.zoneid
		JOIN Rate sr WITH(NOLOCK, INDEX(IX_Rate_Zone)) ON sr.zoneid = sz.zoneid
		JOIN Pricelist sp ON sr.pricelistid = sp.pricelistid
		JOIN Rate cr WITH(NOLOCK, INDEX(IX_Rate_Zone)) ON cr.zoneid = cz.zoneid
		JOIN Pricelist cp ON cr.pricelistid = cp.pricelistid
		JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE calldate BETWEEN @FromDate AND @ToDate
		AND sr.begineffectivedate < bs.calldate AND (sr.endeffectivedate IS NULL OR sr.endeffectivedate > bs.calldate)
		AND bs.customerid = sp.customerid
		AND cr.begineffectivedate < bs.calldate AND (cr.endeffectivedate IS NULL OR cr.endeffectivedate > bs.calldate)
		AND bs.supplierid = cp.supplierid
	GROUP BY bs.CustomerID
			, sz.ZoneID
			, sz.Name   
			, bs.Sale_Rate
			, sr.change
			, sr.BeginEffectiveDate	   
			, bs.SupplierID
			, cz.ZoneID
			, cz.Name	   
			, bs.Cost_Rate
			, cr.change
			, cr.begineffectivedate

END