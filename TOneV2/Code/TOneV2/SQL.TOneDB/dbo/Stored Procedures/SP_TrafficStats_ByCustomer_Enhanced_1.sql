CREATE PROCEDURE [dbo].[SP_TrafficStats_ByCustomer_Enhanced]
   @fromDate datetime,
   @ToDate datetime,
   @SupplierID Varchar(5) = null
AS
BEGIN
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

;WITH
	ExchangeRates AS
	(
		SELECT * FROM dbo.GetDailyExchangeRates(@fromDate, @ToDate)
	),
	traffic_Stats_Data AS(
		SELECT 
		ISNULL(TS.CustomerID, '') AS CustomerID,
		SupplierID AS SupplierID,
		NumberOfCalls AS attempts,
		SuccessfulAttempts AS SuccessfulAttempts,
		DurationsInSeconds as DurationsInSeconds,
		PDDinSeconds as PDDinSeconds,
		deliveredAttempts AS deliveredAttempts,
		firstcdrattempt AS FirstCDRAttempt
		FROM TrafficStats ts WITH(NOLOCK)
		WHERE (FirstCDRAttempt >= @fromDate AND FirstCDRAttempt <= @ToDate) 
		    AND ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
			AND (TS.SupplierID = @SupplierID)
					)
	,traffic_Stats AS
	(
		SELECT 
		 TS.CustomerID,
	   Sum(attempts) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   case when Sum(attempts) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(attempts) ELSE 0 end as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   case when Sum(attempts) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(Attempts) ELSE 0 end as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
		FROM traffic_Stats_Data ts WITH(NOLOCK)

			 		 		
	GROUP BY TS.CustomerID
	)
	--SELECT * FROM traffic_stats
	,Billing_Data AS
	(
		SELECT 
		BS.CustomerID AS CustomerID,
	      BS.NumberOfCalls AS NumberOfCalls,
	      BS.Cost_Nets  AS Cost_Nets,
	      BS.Sale_Nets  AS Sale_Nets,
	     
	      BS.Sale_Rate AS Sale_Rate,
	      BS.Cost_Rate AS Cost_Rate,
	      ERC.Rate AS CRate,
	      ERS.Rate AS SRate,
	      BS.CallDate AS CallDate
		FROM Billing_Stats bs WITH(NOLOCK)
				LEFT JOIN ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
				    WHERE (CallDate >= @fromDate AND CallDate <=@ToDate)
    AND bs.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
			AND (bs.SupplierID = @SupplierID)
	
	
	)
	,Billing AS
	(
		SELECT 
		BS.CustomerID AS CustomerID,
        Sum(BS.NumberOfCalls) AS NumberOfCalls,
        SUM(BS.Cost_Nets / ISNULL(BS.CRate, 1)) AS Cost_Nets,
        Sum(BS.Sale_Nets / ISNULL(BS.SRate, 1)) AS Sale_Nets,
        SUM(BS.Sale_Nets / ISNULL(BS.SRate, 1)) - SUM(BS.Cost_Nets / ISNULL(BS.CRate, 1)) AS Profit,
	    AVG(BS.Sale_Rate) AS AverageSaleRate,
	    AVG(BS.Cost_Rate) AS AverageCostRate,
	    0 AS TotalProfit,
	    0 AS TotalAttempts

	    
		FROM Billing_Data BS WITH(NOLOCK)
	GROUP BY BS.CustomerID
	)
	,final_result AS
	(
		SELECT
		TS.CustomerID AS CustomerID, 
		TS.Attempts AS Attempts, 
		TS.DurationsInMinutes AS DurationsInMinutes,
		TS.SuccessfulAttempts AS SuccessfulAttempts,
		TS.ASR AS ASR, 
		TS.ACD AS ACD, 
		TS.DeliveredASR AS DeliveredASR, 
		TS.AveragePDD AS AveragePDD,
	    BS.NumberOfCalls AS NumberOfCalls,  
	    BS.Cost_Nets  AS Cost_Nets, 
	    BS.Sale_Nets  AS Sale_Nets, 
	    BS.Profit  AS Profit, 
		BS.AverageCostRate AS AverageCostRate,
	    BS.AverageSaleRate AS AverageSaleRate
	    --BS.TotalProfit=SUM(BS.profit),
		--RS.TotalAttempts=SUM(TS.Attempts),
		--CASE WHEN RS.TotalProfit <> 0 THEN  (BS.Profit * 100. / BS.TotalProfit) ELSE 0.0 END  AS Percentage, 
		--CASE WHEN RS.TotalAttempts <> 0 THEN (TS.Attempts * 100. / BS.TotalAttempts) ELSE 0.0 END  AS AttemptPercentage
		FROM traffic_stats TS WITH(NOLOCK)
		LEFT JOIN Billing BS WITH (NOLOCK) ON TS.CustomerID= BS.CustomerID
	)
   
SELECT * FROM final_result

END