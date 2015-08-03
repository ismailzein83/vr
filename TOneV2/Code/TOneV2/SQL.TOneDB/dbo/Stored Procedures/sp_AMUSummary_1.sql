
CREATE PROCEDURE [dbo].[sp_AMUSummary]
(

   @fromDate datetime ,
   @ToDate datetime
   )
AS
BEGIN


--DECLARE @TRafficStats table	(CustomerID varchar(10),Attempts int , SuccessfulAttempts int, DurationInMinutes numeric(19,5),ASR float, ACD float,DeliveredASR float,AveragePDD float,NumberOfCalls int,PricedDuration numeric(13,4),
--Sale_Nets float,Cost_Nets float,Profit float,Percentage float, rowIndex int)

--DECLARE @Results table (AccountManagerUser nvarchar(255),Attempts int, SuccessfulAttempts int,DurationInMinutes float,PricedDuration float, Sale_Nets float,Cost_Nets float,Profit float,Percentage float)

DECLARE @amuids  table (amuid int,carrierAccountid varchar(5),ParentId int)
Insert Into @amuids  
	select distinct c.amuid,c.carrieraccountid,a.ParentID from amu_carrier c inner join amu a on a.ID = c.amuid where c.AMUCarrierType=0


DECLARE @amuNames  table (amuid int,AMUName nvarchar(255) )

Insert INTO @amuNames  

	select distinct amu.id,u.Name as AMUName from  amu amu ,[user] u where amu.UserID= u.ID
	
	
DECLARE @IndirectCarriers  table (amuid int,UserID int,ParentID int)
Insert Into @IndirectCarriers  
    select distinct ID,UserID,ParentID from amu where amu.ParentID in (Select ID from amu) 
	


	--Insert into @TRafficStats  exec [SP_TrafficStats_CustomerSummary]  @fromDate =@fromDate,
 --  @ToDate =@ToDate,
 --  @CustomerID =null,
 --  @TopRecord  =100,
 --  @GroupByProfile  = 'N',
 --  @CustomerAmuID  = NULL,
 --  @SupplierAmuID  = NULL
   
   
   
   
   DECLARE   @CustomerID varchar(15)=null,@CustomerAmuID int = NULL,@SupplierAmuID int = NULL
   DECLARE @CustomerIDs TABLE( CarrierAccountID VARCHAR(5) )
	DECLARE @SupplierIDs TABLE( CarrierAccountID VARCHAR(5) )
	
	IF(@CustomerAMUID IS NOT NULL)
	BEGIN
		DECLARE @customerAmuFlag VARCHAR(20)
		SET @customerAmuFlag = (SELECT Flag FROM AMU WHERE ID = @CustomerAMUID)
		INSERT INTO @CustomerIDs
		SELECT ac.CarrierAccountID
		FROM AMU_Carrier ac
		WHERE ac.AMUCarrierType = 0
		AND ac.AMUID IN (
			SELECT ID FROM AMU
			WHERE Flag LIKE @customerAmuFlag + '%'
			)
	END

	IF(@SupplierAMUID IS NOT NULL)
	BEGIN	
		DECLARE @supplierAmuFlag VARCHAR(20)
		SET @supplierAmuFlag = (SELECT Flag FROM AMU WHERE ID = @SupplierAMUID)
		INSERT INTO @SupplierIDs
		SELECT ac.CarrierAccountID
		FROM AMU_Carrier ac
		WHERE ac.AMUCarrierType = 1
		AND ac.AMUID IN (
			SELECT ID FROM AMU
			WHERE Flag LIKE @supplierAmuFlag + '%'
			)
	END
	
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
		Calldate,
		Attempts,
		DeliveredAttempts,
		DeliveredNumberOfCalls,
		SuccessfulAttempts,
		DurationsInSeconds,
		PDDInSeconds,
		NumberOfCalls
		
	FROM
		TrafficStatsDaily ts   WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
    WHERE (Calldate >= @fromDate AND Calldate <  @ToDate )
		AND CustomerID NOT IN (SELECT rasc.CID FROM @RepresentedAsSwitchCarriers rasc)
        AND SupplierID NOT IN (SELECT rasc.CID FROM @RepresentedAsSwitchCarriers rasc)
        AND(@CustomerAmuID IS NULL OR CustomerID IN (SELECT * FROM @CustomerIDs))
		AND(@SupplierAmuID IS NULL OR SupplierID IN (SELECT * FROM @SupplierIDs))
) 
, Traffic AS 
( 
	SELECT 
	   ISNULL(TS.CustomerID, '') AS CustomerID, 
	   Sum(Attempts) as Attempts, 
	   Sum(SuccessfulAttempts) as SuccessfulAttempts,
	   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
	   case when Sum(NumberofCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberofCalls) ELSE 0 end as ASR, 
	   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
	   case when Sum(NumberofCalls) > 0 then Sum(DeliveredNumberofCalls) * 100.0 / SUM(NumberofCalls) ELSE 0 end as DeliveredASR, 
	   Avg(PDDinSeconds) as AveragePDD 
	FROM Traffic_ TS WITH(NOLOCK, INDEX(IX_TrafficStatsDaily_DateTimeFirst))
	WHERE  (@CustomerID IS NULL OR ts.CustomerId = @CustomerID)   
	GROUP BY ISNULL(TS.CustomerID, '')  
), 
Billing AS 
(
	SELECT
      BS.CustomerID AS CustomerID,
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
	WHERE  (BS.CallDate >= @fromDate AND BS.CallDate < @ToDate)
		AND (@CustomerID IS NULL OR BS.CustomerID =  @CustomerID)
		AND(@CustomerAmuID IS NULL OR BS.CustomerID IN (SELECT * FROM @CustomerIDs))
		AND(@SupplierAmuID IS NULL OR BS.SupplierID IN (SELECT * FROM @SupplierIDs))
	GROUP BY BS.CustomerID
	)
, Results AS 
(
	SELECT  T.CustomerID, T.Attempts,T.SuccessfulAttempts, T.DurationsInMinutes, 
	        B.Calls AS NumberOfCalls,B.PricedDuration AS PricedDuration, isnull(B.Sale,0) AS Sale_Nets,isnull(B.Cost,0) AS Cost_Nets,isnull(B.Profit,0) AS Profit,0 AS Percentage
	         ,  ROW_NUMBER() OVER (ORDER BY DurationsInMinutes DESC) AS rownIndex
	FROM Traffic T WITH(NOLOCK)
	LEFT JOIN Billing B ON T.CustomerID = B.CustomerID
	)
	
	--SELECT * FROM Results Where CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc) 
   
      
SELECT 
 isnull(AMU.amuid,0) as TrafficAMUID,
 AMUn.AMUName  as TrafficAMUName,
 --ts.CustomerID,
 isnull(sum(Attempts),0)  Attempts , 
 isnull(sum(SuccessfulAttempts),0)  SuccessfulAttempts , 
 isnull(sum(DurationsInMinutes ),0)  DurationsInMinutes, 
 isnull(sum(NumberOfCalls),0) PricedCalls ,
 isnull(sum(PricedDuration),0) PricedDuration ,
 isnull(sum(Sale_Nets),0)  Sale_Nets ,
 isnull(sum(Cost_Nets),0) Cost_Nets ,
 isnull(sum(Profit),0) Profit ,
 isnull(sum(Percentage),0) Percentage 
  FROM Results ts left JOIN @amuids AMU ON AMU.CarrierAccountID = ts.CustomerID
     left JOIN @amuNames AMUn  ON AMU.AMUID = AMUn.amuid
 where( ts.CustomerID not  in (SELECT CarrierAccountID FROM AMU a Where a.ID!=AMU.amuid and Flag like (Select Flag + '%' from AMU a where a.ID =AMU.amuid))
OR AMU.amuid is null  OR ts.CustomerID in (SELECT CarrierAccountID FROM AMU a Where AMU.ParentID is not null) OR ts.CustomerID in (SELECT CarrierAccountID FROM AMU a Where AMU.ParentID is  null))    
group by  AMUn.AMUName ,AMU.amuid--,ts.CustomerID


END