CREATE PROCEDURE [Analytics].[SP_Traffic_DailyProfitSummary](
	@FromDate Datetime ,
	@ToDate Datetime 
)
with Recompile
	AS 

	--SET @todate = dateadd(dd,1,@todate)

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
		    bs.calldate  [Day],
			--SUM(bs.NumberOfCalls) AS Calls,
			--SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],    
			--SUM(bs.SaleDuration / 60.0) [SaleDuration], 
			SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet,           			
			SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet,
			SUM((bs.sale_nets / ISNULL(ERC.Rate,1)) - (bs.cost_nets / ISNULL(ERC.Rate, 1))) AS Profit
		    FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
		    LEFT JOIN @ExchangeRates ERC ON ERC.Currency collate SQL_Latin1_General_CP1_CI_AS = bs.Cost_Currency AND ERC.Date = bs.CallDate			
            LEFT JOIN @ExchangeRates ERS ON ERS.Currency collate SQL_Latin1_General_CP1_CI_AS = bs.Sale_Currency AND ERS.Date = bs.CallDate
            WHERE bs.calldate =@fromdate --AND bs.calldate<@ToDate
			GROUP BY 
			    bs.calldate
			--ORDER BY CAST(bs.calldate AS varchar(11)) Asc	
			)
			
			
select [day],datename(dw, [day]) as [weekday], datepart(dw, [day]) as dayNumber, salenet ,costnet, profit from billing
order by [day],datepart(dw, [day])
	
	RETURN