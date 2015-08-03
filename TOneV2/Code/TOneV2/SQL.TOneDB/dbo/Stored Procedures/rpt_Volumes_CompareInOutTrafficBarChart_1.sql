






CREATE  PROCEDURE [dbo].[rpt_Volumes_CompareInOutTrafficBarChart](
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

   Declare @Results Table(CallDate datetime,CallWeek int,CallMonth int,CallYear int,DurationIn numeric(13,5), DurationOut numeric(13,5), NetIn numeric(13,5), NetOut numeric(13,5))

	IF(@Period = 'None')
		BEGIN
			INSERT INTO @Results(
				DurationIn,
				DurationOut,
				NetIn,
				NetOut
			)
			SELECT
				  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN (BS.SaleDuration / 60.0) ELSE 0 END) AS DurationIn,
				  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN (BS.CostDuration / 60.0) ELSE 0 END) AS DurationOut,
				  
				  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN bs.Sale_Nets / ISNULL(ERS.Rate, 1) ELSE 0 END) AS NetIn,
				  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN bs.Cost_Nets / ISNULL(ERC.Rate, 1) ELSE 0 END) AS NetOut
			FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
			WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
		END
		ELSE
			IF(@Period = 'Daily')
			BEGIN
				INSERT INTO @Results(
					CallDate,
					DurationIn,
					DurationOut,
					NetIn,
					NetOut
				)
				SELECT
				  bs.CallDate AS CallDate,
				  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN (BS.SaleDuration /60.0) ELSE 0 END) AS DurationIn,
				  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN (BS.CostDuration / 60.0) ELSE 0 END) AS DurationOut,
				  
				  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN bs.Sale_Nets / ISNULL(ERS.Rate, 1) ELSE 0 END) AS NetIn,
				  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN bs.Cost_Nets / ISNULL(ERC.Rate, 1) ELSE 0 END) AS NetOut
				FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
				LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
				LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
				WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
				GROUP BY bs.CallDate 
				ORDER BY bs.CallDate
			END
			ELSE
				IF(@Period = 'Weekly')
				BEGIN
						INSERT INTO @Results(
						CallWeek,
						CallYear,
						DurationIn,
						DurationOut,
						NetIn,
						NetOut
					)
					SELECT
						  datepart(week,BS.CallDate) AS CallWeek,
						  datepart(year,bs.CallDate) AS CallYear,
						  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN (BS.SaleDuration /60.0) ELSE 0 END) AS DurationIn,
						  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN (BS.CostDuration / 60.0) ELSE 0 END) AS DurationOut,
						  
						  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN bs.Sale_Nets / ISNULL(ERS.Rate, 1) ELSE 0 END) AS NetIn,
						  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN bs.Cost_Nets / ISNULL(ERC.Rate, 1) ELSE 0 END) AS NetOut
					FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
					LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
					LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
					WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
					GROUP BY DATEPART(week,BS.calldate),DATEPART(year,BS.calldate)
					ORDER BY DATEPART(year,BS.calldate) ,DATEPART(week,BS.calldate) ASC
				END
				ELSE 
					IF(@Period = 'Monthly')
						BEGIN
								INSERT INTO @Results(
								CallMonth,
								CallYear,
								DurationIn,
								DurationOut,
								NetIn,
								NetOut
							)
							SELECT
							      datepart(month,BS.CallDate) AS CallMonth,
								  datepart(year,bs.CallDate) AS CallYear,
								  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN (BS.SaleDuration / 60.0) ELSE 0 END) AS DurationIn,
								  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN (BS.CostDuration / 60.0) ELSE 0 END) AS DurationOut,
								  
								  SUM(CASE WHEN BS.CustomerID=@CustomerID THEN bs.Sale_Nets / ISNULL(ERS.Rate, 1) ELSE 0 END) AS NetIn,
								  SUM(CASE WHEN BS.SupplierID=@CustomerID THEN bs.Cost_Nets / ISNULL(ERC.Rate, 1) ELSE 0 END) AS NetOut
							FROM Billing_Stats BS  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier))
							LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
							LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate
							WHERE BS.CallDate>=@FromDate  AND BS.CallDate< @ToDate
							GROUP BY DATEPART(month,BS.calldate),DATEPART(year,BS.calldate)
							ORDER BY DATEPART(year,BS.calldate),DATEPART(month,BS.calldate) ASC 
						END

SELECT * FROM @Results

RETURN