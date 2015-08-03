
CREATE PROCEDURE [dbo].[SP_SuppliersByAMUsCustomers](
   @fromDate datetime,
   @ToDate datetime,
   @CustomerID Varchar(5) = null
   )
  
AS
BEGIN

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	DECLARE @Traffic TABLE (SupplierID VarChar(15), Attempts int, DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5))
	SET	NOCOUNT ON 	
	INSERT INTO @Traffic(SupplierID, Attempts, SuccessfulAttempts, DurationsInMinutes)
	SELECT 
	   ISNULL(TS.SupplierID, ''),
	   Sum(attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes     
    FROM TrafficStatsDaily TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
    WHERE (CallDate >= @FromDate AND CallDate <= @ToDate)
  --  AND ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
    AND SupplierID NOT IN (SELECT rasc.CarrierAccountID FROM CarrierAccount  rasc WHERE rasc.RepresentsASwitch='Y')
	AND TS.CustomerID = @CustomerID 	 		
	GROUP BY TS.SupplierID
	
	DECLARE @Result TABLE (
		SupplierID VarChar(15),Attempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5), Percentage numeric (13,5) , PricedDuration  DECIMAL(13,5)
		)
	BEGIN	
	;With MyBilling as (SELECT
		  BS.SupplierID, 	     
		  Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      AVG(bs.Sale_Rate) AS SaleRate,
	      AVG(bs.Cost_Rate) as CostRate,
	      0	AS Percentage,
	      SUM(BS.SaleDuration)/60 AS PricedDuration
	FROM Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) 
	LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
	LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate AND (BS.CustomerID=@CustomerID)
	AND bs.SupplierID NOT IN (SELECT rasc.CarrierAccountID FROM CarrierAccount  rasc WHERE rasc.RepresentsASwitch='Y')
	GROUP BY BS.SupplierID
	)
	
	INSERT INTO @Result(SupplierID ,Attempts, DurationsInMinutes,SuccessfulAttempts,NumberOfCalls, Cost_Nets, Sale_Nets, Profit,Percentage,PricedDuration)
	SELECT 
	  TS.SupplierID,isnull(TS.Attempts,0), isnull(TS.DurationsInMinutes,0),isnull(TS.SuccessfulAttempts,0),isnull(BS.Calls,0),
	  isnull(BS.Cost,0), isnull(BS.Sale,0),isnull(BS.Profit,0), isnull(BS.Percentage,0), isnull( BS.PricedDuration,0)       
    FROM @Traffic TS 
	LEFT JOIN MyBilling BS ON TS.SupplierID= BS.SupplierID 
	END 
	DECLARE @TotalProfit numeric(13,5)
	SELECT  @TotalProfit = SUM(profit) FROM @Result
	UPDATE  @Result SET Percentage =CASE WHEN @TotalProfit<>0 THEN (Profit * 100. / @TotalProfit) ELSE 0 END 
	DECLARE @TotalAttempts bigint
	SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
	UPDATE  @Result SET AttemptPercentage = CASE WHEN @TotalAttempts<>0 THEN (Attempts * 100. / @TotalAttempts) ELSE 0 END 
	SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END