
CREATE PROCEDURE [dbo].[sp_AMUCustomerSummary]
(

    @fromDate datetime ,
    @ToDate datetime,
    @AMUID int 
   )
AS
BEGIN


DECLARE	 @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	DECLARE @RepresentedAsSwitchCarriers TABLE (CID VARCHAR(5))
INSERT INTO  @RepresentedAsSwitchCarriers SELECT ca.CarrierAccountID FROM CarrierAccount ca WITH (NOLOCK) WHERE ca.RepresentsASwitch='Y'
	
if @AMUID = 0 
begin
;WITH 
amuids as 
(
	select distinct amuid,carrieraccountid from amu_carrier amu 
)
,
Customerids as 
(
	select distinct c.carrieraccountid,p.Name,c.NameSuffix from carrieraccount c inner join carrierprofile p on c.profileID = p.[ProfileID] 
)
,Traffic_ AS
(
	SELECT
		CustomerID,
		Attempts,
		DeliveredNumberOfCalls,
		SuccessfulAttempts,
		DurationsInSeconds,
		NumberOfCalls
		
	FROM
		TrafficStatsDaily ts   WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
    WHERE (Calldate >= @fromDate AND Calldate <  @ToDate )
		AND CustomerID NOT IN (SELECT rasc.CID FROM @RepresentedAsSwitchCarriers rasc)
        AND SupplierID NOT IN (SELECT rasc.CID FROM @RepresentedAsSwitchCarriers rasc)
     
) 
, Traffic AS 
( 
	SELECT 
	   ISNULL(TS.CustomerID, '') AS CustomerID, 
	   CID.Name as CustomerName,
	   CID.NameSuffix as Suffix, 
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) as SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes
	FROM Traffic_ TS WITH(NOLOCK, INDEX(IX_TrafficStatsDaily_DateTimeFirst))
	left JOIN Customerids CID ON CID.carrieraccountid = TS.CustomerID 
	WHERE 
	  customerid not in ( select CarrierAccountID from amuids)  
	GROUP by TS.CustomerID, CID.Name,CID.NameSuffix
)

, 
Billing AS 
(
	SELECT
      BS.CustomerID AS CustomerID,
      CID.Name customername,
	  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
	  ISNULL(SUM(BS.SaleDuration)/60,0) AS PricedDuration,
	  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
	  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
	  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
	  0 AS PercentageProfit
	FROM
		 Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date)) 
	     LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
         LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
         left JOIN Customerids CID ON CID.carrieraccountid = BS.CustomerID 
	WHERE  (BS.CallDate >= @fromDate AND BS.CallDate < @ToDate)
	and   customerid not in ( select carrieraccountid from amuids)  
	GROUP BY CID.Name, BS.CustomerID
	)
, Results AS 
(
	SELECT  T.CustomerName as CustomerName, T.CustomerID as CarrierAccountID,T.Suffix as Suffix, T.Attempts, T.SuccessfulAttempts, T.DurationsInMinutes,
	        B.Calls AS NumberOfCalls,B.PricedDuration AS PricedDuration, isnull(B.Sale,0) AS Sale_Nets,isnull(B.Cost,0) AS Cost_Nets,isnull(B.Profit,0) AS Profit,0 AS Percentage
	         ,  ROW_NUMBER() OVER (ORDER BY DurationsInMinutes DESC) AS rownIndex
	FROM Traffic T WITH(NOLOCK)
	LEFT JOIN Billing B ON T.CustomerID = B.CustomerID
)
SELECT * FROM Results 
END


ELSE

IF @AMUID != 0 
BEGIN
	;WITH
	
amuids as 
(
	select distinct c.amuid,c.carrieraccountid,a.ParentID from amu_carrier c inner join amu a on a.ID = c.amuid where c.AMUCarrierType=0
)
,
Customerids as 
(
	select distinct c.carrieraccountid,p.Name,c.NameSuffix from carrieraccount c inner join carrierprofile p on c.profileID = p.[ProfileID] 
)
, Traffic_ AS
(
		
	SELECT
            CustomerID,
			Attempts,
			DeliveredNumberOfCalls,
			SuccessfulAttempts,
			DurationsInSeconds,
			NumberOfCalls		
		FROM
			TrafficStatsDaily   WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
        WHERE ( CallDate >= @fromDate AND CallDate <  @ToDate )
        AND customerid NOT IN (SELECT rasc.CID FROM @RepresentedAsSwitchCarriers rasc)
        AND SupplierID NOT IN (SELECT rasc.CID FROM @RepresentedAsSwitchCarriers rasc)
     
		) 
, Traffic AS 
( 
		 SELECT 
		  ISNULL(TS.CustomerID, '') AS CustomerID, 
	       CID.Name as CustomerName, 
	       CID.NameSuffix as Suffix, 
		   Sum(Attempts) as Attempts, 
		   Sum(SuccessfulAttempts) as SuccessfulAttempts,
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes		 
		FROM Traffic_ TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
	  left JOIN amuids AMU ON AMU.CarrierAccountID = TS.CustomerID
	  left JOIN Customerids CID ON CID.carrieraccountid = AMU.carrierAccountID 
	 -- left JOIN Customerids CID ON CID.carrieraccountid = TS.CustomerID    
	    Where AMU.amuid = @AMUID 
	    AND
	    ( ts.CustomerID not  in (SELECT AMU.CarrierAccountID FROM AMU a Where a.ID!=AMU.amuid and Flag like (Select Flag + '%' from AMU a where a.ID =AMU.amuid))
        OR AMU.amuid is null  OR ts.CustomerID in (SELECT AMU.CarrierAccountID FROM AMU a Where AMU.ParentID is not null) OR ts.CustomerID in (SELECT AMU.CarrierAccountID FROM AMU a Where AMU.ParentID is  null)) --a.ID!=AMU.amuid and Flag like (Select Flag + '%' from AMU a where a.ID =AMU.amuid)))-- and (AC.AMUCarrierType=0)   
	    GROUP by amu.CarrierAccountID, CID.Name,CID.NameSuffix, TS.CustomerID
)
 
,Billing AS 
(
	SELECT
	           BS.CustomerID,
              CID.Name customername,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(BS.SaleDuration)/60,0) AS PricedDuration,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0 AS PercentageProfit
	FROM
		 Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date)) 
	     LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
         LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
         left JOIN amuids AMU ON AMU.CarrierAccountID = BS.CustomerID
           left JOIN Customerids CID ON CID.carrieraccountid = AMU.carrierAccountID 
         
	WHERE  (BS.CallDate >= @fromDate AND BS.CallDate < @ToDate)  
	 AND  AMU.amuid = @AMUID	
	GROUP BY CID.Name,AMU.CarrierAccountID,BS.CustomerID
	)
, Results AS 
(
	SELECT T.customername, T.CustomerID as CarrierAccountID,T.Suffix as Suffix, T.Attempts, T.SuccessfulAttempts, T.DurationsInMinutes, 
	        B.Calls AS NumberOfCalls,B.PricedDuration AS PricedDuration, isnull(B.Sale,0) AS Sale_Nets,
	        isnull(B.Cost,0) AS Cost_Nets,isnull(B.Profit,0) AS Profit,0 AS Percentage
	         
	FROM Traffic T WITH(NOLOCK)
	LEFT JOIN Billing B ON T.CustomerID = B.CustomerID 
	
	)
, Final AS
(
	SELECT
		
		 R.customername as CustomerName,
		 R.CarrierAccountID as CarrierAccountID,
		 R.Suffix as Suffix,
		 SUM(R.Attempts) AS Attempts,
		 Sum(R.SuccessfulAttempts) as SuccessfulAttempts,
		 SUM(R.DurationsInMinutes) AS DurationsInMinutes,
		 SUM(R.NumberOfCalls) AS NumberOfCalls,
		 SUM(R.PricedDuration) AS PricedDuration,
		 SUM(R.Sale_Nets) AS Sale_Nets,
		 SUM(R.Cost_Nets) AS Cost_Nets,
		 SUM(R.Profit) AS Profit,
		 AVG(R.Percentage) AS Percentage
		 
	FROM
		Results R LEFT JOIN CarrierAccount C ON R.CarrierAccountID = C.CarrierAccountID
		
	GROUP BY  R.customername,R.CarrierAccountID,R.Suffix
	
)
	
	SELECT  * FROM Final
	
END
END