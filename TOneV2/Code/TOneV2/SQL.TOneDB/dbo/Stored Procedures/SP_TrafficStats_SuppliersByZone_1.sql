

CREATE PROCEDURE [dbo].[SP_TrafficStats_SuppliersByZone]
   @fromDate datetime,
   @ToDate datetime,
   @ZoneID INT 
AS
BEGIN

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
	
                 
SET NOCOUNT ON 	
;With 
Traffic AS
	(
		SELECT 
		   ISNULL(TS.SupplierID, '') SupplierID,
		   Sum(Attempts) as Attempts, 
		   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
	      
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
		WHERE (FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <= @ToDate)
				 AND ts.CustomerID NOT IN ( SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
				 and (ts.SupplierID is not null and ts.SupplierID not IN ( SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc))
				AND TS.OurZoneID = @ZoneID 		 		
		GROUP BY TS.SupplierID
	)	

,Result As 
	(
		SELECT 
			  TS.SupplierID, TS.Attempts, TS.DurationsInMinutes,TS.SuccessfulAttempts,TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
			  Sum(BS.NumberOfCalls) AS Calls,
			  SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
			  Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
			  SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
			  0	Percentage      
		FROM Traffic TS 
			LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON TS.SupplierID= BS.SupplierID 
			And (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate)  AND BS.SaleZoneID = @ZoneID
			LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
			LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		
		GROUP BY TS.SupplierID, TS.Attempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,TS.SuccessfulAttempts
	 )
,TotalCTE As 
	(
		SELECT  SUM(profit) TotalProfit, SUM(Attempts) TotalAttempts FROM Result	 
	)

SELECT	SupplierID ,
		Attempts , 
		CASE WHEN TotalCTE.TotalAttempts <>0 THEN  (Attempts * 100. / TotalCTE.TotalAttempts) ELSE 0.0 END AttemptPercentage , DurationsInMinutes , 
		SuccessfulAttempts ,
		ASR , 
		ACD , 
		DeliveredASR , 
		AveragePDD ,
		Calls NumberOfCalls , 
		Cost Cost_Nets , 
		Sale Sale_Nets ,
		Profit ,
		CASE WHEN TotalCTE.TotalProfit <>0 THEN (Profit * 100. / TotalCTE.TotalProfit) ELSE 0.0 END  Percentage  
FROM Result,TotalCTE 
ORDER BY DurationsInMinutes DESC 



END