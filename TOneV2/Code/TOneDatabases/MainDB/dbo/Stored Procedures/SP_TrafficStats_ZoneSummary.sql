

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_TrafficStats_ZoneSummary]
	@fromDate DATETIME ,
	@ToDate DATETIME,
	@ZoneID VARCHAR(15) = NULL,
	@TopRecord INT = NULL
AS
BEGIN

;WITH
	 
Traffic_Stats_Data AS (
		SELECT 
		        
		        ts.CustomerID AS CustomerID
		       , ts.OurZoneID AS OurZoneID
		       , ts.SupplierID AS SupplierID
		       , ts.SupplierZoneID AS SupplierZoneID
		       , ts.Attempts AS Attempts
		       , ts.DeliveredAttempts AS DeliveredAttempts 
		       , ts.DeliveredNumberOfCalls AS DeliveredNumberOfCalls
		       , ts.SuccessfulAttempts AS SuccessfulAttempts
		       , ts.DurationsInSeconds AS DurationsInSeconds
		       , ts.PDDInSeconds AS PDDInSeconds
		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
		       , ts.NumberOfCalls AS NumberOfCalls
		       , ts.LastCDRAttempt AS LastCDRAttempt 
		 FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))  WHERE 
		
	  FirstCDRAttempt >= @FromDate
	       AND FirstCDRAttempt < @ToDate
	       AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
	 )
	
	,Traffic_Stats AS 
	
	(
		SELECT 
		        
		        ts.OurZoneID AS OurZoneID
		       , Sum(ts.Attempts) AS Attempts
		       , Sum(ts.DeliveredAttempts) AS DeliveredAttempts 
		       , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
		       , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
		       , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
		       , AVG(ts.PDDInSeconds) AS PDDInSeconds
		       , Max(ts.MaxDurationInSeconds) AS MaxDurationInSeconds
		       , Sum(ts.NumberOfCalls) AS NumberOfCalls
		       , Max(ts.LastCDRAttempt) AS LastCDRAttempt 
		 FROM Traffic_Stats_Data ts WITH(NOLOCK)

		GROUP BY  ts.OurZoneID 
	      
	)
	
	, Traffic AS (
		
		SELECT 
		
		   isnull(TS.OurZoneID,0) AS ZoneID,
	       SUM(Attempts) AS Attempts,
	       SUM(DurationsInSeconds / 60.) AS DurationsInMinutes,
	       SUM(SuccessfulAttempts) * 100.0 / SUM(Attempts) AS ASR,
	       CASE 
	            WHEN SUM(SuccessfulAttempts) > 0 THEN SUM(DurationsInSeconds) / (60.0 * SUM(SuccessfulAttempts))
	            ELSE 0
	       END AS ACD,
	       SUM(DeliveredAttempts) * 100.0 / SUM(Attempts) AS DeliveredASR,
	       AVG(PDDinSeconds) AS AveragePDD
	 FROM   Traffic_Stats TS 
		       
		WHERE (@ZoneID IS NULL OR TS.OurZoneID = @ZoneID )
	       GROUP BY
	       isnull(TS.OurZoneID,0)
	    
),

 Billing AS
	  (
	  	Select
	  	  BS.CallDate  CallDate,
	  	  
	  	  BS.Cost_Currency  Cost_Currency,
	  	  BS.SaleZoneID  SaleZoneID,
	  	  BS.Sale_Currency  Sale_Currency,
	  	  BS.NumberOfCalls NumberOfCalls,
	  	  BS.Cost_Nets Cost_Nets,
	  	  BS.Sale_Nets Sale_Nets
		FROM  Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date)) 
	  	WHERE 
	            
	    BS.CallDate >= @fromDate AND BS.CallDate < @ToDate
	  ),
	  
  Billing_Currency AS (
  	SELECT 
      BS.NumberOfCalls NumberOfCalls,
      BS.Cost_Nets Cost_Nets,
      BS.Sale_Nets  Sale_Nets,
      BS.SaleZoneID SaleZoneID,
      CS.LastRate CSLastRate,
      CC.LastRate CCLastRate
  	FROM Billing BS WITH (NOLOCK)
       LEFT JOIN Currency CS WITH (NOLOCK)
            ON  CS.CurrencyID = BS.sale_currency
       LEFT JOIN Currency CC WITH (NOLOCK)
            ON  CC.CurrencyID = BS.cost_currency
    
  	
  	)
	  
	  
	
,Result AS (
	
	SELECT T.ZoneID AS ZoneID,
	       T.Attempts AS Attempts,
	       T.DurationsInMinutes AS DurationsInMinutes,
	       T.ASR AS ASR,
	       T.ACD AS ACD,
	       T.DeliveredASR AS DeliveredASR,
	       T.AveragePDD AS AveragePDD,
	       ISNULL(SUM(BS.NumberOfCalls), 0) AS NumberOfCalls,
	       ISNULL(SUM(BS.Cost_Nets / BS.CCLastRate), 0) AS Cost_Nets,
	        ISNULL(SUM(BS.Sale_Nets / BS.CSLastRate), 0) AS Sale_Nets,
	       ISNULL(SUM(BS.Sale_Nets / BS.CSLastRate), 0) -ISNULL(SUM(BS.Cost_Nets / BS.CCLastRate), 0) AS 
	       Profit
	       --ISNULL(Profit * 100.0 / SUM(Result.Profit) AS Percentage
	         ----------------------------------------------------------
	       ----------------------------------------------------------
	       -----------------------NOTE-------------------------------
	       -- Percentage should be calculated for each row in C# Code
	    
	    
	FROM   Traffic T
	       LEFT JOIN Billing_Currency BS WITH (NOLOCK)
	            ON  T.ZoneID = BS.SaleZoneID
	GROUP BY
	       T.ZoneID,
	       T.Attempts,
	       T.DurationsInMinutes,
	       T.ASR,
	       T.ACD,
	       T.DeliveredASR,
	       T.AveragePDD 
)
	
	      	
	SELECT *
	FROM   Result
	ORDER BY
	       DurationsInMinutes DESC
	                         
 END