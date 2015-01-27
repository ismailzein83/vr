CREATE    PROCEDURE [dbo].[SP_TrafficStats_SupplierSummary]
   @fromDate datetime ,
   @ToDate datetime,
   @SupplierID varchar(15)=null,
   @TopRecord INT =NULL,
   @GroupByProfile char(1) = 'N',
   @CustomerAmuID int = NULL,
   @SupplierAmuID int = NULL
AS
BEGIN

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
IF @GroupByProfile = 'N'
BEGIN
;WITH Traffic_ AS
	(
		
		SELECT
			SupplierID,
			CallDate,
			Attempts,
			DeliveredAttempts,
			SuccessfulAttempts,
			DurationsInSeconds,
			PDDInSeconds,
			NumberOfCalls
			
		FROM
			TrafficStatsDaily ts  WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
        WHERE ( CallDate >= @fromDate AND CallDate <  @ToDate )
			AND (@SupplierID IS NULL OR ts.SupplierID= @SupplierID)
			AND CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
			AND(@CustomerAmuID IS NULL OR CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR SupplierID IN (SELECT * FROM @SupplierIDs))
		) 
, Traffic AS 
( 
		 SELECT 
           ISNULL(TS.SupplierID, '') AS SupplierID, 
		   Sum(Attempts) as Attempts,
		   Sum(SuccessfulAttempts) as SuccessfulAttempts, 
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM Traffic_ TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
		  where  (@SupplierID IS NULL OR ts.SupplierID = @SupplierID)
	   
	 GROUP BY ISNULL(TS.SupplierID, '')  
), 
Billing AS 
(
	SELECT
	           BS.SupplierID AS SupplierID,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(bs.CostDuration)/60,0) AS PricedDuration,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0 AS PercentageProfit
	FROM
		 Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date)) 
	     LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
         LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  (BS.CallDate >= @fromDate AND BS.CallDate < @ToDate)
		AND (@SupplierID IS NULL OR BS.SupplierID =  @SupplierID)
		AND CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		AND SupplierID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		AND(@CustomerAmuID IS NULL OR CustomerID IN (SELECT * FROM @CustomerIDs))
		AND(@SupplierAmuID IS NULL OR SupplierID IN (SELECT * FROM @SupplierIDs))
	GROUP BY BS.SupplierID
	)
,
 Results AS 
(
	SELECT  T.SupplierID, T.Attempts, T.SuccessfulAttempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
	       B.Calls AS NumberOfCalls,B.PricedDuration AS PricedDuration,ISNULL( B.Sale,0) AS Sale_Nets,ISNULL( B.Cost,0) AS Cost_Nets,ISNULL( B.Profit,0) AS Profit,0 AS Percentage
	       ,  ROW_NUMBER() OVER (ORDER BY DurationsInMinutes DESC) AS rownIndex
	         
	FROM Traffic T WITH(NOLOCK)
	LEFT JOIN Billing B ON T.SupplierID= B.SupplierID
	
	)


SELECT * FROM Results 
WHERE rownIndex <= @TopRecord
END

IF @GroupByProfile= 'Y'
BEGIN

;WITH Traffic_ AS
	(
		
		SELECT
			SupplierID,
			Calldate,
			Attempts,
			DeliveredAttempts,
			SuccessfulAttempts,
			DurationsInSeconds,
			PDDInSeconds,
			NumberOfCalls
			
		FROM
			TrafficStatsDaily ts  WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
        WHERE ( Calldate >= @fromDate AND Calldate <  @ToDate )
			AND (@SupplierID IS NULL OR ts.SupplierID= @SupplierID)
			AND CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
			AND(@CustomerAmuID IS NULL OR CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR SupplierID IN (SELECT * FROM @SupplierIDs))
		) 
, Traffic AS 
( 
		 SELECT 
   ISNULL(TS.SupplierID, '') AS SupplierID, 
		   Sum(Attempts) as Attempts, 
		   Sum(SuccessfulAttempts) as SuccessfulAttempts,
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM Traffic_ TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
	   
	 GROUP BY ISNULL(TS.SupplierID, '')  
), 
Billing AS 
(
	SELECT
	           BS.SupplierID AS SupplierID,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(bs.CostDuration)/60,0) AS PricedDuration,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0 AS PercentageProfit
	FROM
		 Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date)) 
	     LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
         LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE  (BS.CallDate >= @fromDate AND BS.CallDate < @ToDate)
		AND (@SupplierID IS NULL OR BS.SupplierID =  @SupplierID)
		AND CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		AND SupplierID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		AND(@CustomerAmuID IS NULL OR CustomerID IN (SELECT * FROM @CustomerIDs))
		AND(@SupplierAmuID IS NULL OR SupplierID IN (SELECT * FROM @SupplierIDs))
	GROUP BY BS.SupplierID
	)
,
 Results AS 
(
	SELECT  T.SupplierID, T.Attempts, T.SuccessfulAttempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
	       B.Calls AS NumberOfCalls,B.PricedDuration AS PricedDuration,ISNULL( B.Sale,0) AS Sale_Nets,
	       ISNULL( B.Cost,0) AS Cost_Nets,ISNULL( B.Profit,0) AS Profit,0 AS Percentage
	       
	         
	FROM Traffic T WITH(NOLOCK)
	LEFT JOIN Billing B ON T.SupplierID= B.SupplierID
	
	)
	, Final AS
(
	SELECT
		 P.ProfileID AS ProfileID,
		 SUM(R.Attempts) AS Attempts,
		 Sum(R.SuccessfulAttempts) as SuccessfulAttempts,
		 SUM(R.DurationsInMinutes) AS DurationsInMinutes,
		 AVG(R.ASR) AS ASR,
		 AVG(R.ACD) AS ACD,
		 AVG(R.DeliveredASR) AS DeliveredASR,
		 AVG(R.AveragePDD) AS AveragePDD,
		 SUM(R.NumberOfCalls) AS NumberOfCalls,
		 SUM(R.PricedDuration) AS PricedDuration,
		 SUM(R.Sale_Nets) AS Sale_Nets,
		 SUM(R.Cost_Nets) AS Cost_Nets,
		 SUM(R.Profit) AS Profit,
		 AVG(R.Percentage) AS Percentage
		 --ROW_NUMBER() OVER (ORDER BY DurationsInMinutes DESC) AS rownIndex
	FROM
		Results R LEFT JOIN CarrierAccount C ON R.SupplierID = C.CarrierAccountID
		LEFT JOIN CarrierProfile P ON C.ProfileID = P.ProfileID
	GROUP BY P.ProfileID
	
)
SELECT top (@TopRecord) * FROM Final-- WHERE  
	ORDER BY DurationsInMinutes DESC
END
END