

CREATE PROCEDURE [dbo].[sp_Zebra_TrafficStats_ByPeriods]
@PeriodType varchar(50) = 'Days',
@ZoneID INT = NULL ,
@SupplierID VarChar(MAX) = NULL ,
@CustomerID Varchar(300) = NULL ,
@CodeGroup varchar(10) = NULL ,
@FromDate DATETIME ,
@TillDate DATETIME,
@AllAccounts VARCHAR(MAX) = NULL,
@From INT,
@To INT

AS
DECLARE @SQLString nvarchar(4000)
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
	       AND  ((@SupplierID IS NOT NULL AND ts.SupplierID IN (select * from dbo.fn_ado_param(@SupplierID,','))) OR (@AllAccounts IS NOT NULL AND ts.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@SupplierID IS NULL AND @AllAccounts IS NULL))
	       AND (TS.CustomerID is null or TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc))  
	 ) ,
	 
	 
	 
	 
	 


	Traffic AS (

		SELECT 
			SupplierZoneID,
			--SuccessfulAttempts,
			--DeliveredAttempts,
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Day],
			Sum(Attempts) as Attempts ,
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/60.0) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls)) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)) else NULL end as ACD,
			case when  SUM(Attempts)>0 then  CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(Attempts)) else 0 end as DeliveredASR, 
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts,
			case when Sum(Attempts) > 0 then CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(Attempts)) ELSE 0 end as NER,
			ts.SupplierID as SupplierID
		 FROM Traffic_Stats_Data ts 

		GROUP BY
			SupplierID, SupplierZoneID,

			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)

			END
			)

		SELECT T.*,Z.Name,P.Name+ '(' + C.NameSuffix + ')' as SupplierName INTO #RESULT FROM Traffic T 
		inner join Zone Z on T.SupplierZoneID=Z.ZoneID
		inner join CarrierAccount C on C.CarrierAccountID=T.SupplierID
		inner join CarrierProfile P on C.ProfileID=P.ProfileID
		ORDER BY [Day]

		set @SQLString = '
			select R.*, (Attempts-SuccessfulAttempts) as FailedAttempts
			INTO #result2 From #RESULT R
			where SupplierZoneID IS NOT NULL
			'
			--select * from ' + @tempTableName

	--	execute sp_executesql @SQLString


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
	       AND ((@SupplierID IS NOT NULL AND ts.SupplierID IN (select * from dbo.fn_ado_param(@SupplierID,','))) OR (@AllAccounts IS NOT NULL AND ts.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts))) OR (@SupplierID IS NULL AND @AllAccounts IS NULL))
	       AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)     
	 ),


	Traffic AS (

		SELECT 
			SupplierZoneID,
			--SuccessfulAttempts,
			--DeliveredAttempts,
 			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121) 
			END AS [Day],
			Sum(Attempts) as Attempts ,
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/60.0) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then CONVERT(DECIMAL(10,2), Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls)) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)/Sum(SuccessfulAttempts)) else NULL end as ACD,
			case when SUM(Attempts)>0 then CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(Attempts)) else 0 end as DeliveredASR, 
			--PDDinSeconds as PDD,
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts) as SuccessfulAttempts,
			ts.SupplierID as SupplierID

		 FROM Traffic_Stats_Data ts		
			LEFT 	JOIN Zone OZ WITH(nolock) on TS.OurZoneID=OZ.ZoneID


		 WHERE 
		   (@ZoneID IS NULL OR ts.OurZoneID= @ZoneID) 
			AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		 GROUP BY
			ts.SupplierID, SupplierZoneID,
			CASE 
				WHEN @PeriodType like 'Days' THEN CONVERT(varchar(10), TS.FirstCDRAttempt, 121) 
				WHEN @PeriodType like 'Weeks' THEN 'Week ' + cast(datepart(wk,  TS.FirstCDRAttempt) AS varchar)
				WHEN @PeriodType like 'Months' THEN CONVERT(varchar(7), TS.FirstCDRAttempt, 121)

			END

	)

		SELECT T.*,Z.Name,P.Name+ '(' + C.NameSuffix + ')' as SupplierName INTO #RESULT1 FROM  Traffic T 
		inner join Zone Z on T.SupplierZoneID=Z.ZoneID
		inner join CarrierAccount C on C.CarrierAccountID=T.SupplierID
		inner join CarrierProfile P on C.ProfileID=P.ProfileID
		ORDER BY [Day]

		set @SQLString = '
			select R.*, (R.Attempts-R.SuccessfulAttempts) as FailedAttempts
			INTO #result2 From #RESULT1 R
			where SupplierZoneID IS NOT NULL
		'


	End
END

SET @SQLString = @SQLString + '
	select count(1) from #result2
	 ;with FINAL AS 
                        (
                        select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                        from #result2
                         )
						SELECT * FROM FINAL '--WHERE rowNumber  between +CAST( @From AS varchar) +' AND '+CAST( @To as varchar)

	 execute sp_executesql @SQLString