CREATE PROCEDURE [dbo].[SP_TrafficStats_BySupplierSaleZone]
   @fromDate datetime,
   @ToDate DATETIME--,
   --@CustomerID Varchar(5) = null
AS

	DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

;WITH TrafikBase AS (
	
	SELECT 
	   TS.CustomerID AS CustomerID,
	   TS.SupplierID AS SupplierID,
	   TS.OurZoneID AS OurZoneID,
	   TS.NumberOfCalls as NumberOfCalls, 
	   TS.SuccessfulAttempts AS SuccessfulAttempts,
	   TS.DurationsInSeconds as DurationsInSeconds,
	   TS.Attempts ,
	   TS.deliveredAttempts as deliveredAttempts, 
	   TS.PDDinSeconds as PDDinSeconds 
      
    FROM TrafficStatsDaily TS WITH(NOLOCK)
    WHERE (Calldate >= @FromDate AND Calldate < @ToDate)
  
	
),result AS(

SELECT 
	   ISNULL(TS.SupplierID, '') AS SupplierID,
	   ISNULL(TS.OurZoneID, '') AS OurZoneID,
	   Sum(NumberOfCalls) as Attempts, 
	   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
      
    FROM TrafikBase TS WITH(NOLOCK)
WHERE  (ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc))
			--and  ( TS.CustomerID = @CustomerID OR  @CustomerID is NULL OR  @CustomerID ='SYS')		 		
	GROUP BY TS.SupplierID,TS.OurZoneID
)
SELECT * from result