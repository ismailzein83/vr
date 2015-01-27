



CREATE PROCEDURE [dbo].[SP_TrafficStats_CustomerSummary_Enhanced_New]
   @fromDate datetime ,
   @ToDate datetime,
   @CustomerID varchar(15)=null,
   @TopRecord INT =NULL
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
 
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)



Declare @Traffic TABLE (CustomerID VARCHAR(15) primary KEY, Attempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))

SET NOCOUNT ON 	
if @CustomerID is Null
	INSERT INTO @Traffic(CustomerID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD)

	 SELECT 
		   ISNULL(TS.CustomerID, ''), 
		   Sum(NumberofCalls) as Attempts, 
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   case when Sum(NumberofCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberofCalls) ELSE 0 end as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   case when Sum(NumberofCalls) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(NumberofCalls) ELSE 0 end as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
	     
		FROM TrafficStats TS WITH(NOLOCK)
	    
        WHERE ( FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <=  @ToDate )
         AND ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		
	 GROUP BY ISNULL(TS.CustomerID, '')  

else
	INSERT INTO @Traffic(CustomerID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD)

	 SELECT 
		   ISNULL(TS.CustomerID, ''), 
		   Sum(NumberofCalls) as Attempts, 
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   case when Sum(NumberofCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberofCalls) ELSE 0 end as ASR,
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   case when Sum(NumberofCalls) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(NumberofCalls) ELSE 0 end as DeliveredASR,
		   Avg(PDDinSeconds) as AveragePDD 
	     
		FROM TrafficStats TS WITH(NOLOCK)
		
	    WHERE( FirstCDRAttempt >= @FromDate AND FirstCDRAttempt <=  @ToDate ) AND (TS.CustomerID=@CustomerID)
		AND ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		GROUP BY ISNULL(TS.CustomerID, '')  

DECLARE @Result TABLE (CustomerID VARCHAR(15)PRIMARY KEY, Attempts int,DurationsInMinutes numeric(13,5),ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), NumberOfCalls int, Cost_Nets float, Sale_Nets float, Profit numeric(13,5), Percentage Float)

if @CustomerID is Null
	INSERT INTO @Result(CustomerID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Sale_Nets, Cost_Nets, Profit, Percentage)
		SELECT 
			  T.CustomerID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0
		FROM 
			@Traffic T 
				LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) ON T.CustomerID = BS.CustomerID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) 
				LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
                LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate 
		GROUP BY T.CustomerID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD 
else
	INSERT INTO @Result(CustomerID, Attempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Sale_Nets, Cost_Nets, Profit, Percentage)
		SELECT 
			  T.CustomerID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
			  ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
			  ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
			  ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
			  0
		FROM 
			@Traffic T 
				LEFT JOIN Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date,IX_Billing_Stats_Customer)) ON T.CustomerID = BS.CustomerID AND (BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate) AND (BS.CustomerID=@CustomerID) 
			    LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
                LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
		GROUP BY T.CustomerID, T.Attempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD 

Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result
UPDATE  @Result SET Percentage = CASE WHEN @TotalProfit <> 0 THEN (Profit * 100.0 / @TotalProfit) ELSE 0 END

SET ROWCOUNT @TopRecord

SELECT * FROM @Result ORDER  BY DurationsInMinutes DESC 

END