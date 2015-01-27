﻿

CREATE PROCEDURE [dbo].[sp_TrafficStats_ByPeriodsDashboard]
@PeriodType varchar(50) = 'Days',
@ZoneID INT = NULL ,
@SupplierID VarChar(15) = NULL ,
@CustomerID Varchar(15) = NULL ,
@CodeGroup varchar(10) = NULL ,
@FromDate DATETIME ,
@TillDate DATETIME,
@AllAccounts VARCHAR(MAX) = NULL


AS
DECLARE @SQLString nvarchar(4000)
	declare @tempTableName nvarchar(1000)
	declare @exists bit
	
	set @SQLString=''

BEGIN 

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
		       , ts.PDDInSeconds AS PDDInSeconds
		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
		       , ts.NumberOfCalls AS NumberOfCalls
		       , ts.LastCDRAttempt AS LastCDRAttempt 
		       , ts.FirstCDRAttempt AS FirstCDRAttempt
		 FROM TrafficStats ts WITH(NOLOCK,index(IX_TrafficStats_DateTimeFirst))  WHERE 
		
	  FirstCDRAttempt >= @FromDate
	       AND FirstCDRAttempt < @TillDate
	       AND ((@CustomerID IS NOT NULL AND ts.CustomerID = @CustomerID) OR (@AllAccounts IS NOT NULL AND ts.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@CustomerID IS NULL AND @AllAccounts IS NULL))
	       AND  ((@SupplierID IS NOT NULL AND ts.SupplierID = @SupplierID) OR (@AllAccounts IS NOT NULL AND ts.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@SupplierID IS NULL AND @AllAccounts IS NULL))
	       AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)     
	 ),
	 
	
	Traffic AS (
		 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Day],
			ts.CustomerID as CustomerID,
			Sum(Attempts) as Attempts ,
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/60.0) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls)) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)) else NULL end as ACD,
			CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(Attempts)) as DeliveredASR, 
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		 FROM Traffic_Stats_Data ts 
			 
		GROUP BY
			
			ts.CustomerID,
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
		       , ts.PDDInSeconds AS PDDInSeconds
		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
		       , ts.NumberOfCalls AS NumberOfCalls
		       , ts.LastCDRAttempt AS LastCDRAttempt 
		       , ts.FirstCDRAttempt AS FirstCDRAttempt
		 FROM TrafficStats ts WITH(NOLOCK,index=IX_TrafficStats_DateTimeFirst)  WHERE 
		
	  FirstCDRAttempt >= @FromDate
	       AND FirstCDRAttempt < @TillDate
	       AND ((@CustomerID IS NOT NULL AND ts.CustomerID = @CustomerID) OR (@AllAccounts IS NOT NULL AND ts.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@CustomerID IS NULL AND @AllAccounts IS NULL))
	       AND ((@SupplierID IS NOT NULL AND ts.SupplierID = @SupplierID) OR (@AllAccounts IS NOT NULL AND ts.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@SupplierID IS NULL AND @AllAccounts IS NULL))
	       AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)     
	 ),
	 
	
	Traffic AS (
		 	
		SELECT 
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Day],
			ts.CustomerID as CustomerID,
			Sum(Attempts) as Attempts ,
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/60.0) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then CONVERT(DECIMAL(10,2), Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls)) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)) else NULL end as ACD,
			CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(Attempts)) as DeliveredASR, 
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts
		 FROM Traffic_Stats_Data ts 
			LEFT 	JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID
			 
			 
		 WHERE 
		   (@ZoneID IS NULL OR ts.OurZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		 GROUP BY
			ts.CustomerID,
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)
					
			END
			
	)
			
		SELECT * FROM Traffic 
		ORDER BY [Day]
		
	
	End


	
END