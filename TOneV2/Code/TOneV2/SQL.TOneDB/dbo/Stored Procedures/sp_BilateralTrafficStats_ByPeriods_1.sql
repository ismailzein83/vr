CREATE PROCEDURE [dbo].[sp_BilateralTrafficStats_ByPeriods]
@PeriodType varchar(50) = 'Days',
@ZoneID varchar(max)=null,
@SupplierID VarChar(15) = NULL ,
@CustomerID Varchar(15) = NULL ,
@CodeGroup varchar(10) = NULL ,
@FromDate DATETIME ,
@TillDate DATETIME
AS
BEGIN 


SELECT @FromDate = dbo.DateOf(@FromDate)
SELECT @TillDate =  DATEADD(SECOND,-1,DATEADD(day,1,datediff(day,0,@TillDate)))

DECLARE @Results TABLE (
	Zone varchar(100),
    [Day] varchar(20), 
    Attempts numeric(13,5) NULL, 
	DurationsInSeconds numeric(13,5),
	SuccessfulAttempts numeric(13,5), 
	NumberOfCalls numeric(13,5),
	DeliveredAttempts int
    --ASR numeric(13,5), 
	--ACD numeric(13,5)
	--, 
	--DeliveredASR numeric(13,5),
	--AveragePDD numeric (13,5),
	--MaxDuration numeric (13,5),
	--LastAttempt datetime ,
 --   SuccessfulAttempts int
                      )
SET NOCOUNT ON

if @CustomerID is null and @SupplierID IS NULL
	
	INSERT INTO @Results
		(   
			Zone,
			[Day],
			Attempts,
			DurationsInSeconds,
			SuccessfulAttempts,
			NumberOfCalls,
			DeliveredAttempts
			--DurationsInMinutes ,
			--ASR,
			--ACD
			--,
			--DeliveredASR,
			--AveragePDD,
			--MaxDuration ,
			--LastAttempt ,
			--SuccessfulAttempts
		 )	 	
		SELECT 
		OZ.Name,
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds) as DurationsInSeconds, 
			Sum(SuccessfulAttempts) AS SuccessfulAttempts,
			Sum(NumberOfCalls) AS NumberOfCalls,
			sum(DeliveredAttempts) as DeliveredAttempts
			--Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			--case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			--case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD
			--,
			--Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			--Avg(PDDinSeconds) as AveragePDD,
			--MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			--Max(LastCDRAttempt) as LastAttempt,
			--Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst)) 
		LEFT 	JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
			AND (@ZoneID IS NULL OR OZ.ZoneID in (select * from parsearray(@ZoneID,',') ))
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			AND TS.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc )
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
			,OZ.Name
--		ORDER BY Min(FirstCDRAttempt) DESC 
   
if @CustomerID is not null and @SupplierID IS NULL
	
	INSERT INTO @Results
		(   Zone,
			[Day],
			Attempts,
			DurationsInSeconds,
			SuccessfulAttempts,
			NumberOfCalls,
			DeliveredAttempts
			--DurationsInMinutes ,
			--ASR,
			--ACD
			--,
			--DeliveredASR,
			--AveragePDD,
			--MaxDuration ,
			--LastAttempt ,
			--SuccessfulAttempts
		 )	 	
		SELECT 
		OZ.Name,
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
				Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds) as DurationsInSeconds, 
			Sum(SuccessfulAttempts) AS SuccessfulAttempts,
			Sum(NumberOfCalls) AS NumberOfCalls,
			sum(DeliveredAttempts) as DeliveredAttempts
			--Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			--case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			--case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD
			--,
			--Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			--Avg(PDDinSeconds) as AveragePDD,
			--MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			--Max(LastCDRAttempt) as LastAttempt,
			--Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer)) 
		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
     		AND (ts.CustomerID = @CustomerID)
     		AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) 
			AND (@ZoneID IS NULL OR OZ.ZoneID in (select * from parsearray(@ZoneID,',') ))
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
			,OZ.Name
		ORDER BY Min(FirstCDRAttempt) DESC 


if @CustomerID is null and @SupplierID IS not NULL
	
	INSERT INTO @Results
		(   Zone,
			[Day],
			Attempts,
			DurationsInSeconds,
			SuccessfulAttempts,
			NumberOfCalls,
			DeliveredAttempts
			--DurationsInMinutes ,
			--ASR,
			--ACD
			--,
			--DeliveredASR,
			--AveragePDD,
			--MaxDuration ,
			--LastAttempt ,
			--SuccessfulAttempts
		 )	 	
		SELECT 
		OZ.Name,
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds) as DurationsInSeconds, 
			Sum(SuccessfulAttempts) AS SuccessfulAttempts,
			Sum(NumberOfCalls) AS NumberOfCalls,
			sum(DeliveredAttempts) as DeliveredAttempts
			--Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			--case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			--case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD
			--,
			--Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			--Avg(PDDinSeconds) as AveragePDD,
			--MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			--Max(LastCDRAttempt) as LastAttempt,
			--Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier)) 
		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
     		AND (ts.SupplierID = @SupplierID) AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc ) 
			AND (@ZoneID IS NULL OR OZ.ZoneID in (select * from parsearray(@ZoneID,',') ))
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
			,OZ.Name
		ORDER BY Min(FirstCDRAttempt) DESC 

if @CustomerID  is not null and @SupplierID IS not NULL
	
	INSERT INTO @Results
		(   Zone,
			[Day],
			Attempts,
			DurationsInSeconds,
			SuccessfulAttempts,
			NumberOfCalls,
			DeliveredAttempts
			--DurationsInMinutes ,
			--ASR,
			--ACD
			--,
			--DeliveredASR,
			--AveragePDD,
			--MaxDuration ,
			--LastAttempt ,
			--SuccessfulAttempts
		 )	 	
		SELECT 
		OZ.Name,
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Date],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds) as DurationsInSeconds, 
			Sum(SuccessfulAttempts) AS SuccessfulAttempts,
			Sum(NumberOfCalls) AS NumberOfCalls,
			sum(DeliveredAttempts) as DeliveredAttempts
			--Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
			--case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			--case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD
			--,
			--Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			--Avg(PDDinSeconds) as AveragePDD,
			--MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			--Max(LastCDRAttempt) as LastAttempt,
			--Sum(SuccessfulAttempts) as SuccessfulAttempts
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier)) 
		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
		WHERE 
			TS.FirstCDRAttempt >= @FromDate AND TS.FirstCDRAttempt <=@TillDate
     		AND (ts.CustomerID = @CustomerID) 
     		AND (ts.SupplierID = @SupplierID) 
				AND (@ZoneID IS NULL OR OZ.ZoneID in (select * from parsearray(@ZoneID,',') ))
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)
			AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)
		GROUP BY 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
			END
			,OZ.Name
		ORDER BY Min(FirstCDRAttempt) DESC 

	SELECT * FROM @Results ORDER BY [Day] 
END