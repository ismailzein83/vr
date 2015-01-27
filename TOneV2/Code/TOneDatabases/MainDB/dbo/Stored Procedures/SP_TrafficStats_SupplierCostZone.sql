



CREATE PROCEDURE [dbo].[SP_TrafficStats_SupplierCostZone]
   @fromDate datetime,
   @ToDate datetime,
   @SupplierID Varchar(5) = null,
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
	
Declare @Traffic TABLE (ZoneID int, Attempts int, SuccessfulAttempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5))
SET NOCOUNT ON 	


                 
                 
IF @SupplierID IS NULL
	INSERT INTO @Traffic(ZoneID,Attempts, SuccessfulAttempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD)
	SELECT
		   ISNULL(TS.SupplierZoneID, ''), 
		   Sum(NumberOfCalls) as Attempts, 
		   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
		where ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
			AND(@CustomerAmuID IS NULL OR TS.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR TS.SupplierID IN (SELECT * FROM @SupplierIDs))
		
		GROUP BY TS.SupplierZoneID
ELSE
	INSERT INTO @Traffic(ZoneID,Attempts, SuccessfulAttempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD)
	SELECT
		   ISNULL(TS.SupplierZoneID, ''), 
		   Sum(NumberOfCalls) as Attempts, 
		   Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		   Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
		   Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR, 
		   case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
		   Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
		   Avg(PDDinSeconds) as AveragePDD 
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
		
		WHERE FirstCDRAttempt BETWEEN @FromDate AND @ToDate AND (TS.SupplierID = @SupplierID)
			AND ts.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
			AND(@CustomerAmuID IS NULL OR TS.CustomerID IN (SELECT * FROM @CustomerIDs))
			AND(@SupplierAmuID IS NULL OR TS.SupplierID IN (SELECT * FROM @SupplierIDs))
		GROUP BY TS.SupplierZoneID


DECLARE @Result TABLE (
		ZoneID INT PRIMARY KEY,Attempts int, SuccessfulAttempts int, AttemptPercentage DECIMAL(13,5), DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredASR numeric(13,5), AveragePDD numeric(13,5),
		NumberOfCalls int,Cost_Nets float, Sale_Nets float,Profit numeric (13,5),AverageSaleRate DECIMAL(13,5),AverageCostRate DECIMAL(13,5),Percentage numeric (13,5) , PricedDuration  DECIMAL(13,5)
		)

IF @SupplierID IS NULL
begin
;With MyBilling as (Select
		  BS.CostZoneID , 	     
		  Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      AVG(bs.Sale_Rate) AS SaleRate,
	      AVG(bs.Cost_Rate) as CostRate,
	      0	AS Percentage,
	      SUM(BS.CostDuration)/60 AS PricedDuration
from Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) 
				LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
				LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
Where BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate
		AND(@CustomerAmuID IS NULL OR BS.CustomerID IN (SELECT * FROM @CustomerIDs))
		AND(@SupplierAmuID IS NULL OR BS.SupplierID IN (SELECT * FROM @SupplierIDs))
GROUP BY BS.CostZoneID
)
	INSERT INTO @Result(ZoneID ,Attempts, SuccessfulAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, AverageSaleRate, AverageCostRate, Percentage,PricedDuration)
		SELECT 
			  TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,
			      BS.Calls,
				  BS.Cost,
				  BS.Sale,
				  BS.Profit,
				  BS.SaleRate,
				  BS.CostRate,
				  BS.Percentage,
				  BS.PricedDuration 	      
		FROM @Traffic TS 
			LEFT JOIN MyBilling BS  ON TS.ZoneID= BS.CostZoneID 
	--	GROUP BY TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD
end
else
begin
	;With MyBilling as (Select
		  BS.CostZoneID , 	     
		  Sum(BS.NumberOfCalls) AS Calls,
	      SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Cost,
	      Sum(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) AS Sale,
	      SUM(BS.Sale_Nets / ISNULL(ERS.Rate, 1)) - SUM(BS.Cost_Nets / ISNULL(ERC.Rate, 1)) AS Profit,
	      AVG(bs.Sale_Rate) AS SaleRate,
	      AVG(bs.Cost_Rate) as CostRate,
	      0	AS Percentage,
	      SUM(BS.SaleDuration)/60 AS PricedDuration
from Billing_Stats BS WITH (NOLOCK,Index(IX_Billing_Stats_Date)) 
				LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
				LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
Where BS.CallDate >= @fromDate AND BS.CallDate <= @ToDate and BS.SupplierID=@SupplierID
	AND(@CustomerAmuID IS NULL OR BS.CustomerID IN (SELECT * FROM @CustomerIDs))
	AND(@SupplierAmuID IS NULL OR BS.SupplierID IN (SELECT * FROM @SupplierIDs))
GROUP BY BS.CostZoneID
)
	INSERT INTO @Result(ZoneID ,Attempts, SuccessfulAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, NumberOfCalls, Cost_Nets, Sale_Nets, Profit, AverageSaleRate, AverageCostRate, Percentage,PricedDuration)
		SELECT 
		  TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD,	
		  BS.Calls,
		  BS.Cost,
		  BS.Sale,
		  BS.Profit,
		  BS.SaleRate,
		  BS.CostRate,
		  BS.Percentage,
		  BS.PricedDuration        
		FROM @Traffic TS 
			LEFT JOIN MyBilling BS  ON TS.ZoneID= BS.CostZoneID 
	--	GROUP BY TS.ZoneID, TS.Attempts, TS.SuccessfulAttempts, TS.DurationsInMinutes, TS.ASR, TS.ACD, TS.DeliveredASR, TS.AveragePDD
end
Declare @TotalProfit numeric(13,5)
SELECT  @TotalProfit = SUM(profit) FROM @Result


UPDATE  @Result SET Percentage = CASE WHEN @TotalProfit<>0 THEN  (Profit * 100. / @TotalProfit) ELSE 0 END 
Declare @TotalAttempts bigint
SELECT  @TotalAttempts = SUM(Attempts) FROM @Result
Update  @Result SET AttemptPercentage =CASE WHEN @TotalAttempts<>0 THEN  (Attempts * 100. / @TotalAttempts) ELSE 0 END 

SELECT * FROM @Result ORDER BY DurationsInMinutes DESC 

END