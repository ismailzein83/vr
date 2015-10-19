



CREATE    PROCEDURE [Analytics].[SP_BillingRep_GetExchangeCarriersSummary](
	@FromDate DATETIME, 
	@ToDate DATETIME,	
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