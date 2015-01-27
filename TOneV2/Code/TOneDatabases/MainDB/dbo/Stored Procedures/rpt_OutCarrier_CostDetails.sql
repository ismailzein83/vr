




CREATE PROCEDURE [dbo].[rpt_OutCarrier_CostDetails]
(@fromDate DATETIME, @toDate DATETIME,
	@CustomerAmuID int = NULL,
	@SupplierAmuID int = NULL)
AS
	DECLARE @ExchangeRates TABLE(
	            Currency VARCHAR(3),
	            Date SMALLDATETIME,
	            Rate FLOAT
	            PRIMARY KEY(Currency, Date)
	        )
	
	DECLARE @CustomerIDs TABLE( CarrierAccountID VARCHAR(5) )
	DECLARE @SupplierIDs TABLE( CarrierAccountID VARCHAR(5) )

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

	INSERT INTO @ExchangeRates
	SELECT *
	FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	
	SELECT bs.SupplierID AS Carrier,bs.CustomerID AS Customer,
	       SUM(isnull(bs.CostDuration,0) / 60.0) AS 'Duration',
	       SUM(isnull(bs.Cost_Nets,0)) AS 'Amount'
	FROM   Billing_Stats bs WITH(NOLOCK)
	       LEFT JOIN @ExchangeRates ERC
	            ON  ERC.Currency = bs.Cost_Currency
	            AND ERC.Date = bs.CallDate
	WHERE  bs.CallDate >= @fromDate AND bs.CallDate <=@toDate
	AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
  AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
	GROUP BY
	       bs.CustomerID,bs.SupplierID