






CREATE PROCEDURE [dbo].[rpt_SaleZoneCostSummary](
	@FromDate DATETIME, 
	@ToDate DATETIME,
	@GroupBy NVARCHAR(20),
	@CustomerAmuID int = NULL,
	@SupplierAmuID int = NULL)
WITH RECOMPILE
AS
	DECLARE @ExchangeRates TABLE(
	            Currency VARCHAR(3),
	            Date SMALLDATETIME,
	            Rate FLOAT
	            PRIMARY KEY(Currency, Date)
	        )
	
	INSERT INTO @ExchangeRates
	SELECT *
	FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	

	       
	DECLARE @CustomerIDs TABLE( CarrierAccountID VARCHAR(5) )
	DECLARE @SupplierIDs TABLE( CarrierAccountID VARCHAR(5) )

	--IF(@CustomerAMUID IS NOT NULL)
	--BEGIN
	--	INSERT INTO @CustomerIDs
	--	SELECT ac.CarrierAccountID
	--	FROM AMU_Carrier ac
	--	WHERE ac.AMUCarrierType = 0
	--	AND ac.AMUID = @CustomerAMUID
	--END

	--IF(@SupplierAMUID IS NOT NULL)
	--BEGIN	
	--	INSERT INTO @SupplierIDs
	--	SELECT ac.CarrierAccountID
	--	FROM AMU_Carrier ac
	--	WHERE ac.AMUCarrierType = 1
	--	AND ac.AMUID = @SupplierAMUID
	--END
	
	IF(@CustomerAMUID IS NOT NULL)
	BEGIN
		DECLARE @customerAmuFlag VARCHAR(20)
		SET @customerAmuFlag = (SELECT Flag FROM AMU WHERE ID = @CustomerAMUID)
		INSERT INTO @CustomerIDs
		SELECT ac.CarrierAccountID
		FROM AMU_Carrier ac
		WHERE ac.AMUCarrierType = 0
		AND ac.AMUID IN (
			SELECT ID FROM AMU
			WHERE Flag LIKE @customerAmuFlag + '%'
			)
	END

	IF(@SupplierAMUID IS NOT NULL)
	BEGIN	
		DECLARE @supplierAmuFlag VARCHAR(20)
		SET @supplierAmuFlag = (SELECT Flag FROM AMU WHERE ID = @SupplierAMUID)
		INSERT INTO @SupplierIDs
		SELECT ac.CarrierAccountID
		FROM AMU_Carrier ac
		WHERE ac.AMUCarrierType = 1
		AND ac.AMUID IN (
			SELECT ID FROM AMU
			WHERE Flag LIKE @supplierAmuFlag + '%'
			)
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