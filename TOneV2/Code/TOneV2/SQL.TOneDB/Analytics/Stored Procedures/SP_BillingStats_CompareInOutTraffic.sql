

CREATE  PROCEDURE [Analytics].[SP_BillingStats_CompareInOutTraffic](
	@FromDate Datetime ,
	@ToDate Datetime ,
	@CustomerID varchar(5),
	@Period varchar(7)
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

   Declare @Results Table(CallDate datetime,CallWeek int,CallMonth int,CallYear int,TrafficDirection varchar(10),
						   Duration numeric(13,5), Net numeric(13,5), 
                           PercDuration varchar(100), PercNet varchar(100), 
                           TotalDuration numeric(13,5), TotalNet numeric(13,5))

	IF(@Period = 'None')
		BEGIN
			INSERT INTO @Results(
				TrafficDirection,
				Duration,
				Net,
				PercDuration,
				PercNet,
				TotalDuration,
				TotalNet
			)
			SELECT
				  'IN' AS TrafficDirection, 
				  SUM(BS.SaleDuration)/60.0 AS  Duration,
				  SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net,
				  0,0,0,0
			FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
			WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate AND BS.CustomerID=@CustomerID
			UNION ALL	
			SELECT 
				  'OUT' AS TrafficDirection, 
				  SUM(BS.CostDuration)/60.0 AS  Duration,
				  SUM(bs.Cost_Nets / ISNULL(ERS.Rate, 1)) AS Net,
				  0,0,0,0
			FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = BS.CallDate
			WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate AND BS.SupplierID=@CustomerID 		
		END
		ELSE
			IF(@Period = 'Daily')
			BEGIN
				INSERT INTO @Results(
					CallDate,
					TrafficDirection,
					Duration,
					Net,
					PercDuration,
					PercNet,
					TotalDuration,
					TotalNet
				)
				SELECT
					  bs.CallDate AS CallDate,
					  'IN' AS TrafficDirection, 
					  SUM(BS.SaleDuration)/60.0 AS  Duration,
					  SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net,
					  0,0,0,0
				FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
				LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
				WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate AND BS.CustomerID=@CustomerID
				GROUP BY bs.CallDate 
				UNION ALL	
				SELECT 
					  bs.CallDate AS CallDate,
					  'OUT' AS TrafficDirection, 
					  SUM(BS.CostDuration)/60.0 AS  Duration,
					  SUM(bs.Cost_Nets / ISNULL(ERS.Rate, 1)) AS Net,
					  0,0,0,0
				FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
				LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = BS.CallDate
				WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
 					AND BS.SupplierID=@CustomerID 	
				GROUP BY bs.CallDate 
			END
			ELSE
				IF(@Period = 'Weekly')
				BEGIN
						INSERT INTO @Results(
						CallWeek,
						CallYear,
						TrafficDirection,
						Duration,
						Net,
						PercDuration,
						PercNet,
						TotalDuration,
						TotalNet
					)
					SELECT
						  datepart(week,BS.CallDate) AS CallWeek,
						  datepart(year,bs.CallDate) AS CallYear,
						  'IN' AS TrafficDirection, 
						  SUM(BS.SaleDuration)/60.0 AS  Duration,
						  SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net,
						  0,0,0,0
					FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
					LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
					WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
						AND BS.CustomerID=@CustomerID
					GROUP BY DATEPART(year,BS.calldate),DATEPART(week,BS.calldate)
					UNION ALL	
					SELECT 
						  datepart(week,BS.CallDate) AS CallWeek,
						  datepart(year,bs.CallDate) AS CallYear,
						  'OUT' AS TrafficDirection, 
						  SUM(BS.CostDuration)/60.0 AS  Duration,
						  SUM(bs.Cost_Nets / ISNULL(ERS.Rate, 1)) AS Net,
						  0,0,0,0
					FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
					LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = BS.CallDate
					WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
 						AND BS.SupplierID=@CustomerID 	
 					GROUP BY DATEPART(year,BS.calldate),DATEPART(week,BS.calldate)
				END
				ELSE 
					IF(@Period = 'Monthly')
						BEGIN
								INSERT INTO @Results(
								CallMonth,
								CallYear,
								TrafficDirection,
								Duration,
								Net,
								PercDuration,
								PercNet,
								TotalDuration,
								TotalNet
							)
							SELECT
							      datepart(month,BS.CallDate) AS CallMonth,
								  datepart(year,bs.CallDate) AS CallYear,
								  'IN' AS TrafficDirection, 
								  SUM(BS.SaleDuration)/60.0 AS  Duration,
								  SUM(bs.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Net,
								  0,0,0,0
							FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer))
							LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
							WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
 								AND BS.CustomerID=@CustomerID
							GROUP BY DATEPART(year,BS.calldate),DATEPART(month,BS.calldate)
							UNION ALL	
							SELECT 
	    						  datepart(month,BS.CallDate) AS CallMonth,
								  datepart(year,bs.CallDate) AS CallYear,
								  'OUT' AS TrafficDirection, 
								  SUM(BS.CostDuration)/60.0 AS  Duration,
								  SUM(bs.Cost_Nets / ISNULL(ERS.Rate, 1)) AS Net,
								  0,0,0,0
							FROM Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Supplier))
							LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Cost_Currency AND ERS.Date = BS.CallDate
							WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
 								AND BS.SupplierID=@CustomerID 	
							GROUP BY DATEPART(year,BS.calldate),DATEPART(month,BS.calldate)
						END

   
-- Calculate Totals, then Percentages
Update @Results SET 
	TotalDuration = (SELECT Sum(Duration) FROM @Results),
	TotalNet = (SELECT Sum(Net) FROM @Results)

Update @Results SET 
	PercDuration = TrafficDirection + ' ' + cast(cast((Duration / TotalDuration * 100) as numeric(13,2)) as varchar(20)) + '%' ,
	PercNet = TrafficDirection + ' ' + cast(cast((Net / TotalNet * 100) as numeric(13,2)) as varchar(20)) + '%' 

SELECT * 
FROM @Results 
ORDER BY CallYear,CallMonth,CallWeek,CallDate ASC

RETURN