







CREATE PROCEDURE [dbo].[SP_TrafficStats_ByCustomer_new]
   @fromDate datetime,
   @ToDate datetime,
   @SupplierID Varchar(5) = null
AS
BEGIN
--SET @FromDate=     CAST(
--(
--	STR( YEAR( @FromDate ) ) + '-' +
--	STR( MONTH( @FromDate ) ) + '-' +
--	STR( DAY( @FromDate ) ) 
--)
--AS DATETIME
--)

--SET @ToDate= CAST(
--(
--	STR( YEAR( @ToDate ) ) + '-' +
--	STR( MONTH(@ToDate ) ) + '-' +
--	STR( DAY( @ToDate ) ) + ' 23:59:59.99'
--)
--AS DATETIME
--)	

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)




Declare @Traffic TABLE (CustomerID VarChar(15)  primary KEY, Attempts int, DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	
INSERT INTO @Traffic(CustomerID, Attempts, SuccessfulAttempts, DurationsInMinutes, ASR,ACD,DeliveredASR,AveragePDD)
SELECT 
	   ISNULL(TS.CustomerID, ''),
	   Sum(NumberofCalls) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   case when Sum(NumberofCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberofCalls) ELSE 0 end as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   case when Sum(NumberofCalls) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(Attempts) ELSE 0 end as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
      
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier,IX_TrafficStats_Supplier))
    WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate) --AND TS.CustomerID NOT IN (SELECT * FROM dbo.getRepresentedAsSwitchCarriers())
    AND ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
			AND (TS.SupplierID = @SupplierID)
			 		 		
	GROUP BY TS.CustomerID

DECLARE @Result TABLE (
		CustomerID VarChar(15)primary KEY,Attempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), SuccessfulAttempts numeric (13,5),ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),AverageSaleRate DECIMAL(13,5),AverageCostRate DECIMAL(13,5), Percentage numeric (13,5) , PricedDuration  DECIMAL(13,5)
		)
begin
	;With MyBilling as (Select
		  BS.CustomerID,
		  BS.SaleZoneID , 	     
		  Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      AVG(bs.Sale_Rate) AS SaleRate,
	      AVG(bs.Cost_Rate) as CostRate,
	      0	AS Percentage,
	      SUM(BS.SaleDuration)/60 AS PricedDuration,
	              bs.Cost_Currency,bs.Sale_Currency
from Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) 
				LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
				LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
Where BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate and BS.SupplierID=@SupplierID 
GROUP BY BS.SaleZoneID,bs.Cost_Currency,bs.Sale_Currency,BS.CustomerID
)
INSERT INTO @Result(CustomerID ,Attempts, DurationsInMinutes,SuccessfulAttempts, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit,AverageSaleRate,AverageCostRate, Percentage,PricedDuration)
	SELECT 
		  TS.CustomerID, TS.Attempts, TS.DurationsInMinutes,TS.SuccessfulAttempts,TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
				  BS.Calls,
				  BS.Cost,
				  BS.Sale,
				  BS.Profit,
				  BS.SaleRate,
				  BS.CostRate,
				  BS.Percentage,
				  BS.PricedDuration      
    FROM @Traffic TS 
		LEFT JOIN MyBilling BS ON TS.CustomerID= BS.CustomerID
	--GROUP BY TS.CustomerID, TS.Attempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,TS.SuccessfulAttempts
end 
Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = CASE WHEN @TotalProfit <> 0 THEN  (Profit * 100. / @TotalProfit) ELSE 0.0 END 
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage = CASE WHEN @TotalAttempts <> 0 THEN (Attempts * 100. / @TotalAttempts) ELSE 0.0 END 

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END