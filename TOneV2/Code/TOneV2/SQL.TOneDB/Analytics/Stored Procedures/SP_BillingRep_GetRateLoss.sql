


CREATE  PROCEDURE [Analytics].[SP_BillingRep_GetRateLoss](
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomersID varchar(max),
	@SuppliersID varchar(max),
	@zoneIds varchar(max),
	--@Margin FLOAT = 0,
	@CustomerAmuID varchar(max),
	@SupplierAmuID varchar(max),
	@CurrencyID varchar(3) = NULL
)with Recompile
	AS 
	
	DECLARE @Margin FLOAT = 0;
	
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
	
	DECLARE @ZoneIdsTable TABLE (ZoneId int)
	INSERT INTO @ZoneIdsTable (ZoneId)
	select Convert(int, ParsedString) from [BEntity].[ParseStringList](@zoneIds)
		
	
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
		CZ.Name AS CostZone, 
		SZ.Name AS SaleZone, 
		bs.SupplierID  AS SupplierID, 
		bs.CustomerID AS CustomerID,
		AVG(bs.Sale_Rate / ISNULL(ERS.Rate, 1)) AS SaleRate,
		AVG(bs.Cost_Rate / ISNULL(ERC.Rate, 1)) AS CostRate,
		SUM(bs.CostDuration/60.0) AS CostDuration,
		SUM(bs.SaleDuration/60.0) AS SaleDuration, 
		SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet, 
		SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		Sz.ZoneID AS SaleZoneID         			
	 FROM
	     Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer),INDEX(IX_Billing_Stats_Supplier))
	    LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = bs.SaleZoneID
		LEFT JOIN Zone CZ WITH(NOLOCK) ON CZ.ZoneID = bs.CostZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate	 	
          AND (@zoneIds is Null or SZ.ZoneID IN (SELECT ZoneID FROM @ZoneIdsTable)  )
		  AND (@CustomersID IS NULL OR bs.CustomerID In (SELECT * FROM @CustomersIDs )) 
          AND (@SuppliersID IS NULL OR bs.SupplierID IN( SELECT * FROM @SuppliersIDs ))
          AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
		  AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
	GROUP BY 
	    CZ.Name,SZ.Name,bs.SupplierID,bs.CustomerID,sz.ZoneID
	HAVING 	
        (SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) - SUM(bs.sale_nets / ISNULL(ERS.Rate, 1))) > @Margin
  
	  
	
RETURN