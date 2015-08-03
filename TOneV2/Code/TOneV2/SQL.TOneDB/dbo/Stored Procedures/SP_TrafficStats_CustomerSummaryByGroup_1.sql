



CREATE PROCEDURE [dbo].[SP_TrafficStats_CustomerSummaryByGroup]
   @fromDate datetime ,
   @ToDate datetime,
   @CustomerID varchar(15)=null

AS
BEGIN
	
 
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	DECLARE @RepresentedAsSwitchCarriers TABLE (
CID VARCHAR(5)
)
INSERT INTO  @RepresentedAsSwitchCarriers SELECT ca.CarrierAccountID FROM CarrierAccount ca WITH (NOLOCK)
                 WHERE ca.RepresentsASwitch='Y'

;WITH Traffic_ AS
	(
		
		SELECT
			CustomerID,
			FirstCDRAttempt,
			Attempts,
			DeliveredAttempts,
			DeliveredNumberOfCalls,
			SuccessfulAttempts,
			DurationsInSeconds,
			PDDInSeconds,
			NumberOfCalls
			
		FROM
			TrafficStats   WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
        WHERE ( FirstCDRAttempt >= @fromDate AND FirstCDRAttempt <  @ToDate )
        AND customerid NOT IN (SELECT rasc.CID
                                    FROM @RepresentedAsSwitchCarriers rasc)
        AND SupplierID NOT IN (SELECT rasc.CID
                                    FROM @RepresentedAsSwitchCarriers rasc)
		) 
, Traffic AS 
( 
		 SELECT 
		   ISNULL(TS.CustomerID, '') AS CustomerID, 
		   Sum(NumberofCalls) as Attempts, 
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   case when Sum(NumberofCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberofCalls) ELSE 0 end as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   case when Sum(NumberofCalls) > 0 then Sum(DeliveredNumberofCalls) * 100.0 / SUM(NumberofCalls) ELSE 0 end as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM Traffic_ TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
	   where  (@CustomerID IS NULL OR ts.CustomerId = @CustomerID)
	   	   
	 GROUP BY ISNULL(TS.CustomerID, '')  
), 
Billing AS 
(
	SELECT
	           BS.CustomerID AS CustomerID,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0 AS PercentageProfit
	FROM
		 Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date)) 
	     LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
         LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  (BS.CallDate >= @fromDate AND BS.CallDate < @ToDate)
	AND (@CustomerID IS NULL OR BS.CustomerID =  @CustomerID)
	GROUP BY BS.CustomerID
	)
, Results AS 
(
	SELECT  T.CustomerID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
	        B.Calls AS NumberOfCalls, isnull(B.Sale,0) AS Sale_Nets,isnull(B.Cost,0) AS Cost_Nets,isnull(B.Profit,0) AS Profit,0 AS Percentage
	         ,  ROW_NUMBER() OVER (ORDER BY DurationsInMinutes DESC) AS rownIndex
	FROM Traffic T WITH(NOLOCK)
	LEFT JOIN Billing B ON T.CustomerID = B.CustomerID	
	)
, FinalResult as 
(
	select G.CarrierGroupName as CarrierGroup,
		   SUM(T.Attempts) as Attempts
	from Results T Left outer join CarrierAccount A on t.CustomerID = a.CarrierAccountID
		 left outer join CarrierGroup G on a.CarrierGroupID = g.CarrierGroupID
	where T.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)  
	group by G.CarrierGroupName
)


SELECT * FROM FinalResult-- WHERE  
--rownIndex <= @TopRecord
--and CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)  

END