
CREATE PROCEDURE [Analytics].[SP_BillingRep_GetSaleZoneCostSummary](
	@FromDate DATETIME, 
	@ToDate DATETIME,
	@GroupBy NVARCHAR(20),
	@CustomerAmuID varchar(max),
	@SupplierAmuID varchar(max),
	@CurrencyID varchar(3) = NULL
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

	IF @GroupBy = 'AverageCost'
	
	SELECT SUM(bs.cost_nets) /sum(bs.CostDuration) / sum(ISNULL(ERC.Rate, 1))*60 AS 'AvgCost',
	       bs.salezoneID AS salezoneID,
	       SUM(bs.CostDuration)/60 AS 'AvgDuration'
	FROM   billing_stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
	  left   JOIN @ExchangeRates ERC
	         ON  bs.Cost_Currency= ERC.Currency 
	          AND ERC.Date = bs.CallDate
	WHERE  bs.calldate >=@FromDate AND bs.calldate<=@ToDate
			AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
	GROUP BY
	       bs.salezoneID
	
	IF @GroupBy = 'Supplier'
	
	SELECT bs.SupplierID,
           --MAX(bs.Cost_Rate) AS HighestRate, 
           SUM(bs.cost_nets) /sum(bs.CostDuration) / sum(ISNULL(ERC.Rate, 1))*60 AS 'HighestRate',
           bs.SaleZoneID,
           SUM(bs.CostDuration)/60 AS AvgDuration      
	FROM   dbo.Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date)) 
	LEFT JOIN @ExchangeRates ERC ON  ERC.Currency = bs.Cost_Currency
	           AND ERC.Date = bs.CallDate
    WHERE   bs.calldate >=@FromDate AND bs.calldate<=@ToDate
			AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
GROUP BY bs.SupplierID, 
         bs.SaleZoneID



IF @GroupBy = 'Service'

SELECT 
SUM(bs.cost_nets) /sum(bs.CostDuration) / sum(ISNULL(ERC.Rate, 1))*60 AS 'AvgServiceCost',
	       bs.salezoneID AS salezoneID,
	       fs.Symbol AS 'Service',
	       SUM(bs.CostDuration)/60 AS 'AvgDuration'
	FROM      
dbo.Billing_Stats bs inner JOIN Rate r ON r.ZoneID = bs.costzoneid
	 right join  dbo.FlaggedService fs on
                       fs.FlaggedServiceID = r.ServicesFlag 
   left JOIN @ExchangeRates ERC
	       ON  ERC.Currency = bs.Cost_Currency
	WHERE  bs.calldate >=@FromDate AND bs.calldate<=@ToDate
	AND r.IsEffective='Y'
	AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
	GROUP BY
	       bs.salezoneID,
	       fs.Symbol
	RETURN