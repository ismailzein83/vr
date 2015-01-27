


CREATE PROCEDURE [dbo].[EA_Billing_Trafficmonitor]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@SaleZoneID INT = NULL,
	@AllAccounts varchar(MAX) = NULL

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



	SELECT 
		bs.CallDate AS CallDate,
		bs.SaleZoneID AS SaleZone, 
		bs.CostZoneID AS CostZone, 
		bs.CustomerID AS CustomerID,
		bs.SupplierID  AS SupplierID,
		bs.Sale_Rate  / ISNULL(ERS.Rate, 1) AS SaleRate,
		bs.Cost_Rate / ISNULL(ERC.Rate, 1) AS CostRate,
		SUM(bs.SaleDuration/60.0) AS SaleDuration,
		SUM(bs.CostDuration/60.0) AS CostDuration,
		SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet         			
    FROM
	    Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer),INDEX(IX_Billing_Stats_Supplier))
	    LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = bs.SaleZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE        
			bs.calldate>=@FromDate AND bs.calldate<@ToDate
	 	--AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
	 	AND  bs.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) 
        AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)

        AND (@SaleZoneID IS Null OR SZ.ZoneID = @SaleZoneID)
        AND bs.Sale_Rate <> 0
	 	AND bs.Cost_Rate <> 0
	GROUP BY 
	     bs.CallDate
	    ,bs.CustomerID
	    ,bs.SupplierID
	    ,bs.SaleZoneID
	    ,bs.CostZoneID
	  	,bs.Sale_Rate  / ISNULL(ERS.Rate, 1) 
		,bs.Cost_Rate / ISNULL(ERC.Rate, 1)
	ORDER BY 
	     bs.CallDate
	    ,bs.CustomerID
	    ,bs.SupplierID
	    ,bs.SaleZoneID
	    ,bs.CostZoneID
		,bs.Sale_Rate  / ISNULL(ERS.Rate, 1) 
		,bs.Cost_Rate / ISNULL(ERC.Rate, 1)
	

RETURN