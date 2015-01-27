
CREATE PROCEDURE [dbo].[rpt_DailyCarrierSummary](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Cost char(1)='Y',
	@IsGroupedByDay char(1)='N',
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
	
  IF @cost='Y'
		BEGIN
     		SELECT 
   			CASE WHEN @IsGroupedByDay ='Y' THEN cast(bs.calldate  AS varchar(11)) ELSE  bs.SupplierID END AS [Day],
   			bs.SupplierID AS CarrierID,
			SUM(bs.NumberOfCalls) AS Attempts, 
			SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet], 
			SUM(bs.CostDuration)/60.0 AS Duration,
			SUM(bs.Cost_Nets / ISNULL(ERC.Rate, 1))  AS Net
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			WHERE  bs.calldate>=@FromDate AND bs.calldate<=@ToDate
			AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
			AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)
			AND (@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND (@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
			GROUP BY 
			bs.SupplierID,CASE WHEN @IsGroupedByDay ='Y' THEN cast(bs.calldate AS varchar(11))ELSE  bs.SupplierID  END
		END
  ELSE 
		BEGIN
			SELECT 
			CASE WHEN @IsGroupedByDay ='Y' THEN cast(bs.calldate AS varchar(11)) ELSE  bs.CustomerID END AS [Day], 
			bs.CustomerID AS CarrierID,
			SUM(bs.NumberOfCalls) AS Attempts, 
			SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet], 
			SUM(bs.SaleDuration)/60.0 AS Duration,
			SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net
			FROM   Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
			WHERE  bs.calldate >=@FromDate AND bs.calldate<=@ToDate
			AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
			AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)
			AND (@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND (@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
			GROUP BY 
			bs.CustomerID,CASE WHEN @IsGroupedByDay ='Y' THEN cast(bs.calldate AS varchar(11))ELSE  bs.CustomerID END  
        END           
RETURN