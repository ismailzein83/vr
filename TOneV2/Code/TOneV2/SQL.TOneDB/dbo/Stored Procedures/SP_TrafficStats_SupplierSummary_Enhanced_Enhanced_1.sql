



CREATE PROCEDURE [dbo].[SP_TrafficStats_SupplierSummary_Enhanced_Enhanced]
   @fromDate datetime ,
   @ToDate datetime,
   @SupplierID varchar(15)=null,
   @TopRecord INT =NULL
AS
BEGIN

 
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)
;WITH Traffic_ AS
	(
		
		SELECT
			SupplierID,
			FirstCDRAttempt,
			Attempts,
			DeliveredAttempts,
			SuccessfulAttempts,
			DurationsInSeconds,
			PDDInSeconds,
			NumberOfCalls
			
		FROM
			TrafficStats ts  WITH(NOLOCK)
        WHERE ( FirstCDRAttempt >= @fromDate AND FirstCDRAttempt <=  @ToDate )
	 AND (@SupplierID IS NULL OR ts.SupplierID= @SupplierID)
	    AND CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		) 
, Traffic AS 
( 
		 SELECT 
   ISNULL(TS.SupplierID, '') AS SupplierID, 
		   Sum(Attempts) as Attempts, 
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM Traffic_ TS WITH(NOLOCK)
	   
	 GROUP BY ISNULL(TS.SupplierID, '')  
), 
Billing AS 
(
	SELECT
	           BS.SupplierID AS SupplierID,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0 AS PercentageProfit
	FROM
		 Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date)) 
	     LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
         LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate)
	AND (@SupplierID IS NULL OR BS.SupplierID =  @SupplierID)
	 AND CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
	GROUP BY BS.SupplierID
	)
,
 Results AS 
(
	SELECT  T.SupplierID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
	       B.Calls AS NumberOfCalls,ISNULL( B.Sale,0) AS Sale_Nets,ISNULL( B.Cost,0) AS Cost_Nets,ISNULL( B.Profit,0) AS Profit,0 AS Percentage
	       ,  ROW_NUMBER() OVER (ORDER BY DurationsInMinutes DESC) AS rownIndex
	         
	FROM Traffic T WITH(NOLOCK)
	LEFT JOIN Billing B ON T.SupplierID= B.SupplierID
	
	)


SELECT * FROM Results 
WHERE rownIndex <= @TopRecord


END