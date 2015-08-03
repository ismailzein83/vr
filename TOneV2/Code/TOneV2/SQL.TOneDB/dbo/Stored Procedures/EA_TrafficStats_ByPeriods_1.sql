

CREATE PROCEDURE [dbo].[EA_TrafficStats_ByPeriods]
@PeriodType varchar(50) = 'Days',
@ZoneID INT = NULL ,
@SupplierID VarChar(15) = NULL ,
@CustomerID Varchar(15) = NULL ,
@CodeGroup varchar(10) = NULL ,
@FromDate DATETIME ,
@TillDate DATETIME,
@AllAccounts VARCHAR(MAX) = NULL
AS
BEGIN 


SET @FromDate = DATEADD(day,0,datediff(day,0, @FromDate))
SET @TillDate = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @TillDate)))




SET NOCOUNT ON

if @ZoneID is null and @CodeGroup is null
Begin
	
	 WITH
	 
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
		       , ts.CeiledDuration AS CeiledDuration
		       , ts.PDDInSeconds AS PDDInSeconds
		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
		       , ts.NumberOfCalls AS NumberOfCalls
		       , ts.LastCDRAttempt AS LastCDRAttempt 
		       , ts.FirstCDRAttempt AS FirstCDRAttempt
		 FROM TrafficStats ts WITH(NOLOCK,index(IX_TrafficStats_DateTimeFirst))  WHERE 
		
	  FirstCDRAttempt >= @FromDate
	       AND FirstCDRAttempt < @TillDate
	       AND ((@CustomerID IS NULL OR ts.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))))
	       AND  ((@SupplierID IS NULL OR ts.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))))
	       AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)     
	 ),
	 
	
	Traffic AS (
		 	
		SELECT 
			
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Day],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,
			SUM(CeiledDuration)/60.0 AS CeiledDuration,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		 FROM Traffic_Stats_Data ts 
			 
		GROUP BY
			 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)
					
			END
			
			)
		SELECT * FROM Traffic 
		ORDER BY [Day]
	End
Else
Begin
	
	 WITH
	 
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
		       , ts.CeiledDuration AS CeiledDuration
		       , ts.PDDInSeconds AS PDDInSeconds
		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
		       , ts.NumberOfCalls AS NumberOfCalls
		       , ts.LastCDRAttempt AS LastCDRAttempt 
		       , ts.FirstCDRAttempt AS FirstCDRAttempt
		 FROM TrafficStats ts WITH(NOLOCK,index=IX_TrafficStats_DateTimeFirst)  WHERE 
		
	  FirstCDRAttempt >= @FromDate
	       AND FirstCDRAttempt < @TillDate
	       AND ((@CustomerID IS NULL OR ts.CustomerID = @CustomerID) OR (@AllAccounts IS NULL OR ts.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))))
	       AND  ((@SupplierID IS NULL OR ts.SupplierID = @SupplierID) OR (@AllAccounts IS NULL OR ts.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))))
	       AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)     
	 ),
	 
	
	Traffic AS (
		 	
		SELECT 
			
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Day],
			Sum(Attempts) as Attempts ,
			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,
			SUM(CeiledDuration)/60.0 AS CeiledDuration,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		 FROM Traffic_Stats_Data ts 
		LEFT 	JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
			 
		WHERE 
		   (@ZoneID IS NULL OR ts.OurZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		GROUP BY
			 
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)
					
			END
			
			)
		SELECT * FROM Traffic 
		ORDER BY [Day]
	End

--if @CustomerID is not null and @SupplierID IS NULL

--with
--	Traffic_Stats_Data AS (
--		SELECT 
		        
--		         ts.CustomerID AS CustomerID
--		       , ts.OurZoneID AS OurZoneID
--		       , ts.SupplierID AS SupplierID
--		       , ts.SupplierZoneID AS SupplierZoneID
--		       , ts.Attempts AS Attempts
--		       , ts.DeliveredAttempts AS DeliveredAttempts 
--		       , ts.DeliveredNumberOfCalls AS DeliveredNumberOfCalls
--		       , ts.SuccessfulAttempts AS SuccessfulAttempts
--		       , ts.DurationsInSeconds AS DurationsInSeconds
--		       , ts.PDDInSeconds AS PDDInSeconds
--		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
--		       , ts.NumberOfCalls AS NumberOfCalls
--		       , ts.LastCDRAttempt AS LastCDRAttempt 
--		       , ts.FirstCDRAttempt AS FirstCDRAttempt
--		 FROM TrafficStats ts WITH(NOLOCK,index=IX_TrafficStats_DateTimeFirst)  WHERE 
		
--	  FirstCDRAttempt >= @FromDate
--	       AND FirstCDRAttempt <= @TillDate
--	       AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
	            
--	 ),
	 
--	Traffic AS (
--		SELECT 
-- 			CASE 
--				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
--				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
--				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
--			END AS [Date],
--			Sum(Attempts) as Attempts ,
--			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
--			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
--			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
--			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
--			Avg(PDDinSeconds) as AveragePDD,
--			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
--			Max(LastCDRAttempt) as LastAttempt,
--			Sum(SuccessfulAttempts) as SuccessfulAttempts
--		 FROM TrafficStats ts WITH(NOLOCK,index=IX_TrafficStats_DateTimeFirst)  
--		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			
--		WHERE 
			
--     	 --(ts.CustomerID = @CustomerID)
--			--AND 
--			(@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
--			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			
--		GROUP BY 
--			CASE 
--				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
--				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
--				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
--			END
--	)
	
--	SELECT * FROM traffic
-- ORDER BY [Date]
 
--if @CustomerID is null and @SupplierID IS not NULL
--	with
--	Traffic_Stats_Data AS (
--		SELECT 
		        
		       
--		        ts.OurZoneID AS OurZoneID
--		       , ts.SupplierID AS SupplierID
--		       , ts.SupplierZoneID AS SupplierZoneID
--		       , ts.Attempts AS Attempts
--		       , ts.DeliveredAttempts AS DeliveredAttempts 
--		       , ts.DeliveredNumberOfCalls AS DeliveredNumberOfCalls
--		       , ts.SuccessfulAttempts AS SuccessfulAttempts
--		       , ts.DurationsInSeconds AS DurationsInSeconds
--		       , ts.PDDInSeconds AS PDDInSeconds
--		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
--		       , ts.NumberOfCalls AS NumberOfCalls
--		       , ts.LastCDRAttempt AS LastCDRAttempt 
--		       , ts.FirstCDRAttempt AS FirstCDRAttempt
--		 FROM TrafficStats ts WITH(NOLOCK,index=IX_TrafficStats_DateTimeFirst)  WHERE 
		
--	  FirstCDRAttempt >= @FromDate
--	       AND FirstCDRAttempt <= @TillDate
--	   AND (ts.SupplierID = @SupplierID) AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc )      
--	 ),
	 
--	 Traffic AS (
	 
--		SELECT 
-- 			CASE 
--				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
--				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
--				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
--			END AS [Date],
--			Sum(Attempts) as Attempts ,
--			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
--			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
--			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
--			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
--			Avg(PDDinSeconds) as AveragePDD,
--			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
--			Max(LastCDRAttempt) as LastAttempt,
--			Sum(SuccessfulAttempts) as SuccessfulAttempts
--		 FROM TrafficStats ts WITH(NOLOCK,index=IX_TrafficStats_DateTimeFirst)  
--		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
--		WHERE 
     		 
--			 (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
--			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			
--		GROUP BY 
--			CASE 
--				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
--				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
--				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
--			END
	
--	 )
--	 SELECT * FROM traffic
--	 ORDER BY [Date]
	 
--if @CustomerID  is not null and @SupplierID IS not NULL
	
--	 	with
--	Traffic_Stats_Data AS (
--		SELECT 
		        
--		        ts.CustomerID AS CustomerID
--		       , ts.OurZoneID AS OurZoneID
--		       , ts.SupplierID AS SupplierID
--		       , ts.SupplierZoneID AS SupplierZoneID
--		       , ts.Attempts AS Attempts
--		       , ts.DeliveredAttempts AS DeliveredAttempts 
--		       , ts.DeliveredNumberOfCalls AS DeliveredNumberOfCalls
--		       , ts.SuccessfulAttempts AS SuccessfulAttempts
--		       , ts.DurationsInSeconds AS DurationsInSeconds
--		       , ts.PDDInSeconds AS PDDInSeconds
--		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
--		       , ts.NumberOfCalls AS NumberOfCalls
--		       , ts.LastCDRAttempt AS LastCDRAttempt 
--		       , ts.FirstCDRAttempt AS FirstCDRAttempt
--		 FROM TrafficStats ts WITH(NOLOCK,index=IX_TrafficStats_DateTimeFirst)  WHERE 
		
--	  FirstCDRAttempt >= @FromDate
--	       AND FirstCDRAttempt <= @TillDate
--	       --AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc) 
--	       --AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)    
--	 ),
	 
--	 Traffic AS (
	 
--		SELECT 
-- 			CASE 
--				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
--				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
--				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
--			END AS [Date],
--			Sum(Attempts) as Attempts ,
--			Sum(DurationsInSeconds)/60.0 as DurationsInMinutes,          
--			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
--			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts) else NULL end as ACD,
--			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
--			Avg(PDDinSeconds) as AveragePDD,
--			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
--			Max(LastCDRAttempt) as LastAttempt,
--			Sum(SuccessfulAttempts) as SuccessfulAttempts
--		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier)) 
--		LEFT JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
--		WHERE 
			
--     		 (ts.CustomerID = @CustomerID) 
--     		AND (ts.SupplierID = @SupplierID) 
--			AND (@ZoneID IS NULL OR OZ.ZoneID= @ZoneID) 
--			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
--			 AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc) 
--	       AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)  
--		GROUP BY 
--			CASE 
--				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
--				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
--				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)	
--			END
		
--)
--	SELECT * FROM Traffic ORDER BY [Date] 
	
END