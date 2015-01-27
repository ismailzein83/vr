



CREATE  PROCEDURE dbo.rpt_ZoneSummary_Old(
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,
	@Cost char(1)='Y' 
)

	AS 

	SET @FromDate=     CAST(
     (
     STR( YEAR( @FromDate ) ) + '-' +
     STR( MONTH( @FromDate ) ) + '-' +
     STR( DAY( @FromDate ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDate= CAST(
     (
     STR( YEAR( @ToDate ) ) + '-' +
     STR( MONTH(@ToDate ) ) + '-' +
     STR( DAY( @ToDate ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	

	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	IF (@Cost = 'Y')
	BEGIN 
			SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
			    SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END) AS DurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS Rate,
				AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS Rate,
				--(SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 0 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS Net,
				SUM(CASE WHEN bs.Cost_RateType = 0 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  Net,
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
				--AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE NULL END) AS OffPeakRate,
				AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END) AS OffPeakRate,
				--(SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.CostDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Cost_RateType = 1 THEN (bs.Cost_Rate/ISNULL(ERC.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
				SUM(CASE WHEN bs.Cost_RateType = 1 THEN (bs.cost_nets /ISNULL(ERC.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
				SUM(bs.Cost_Discounts) AS Discount,
				SUM(bs.Cost_Commissions) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(IX_Zone_Name)) ON Z.ZoneID=bs.CostZoneID   
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			WHERE bs.calldate >=@FromDate AND bs.calldate<@ToDate
			     AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
	             AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)		
			GROUP BY 
			    Z.Name
			ORDER BY Z.Name ASC 
				
	END 
	ELSE 
	BEGIN 
		     SELECT 
				Z.Name AS Zone,
				SUM(bs.NumberOfCalls) AS Calls,
			    SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],
				SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS DurationInSeconds,
				--AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE NULL END) AS Rate,
				AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END) AS Rate,
				--(SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 0 THEN (bs.Sale_Rate/ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS Net,
				SUM(CASE WHEN bs.Sale_RateType = 0 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  Net,
				SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END) AS OffPeakDurationInSeconds,
				--AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE NULL END) AS OffPeakRate,
				AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS OffPeakRate,
				--(SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.SaleDuration/60.0) ELSE 0 END))*(AVG(CASE WHEN bs.Sale_RateType = 1 THEN (bs.Sale_Rate /ISNULL(ERS.Rate, 1)) ELSE 0 END)) AS OffPeakNet,
				SUM(CASE WHEN bs.Sale_RateType = 1 THEN (bs.sale_nets /ISNULL(ERS.Rate, 1)) ELSE 0 END) AS  OffPeakNet,
				SUM(bs.sale_Discounts) AS Discount,
				SUM(bs.sale_Commissions) AS CommissionValue,
				SUM(bs.sale_ExtraCharges) AS ExtraChargeValue
			FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN Zone Z WITH(NOLOCK INDEX(IX_Zone_Name)) ON Z.ZoneID=bs.SaleZoneID
		    LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
			WHERE bs.calldate>=@FromDate AND bs.calldate<@ToDate
			     AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
	             AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)		
			GROUP BY 
			    Z.Name
			ORDER BY Z.Name ASC
    END 
	RETURN