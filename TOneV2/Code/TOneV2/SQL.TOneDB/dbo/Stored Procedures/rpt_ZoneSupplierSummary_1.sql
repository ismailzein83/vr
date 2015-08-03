
CREATE PROCEDURE [dbo].[rpt_ZoneSupplierSummary]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Top int = 100,
	@CustomerAmuID int = NULL,
	@SupplierAmuID int = NULL
)
WITH Recompile
AS
	
 DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

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
  AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
  AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
  AND  ( @CustomerID IS NULL OR  bs.CustomerID = @CustomerID )
  AND  ( @SupplierID IS NULL OR  bs.SupplierID = @SupplierID )
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
  AND  ( @CustomerID IS NULL OR  bs.CustomerID = @CustomerID )
  AND  ( @SupplierID IS NULL OR  bs.SupplierID = @SupplierID )
  AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
  AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
  AND  bs.SaleZoneID IN (SELECT Id FROM @Zones)
GROUP BY
	   bs.SaleZoneID,
	   bs.SupplierID 
	   
DROP TABLE #Zones

RETURN