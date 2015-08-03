
CREATE PROCEDURE [dbo].[rpt_DailySummary](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerAmuID int = NULL,
	@SupplierAmuID int = NULL 
)
with Recompile
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
	
     SELECT 
			CAST(bs.calldate AS varchar(11))  [Day],
			SUM(bs.NumberOfCalls) AS Calls,
			SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],    
			SUM(bs.SaleDuration / 60.0) [SaleDuration], 
			SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet,           			
			SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet
		    FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
		    LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate			
            LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
            WHERE bs.calldate >=@fromdate AND bs.calldate<=@ToDate
            AND (@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND (@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
			GROUP BY 
			    CAST(bs.calldate AS varchar(11))   
			ORDER BY CAST(bs.calldate AS varchar(11)) ASC	
	
	RETURN