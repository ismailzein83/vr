create PROCEDURE [dbo].[rpt_DailySummaryDashboard](
	@FromDate Datetime ,
	@ToDate Datetime 
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
;with billing as 
(
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
			GROUP BY 
			    CAST(bs.calldate AS varchar(11))   
			--ORDER BY CAST(bs.calldate AS varchar(11)) Asc	
			)
			
			
select [day],datename(dw, [day]) as [weekday], datepart(dw, [day]), calls, durationnet, saleduration, salenet ,costnet from billing
order by [day],datepart(dw, [day])
	
	RETURN