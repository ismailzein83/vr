
CREATE PROCEDURE [dbo].[EA_ZoneMonitor_with_BillingStats]
(
	@FromDate Datetime,
	@ToDate Datetime,
	@CustomerID varchar(5)=NULL,
	@SupplierID varchar(5)=NULL,   
    @CodeGroup varchar(10) = NULL,
    @AllAccounts varchar(MAX) = NULL
)
WITH Recompile
AS
BEGIN
DECLARE @ExchangeRates TABLE(
			Currency VARCHAR(3),
			Date SMALLDATETIME,
			Rate FLOAT
			PRIMARY KEY(Currency, Date)
				)
	
	INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate,@ToDate)
	SELECT 
		bs.CallDate AS CallDate,
		bs.SaleZoneID AS SaleZone, 
		bs.CostZoneID AS CostZone, 
		bs.CustomerID AS CustomerID,
		bs.SupplierID  AS SupplierID,
	ISNULL(bs.Sale_Rate  / ISNULL(ERS.Rate, 1),0.0) AS SaleRate,
	ISNULL(bs.Cost_Rate / ISNULL(ERC.Rate, 1),0.0) AS CostRate,
		ISNULL(SUM(bs.SaleDuration/60.0),0) AS SaleDuration,
		ISNULL(SUM(bs.CostDuration/60.0),0) AS CostDuration,
		ISNULL(SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)),0.0) AS SaleNet,
		ISNULL(SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)),0.0) AS  CostNet  
	INTO #tempBilling       			
    FROM
	    Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date),INDEX(IX_Billing_Stats_Customer),INDEX(IX_Billing_Stats_Supplier))
	    LEFT JOIN Zone SZ WITH(NOLOCK) ON SZ.ZoneID = bs.SaleZoneID
		LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
		LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
	WHERE        
		bs.calldate>=@FromDate AND bs.calldate<@ToDate
	 	AND (@CustomerID IS NULL OR bs.CustomerID=@CustomerID) 
        AND (@SupplierID IS NULL OR bs.SupplierID=@SupplierID)
        AND bs.Sale_Rate <> 0
	 	AND bs.Cost_Rate <> 0
	GROUP BY 
	     bs.CallDate
	    ,bs.CustomerID
	    ,bs.SupplierID
	    ,bs.SaleZoneID
	    ,bs.CostZoneID
	  	,bs.Sale_Rate  / ISNULL(ERS.Rate, 1) 
		,bs.Cost_Rate / ISNULL(ERC.Rate, 1) 


Declare @Results TABLE ( AttemptDateTime DATETIME, OurZoneID INT ,Attempts int, FailedAttempts int, DurationsInMinutes numeric(13,5), CeiledDuration numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredAttempts INT, DeliveredASR numeric(13,5), AveragePDD numeric(13,5), MaxDuration numeric(13,5), LastAttempt datetime, AttemptPercentage numeric(13,5),DurationPercentage numeric(13,5),SuccesfulAttempts int )
	SET NOCOUNT ON
	-- Customer, No Supplier
	IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
	BEGIN
		INSERT INTO @Results (AttemptDateTime ,OurZoneID,Attempts, DurationsInMinutes, CeiledDuration ,ASR,ACD, deliveredAttempts,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
		SELECT 
			   DATEADD(day,0,datediff(day,0, ts.FirstCDRAttempt)) AS AttemptDateTime,
				TS.OurZoneID as ourZoneID,          
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				SUM(ceiledDuration)/60.0 AS CeiledDuration,
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) AS deliveredAttempts, 
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0 as A
				,0 as B,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer)) 
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			 LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				WHERE 
					FirstCDRAttempt >=  @FromDate AND FirstCDRAttempt<@ToDate--'2014-02-01'  
				AND  TS.CustomerID ='c070'
				AND ( CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') 
			Group By 
				DATEADD(day,0,datediff(day,0, ts.FirstCDRAttempt)), 
				TS.OurZoneID 
	END
	-- No Customer, Supplier
	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
		INSERT INTO @Results (AttemptDateTime, OurZoneID, Attempts, DurationsInMinutes, CeiledDuration ,ASR,ACD,DeliveredAttempts,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
			    min(ts.FirstCDRAttempt),
				TS.SupplierZoneID AS OurZoneID,          
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				SUM(ceiledDuration)/60.0 AS CeiledDuration,
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) AS DeliveredAttempts, 
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier)) 
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			 LEFT JOIN Zone AS OZ WITH (nolock) ON TS.SupplierZoneID = OZ.ZoneID
			
				WHERE 
					FirstCDRAttempt BETWEEN  @FromDate AND @ToDate 
				AND TS.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) 
				--AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
				AND (CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') 
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			Group By 
			    --dbo.DateOf(ts.FirstCDRAttempt),
				TS.SupplierZoneID
						
	--Percentage For Attempts-----
	Declare @TotalAttempts bigint
	SELECT  @TotalAttempts = SUM(Attempts) FROM @Results
	Update  @Results SET AttemptPercentage= (Attempts * 100. / @TotalAttempts)

	SELECT SUM(R.Attempts),SUM(R.DurationsInMinutes),SUM(r.CeiledDuration)  FROM @Results R
	SELECT  t.*,b.*,ROW_NUMBER() OVER (PARTITION BY b.SaleZone,b.SaleRate order by b.SaleZone ASC,b.SaleRate ASC) AS RN
	into #temp
	FROM  @Results t  left join #tempBilling  b ON b.CallDate = t.AttemptDateTime and b.SaleZone = t.OurZoneID
	where b.SaleRate!=null
	 Order By Attempts DESC ,DurationsInMinutes DESC
	
	select * from #temp where RN>0
END