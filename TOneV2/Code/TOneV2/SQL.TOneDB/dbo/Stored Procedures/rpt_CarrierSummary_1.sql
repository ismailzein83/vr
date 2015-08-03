
CREATE PROCEDURE [dbo].[rpt_CarrierSummary]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@CustomerAmuID int = NULL,
	@SupplierAmuID int = NULL
)
WITH RECOMPILE
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
	
	SELECT bs.SupplierID AS SupplierID,
		   bs.CustomerID AS CustomerID,
		   SUM(bs.SaleDuration)/60.0 AS SaleDuration,
		   SUM(bs.CostDuration)/60.0 AS CostDuration,
		   SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1)) AS CostNet,
		   SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		   SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CostCommissionValue,
		   SUM(bs.Sale_Commissions/ISNULL(ERS.Rate, 1)) AS SaleCommissionValue,
		   SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS CostExtraChargeValue,
		   SUM(bs.Sale_ExtraCharges/ISNULL(ERS.Rate, 1)) AS SaleExtraChargeValue
	FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  bs.CallDate >= @FromDate
	  AND  bs.CallDate <= @ToDate
	  AND  ( @CustomerID IS NULL OR  bs.CustomerID = @CustomerID )
	  AND  ( @SupplierID IS NULL OR  bs.SupplierID = @SupplierID )
	  AND (@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
	  AND (@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
	GROUP BY
		   bs.SupplierID,
		   bs.CustomerID
	RETURN