
create PROCEDURE [dbo].[rpt_DetailedCarrierSummary](
	@FromDate DATETIME,
	@ToDate DATETIME,
	@CustomerID VARCHAR(5) = NULL,
	@SupplierID VARCHAR(10) = NULL,
	@CustomerAmuID INT = NULL,
	@SupplierAmuID INT = NULL
)
	
AS
BEGIN
	SET NOCOUNT ON;
	
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
	
	;WITH Billing AS (
		SELECT bs.CallDate
				, bs.CustomerID
				, bs.SupplierID
				, bs.CostZoneID
				, bs.SaleZoneID
				, bs.Cost_Currency
				, bs.Sale_Currency
				, bs.Cost_Nets / ISNULL(ERC.Rate, 1) AS Cost_Nets
				, bs.Sale_Nets / ISNULL(ERS.Rate, 1) AS Sale_Nets
				, bs.Sale_Rate / ISNULL(ERS.Rate, 1) AS Sale_Rate
				, bs.Cost_Rate / ISNULL(ERC.Rate, 1) AS Cost_Rate
				, bs.SaleDuration
				, bs.CostDuration
		FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		WHERE bs.CallDate BETWEEN @FromDate AND @ToDate
			AND (@CustomerAmuID IS NULL OR bs.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND (@CustomerID IS NULL OR bs.CustomerID = @CustomerID)
			AND (@SupplierID IS NULL OR bs.SupplierID = @SupplierID)
	)

	SELECT bs.CustomerID AS CustomerID
		   , sz.ZoneID AS SaleZoneID
		   , sz.Name AS SaleZoneName
		   , SUM(bs.SaleDuration)/60. AS SaleDuration
		   , bs.Sale_Rate as SaleRate
		   , sr.change as SaleRateChange
		   , sr.BeginEffectiveDate as SaleRateEffectiveDate
		   , SUM(bs.sale_nets/ISNULL(ERS.Rate, 1)) SaleAmount
		   , bs.SupplierID 
		   , cz.ZoneID
		   , cz.Name AS CostZoneName
		   , SUM(bs.CostDuration)/60. as CostDuration
		   , bs.Cost_Rate AS CostRate
		   , cr.change as CostRateChange
		   , cr.begineffectivedate as CostRateEffectiveDate
		   , SUM(bs.cost_nets/ISNULL(ERC.Rate, 1)) AS CostAmount
		   , SUM(bs.sale_nets/ISNULL(ERS.Rate, 1) - bs.cost_nets/ISNULL(ERC.Rate, 1)) as profit
	FROM Billing bs
		JOIN Zone sz ON bs.salezoneid = sz.zoneid
		JOIN Zone cz ON bs.costzoneid = cz.zoneid
		JOIN Rate sr WITH(NOLOCK, INDEX(IX_Rate_Zone)) ON sr.zoneid = sz.zoneid
		JOIN Pricelist sp ON sr.pricelistid = sp.pricelistid
		JOIN Rate cr WITH(NOLOCK, INDEX(IX_Rate_Zone)) ON cr.zoneid = cz.zoneid
		JOIN Pricelist cp ON cr.pricelistid = cp.pricelistid
		JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE calldate BETWEEN @FromDate AND @ToDate
		AND sr.begineffectivedate < bs.calldate AND (sr.endeffectivedate IS NULL OR sr.endeffectivedate > bs.calldate)
		AND bs.customerid = sp.customerid
		AND cr.begineffectivedate < bs.calldate AND (cr.endeffectivedate IS NULL OR cr.endeffectivedate > bs.calldate)
		AND bs.supplierid = cp.supplierid
	GROUP BY bs.CustomerID
			, sz.ZoneID
			, sz.Name   
			, bs.Sale_Rate
			, sr.change
			, sr.BeginEffectiveDate	   
			, bs.SupplierID
			, cz.ZoneID
			, cz.Name	   
			, bs.Cost_Rate
			, cr.change
			, cr.begineffectivedate

END