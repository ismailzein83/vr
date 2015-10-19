
CREATE PROCEDURE [Analytics].[SP_BillingRep_GetRoutingAnalysis]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomersID varchar(max),
	@SuppliersID varchar(max),
	@Top int = 100,
	@CustomerAmuID varchar(max),
	@SupplierAmuID varchar(max),
	@CurrencyID varchar(3) = NULL
)
WITH Recompile
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
	

CREATE TABLE #Zones(
			ID int,
			Durations float,
			PRIMARY KEY(ID))
	
INSERT INTO #Zones 
SELECT bs.SaleZoneID AS ID,
	   SUM(bs.SaleDuration)
FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
WHERE  bs.CallDate >= @FromDate
  AND  bs.CallDate <= @ToDate
  AND(@CustomersID IS NULL OR bs.CustomerID In (SELECT * FROM @CustomersIDs)) 
  AND(@SuppliersID IS NULL OR bs.SupplierID IN( SELECT * FROM @SuppliersIDs))
  AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
  AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
GROUP BY
	   bs.SaleZoneID 
ORDER BY SUM(bs.SaleDuration) DESC 

DECLARE @Zones TABLE(ID INT)

SET ROWCOUNT @Top
INSERT INTO @Zones SELECT #Zones.ID FROM #Zones ORDER BY Durations DESC 
SET ROWCOUNT 0

SELECT bs.SaleZoneID AS SaleZoneID,
	   bs.SupplierID AS SupplierID,
	   SUM(ISNULL(bs.SaleDuration,0))/60.0 AS Duration,
	   SUM(ISNULL(bs.Cost_Nets,0) / ISNULL(ERC.Rate, 1)) AS CostNet,
	   SUM(ISNULL(bs.Sale_Nets,0) / ISNULL(ERS.Rate, 1)) AS SaleNet,
	   avg(TS.ASR) AS ASR,
	   Avg(TS.ACD) AS ACD
FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
	LEFT JOIN GetSupplierZoneStats(@FromDate,@ToDate,NULL) AS TS ON TS.SupplierID = bs.SupplierID AND TS.OurZoneID = bs.SaleZoneID
	LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
	LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
WHERE  bs.CallDate >= @FromDate
  AND  bs.CallDate <= @ToDate
  AND(@CustomersID IS NULL OR bs.CustomerID In (SELECT * FROM @CustomersIDs)) 
  AND(@SuppliersID IS NULL OR bs.SupplierID IN( SELECT * FROM @SuppliersIDs))
  AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
  AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
  AND  bs.SaleZoneID IN (SELECT Id FROM @Zones)
GROUP BY
	   bs.SaleZoneID,
	   bs.SupplierID 
	   
DROP TABLE #Zones

RETURN