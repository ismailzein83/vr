



CREATE    PROCEDURE [dbo].[rpt_ExchangeCarriersSummary](
	@FromDate DATETIME, 
	@ToDate DATETIME,	
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
	FROM   dbo.GetDailyExchangeRates(@FromDate, @ToDate);
	
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

	BEGIN
		--Customer part 
		WITH CustomerSummary AS 
		(
		    SELECT bs.CustomerID AS CarrierID,
		           SUM(isnull(bs.Sale_Nets,0) / ISNULL(ERS.Rate, 1)) AS Sale,
		           SUM(isnull(bs.Cost_Nets,0) / ISNULL(ERC.Rate, 1)) AS Cost
		    FROM   Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
		           LEFT JOIN @ExchangeRates ERS
		                ON  ERS.Currency = bs.Sale_Currency
		                AND ERS.Date = bs.CallDate
		           LEFT JOIN @ExchangeRates ERC
		                ON  ERC.Currency = bs.Cost_Currency
		                AND ERC.Date = bs.CallDate
		    WHERE  bs.CallDate >= @FromDate
		           AND bs.CallDate <=@ToDate
				AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
				AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
		    GROUP BY
		           bs.CustomerID
		),
		SupplierSummary AS 
		(
		    -- Supplier part 
		    SELECT bs.SupplierID AS CarrierID,
		           SUM(isnull(bs.CostDuration,0) / 60.0) AS Duration,
		           SUM(isnull(bs.Sale_Nets,0) / ISNULL(ERS.Rate, 1)) AS Sale,
		           SUM(isnull(bs.Cost_Nets,0) / ISNULL(ERC.Rate, 1)) AS Cost
		    FROM   Billing_Stats bs WITH(NOLOCK, INDEX(IX_Billing_Stats_Date))
		           LEFT JOIN @ExchangeRates ERS
		                ON  ERS.Currency = bs.Sale_Currency
		                AND ERS.Date = bs.CallDate
		           LEFT JOIN @ExchangeRates ERC
		                ON  ERC.Currency = bs.Cost_Currency
		                AND ERC.Date = bs.CallDate
		    WHERE  bs.CallDate >= @FromDate
		           AND bs.CallDate <=@ToDate
		           AND(@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
				AND(@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
		    GROUP BY
		           bs.SupplierID
		) 
		
		
		SELECT  CASE WHEN  CS.CarrierID IS NULL  THEN SS.CarrierID
		            ELSE   CS.CarrierID END AS CustomerID,
		       CS.Sale - CS.Cost AS CustomerProfit,
		       SS.Sale - SS.Cost AS SupplierProfit
		FROM   CustomerSummary CS
		       FULL JOIN SupplierSummary SS
		            ON  CS.CarrierID = SS.CarrierID
	END
	
	RETURN