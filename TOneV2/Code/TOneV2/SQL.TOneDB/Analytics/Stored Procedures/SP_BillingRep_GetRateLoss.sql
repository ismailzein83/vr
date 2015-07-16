


CREATE  PROCEDURE [Analytics].[SP_BillingRep_GetRateLoss](
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@ZoneID int = NULL,
	--@Margin FLOAT = 0,
	@CustomerAmuID int = NULL,
	@SupplierAmuID int = NULL 
)with Recompile
	AS 
	
	DECLARE @Margin FLOAT = 0;
	
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
		CZ.Name AS CostZone, 
		SZ.Name AS SaleZone, 
		bs.SupplierID  AS SupplierID, 
		bs.CustomerID AS CustomerID,
		AVG(bs.Sale_Rate / ISNULL(ERS.Rate, 1)) AS SaleRate,
		AVG(bs.Cost_Rate / ISNULL(ERC.Rate, 1)) AS CostRate,
		SUM(bs.CostDuration/60.0) AS CostDuration,
		SUM(bs.SaleDuration/60.0) AS SaleDuration, 
		SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet, 
		SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		Sz.ZoneID AS SaleZoneID         			
	 FROM
	     Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer),INDEX(IX_Billing_Stats_Supplier))
	    LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = bs.SaleZoneID
		LEFT JOIN Zone CZ WITH(NOLOCK) ON CZ.ZoneID = bs.CostZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE bs.calldate>=@FromDate AND bs.calldate<=@ToDate
	 	  AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
          AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)
          AND (@ZoneID IS Null OR SZ.ZoneID = @ZoneID)
		  AND (@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
		  AND (@SupplierAmuID IS NULL OR bs.SupplierID IN (SELECT * FROM @SupplierIDs))
	GROUP BY 
	    CZ.Name,SZ.Name,bs.SupplierID,bs.CustomerID,sz.ZoneID
	HAVING 	
        (SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) - SUM(bs.sale_nets / ISNULL(ERS.Rate, 1))) > @Margin
  
	  
	
RETURN