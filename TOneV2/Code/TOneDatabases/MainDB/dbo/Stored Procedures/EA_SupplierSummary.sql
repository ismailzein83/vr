

CREATE PROCEDURE [dbo].[EA_SupplierSummary](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@SupplierID varchar(5)=NULL
	)
with Recompile
	AS 
	
	SET @FromDate = dbo.DateOf(@FromDate)
	SET @ToDate = dbo.DateOf(@ToDate)
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

    DECLARE @SupplierZones TABLE (
    ZoneID INT,
    ZoneName nvarchar(255),
    PRIMARY KEY(ZoneID)
    )
    
    INSERT INTO @SupplierZones SELECT z.ZoneID,z.[Name]
                             FROM Zone z WITH(NOLOCK) WHERE z.SupplierID=@SupplierID 
                             OR (@FromDate>z.BeginEffectiveDate AND @ToDate<z.EndEffectiveDate)
                             
	SELECT 
		--bs.SaleZoneID AS SaleZoneID,
		SZO.ZoneName AS SaleZoneID,
		bs.Cost_Rate AS SaleRate,
		bs.Cost_Currency AS SaleCurrency,
		
		SUM(bs.Cost_Nets / ISNULL(ERS.Rate, 1)) AS SaleNet,
		SUM(bs.NumberOfCalls) AS NumberOfCalls, 
		SUM(bs.CostDuration)/60.0 AS DurationsInMinutes
		--,SUM(bs.Sale_Rate * bs.SaleDuration) AS TotalRate
	FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date, IX_Billing_Stats_Customer))
		LEFT  JOIN @SupplierZones SZO ON SZO.ZoneID=bs.CostZoneID
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	
	WHERE  bs.calldate BETWEEN @FromDate AND @ToDate
		AND bs.SupplierID =@SupplierID
	GROUP BY 
		bs.SaleZoneID,
		bs.Cost_Rate,
		bs.Cost_Currency,
		SZO.ZoneName

  RETURN