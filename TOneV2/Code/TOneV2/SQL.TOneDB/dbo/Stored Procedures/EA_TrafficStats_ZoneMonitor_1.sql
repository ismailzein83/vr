
-- Stored Procedure

CREATE  PROCEDURE [dbo].[EA_TrafficStats_ZoneMonitor]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10) = NULL,
    @SwitchID	  tinyInt = NULL,   
    @CodeGroup varchar(10) = NULL,
    @AllAccounts varchar(MAX) = NULL
WITH RECOMPILE
AS	
BEGIN
	
	Declare @ParsedStringResults TABLE ( ParsedString VARCHAR(200))
	INSERT INTO @ParsedStringResults SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)
	Declare @Results TABLE ( AttemptDateTime DATETIME, OurZoneID INT ,Attempts int, FailedAttempts int, DurationsInMinutes numeric(13,5), CeiledDuration numeric(13,5), ASR numeric(13,5), ACD numeric(13,5), DeliveredAttempts INT, DeliveredASR numeric(13,5), AveragePDD numeric(13,5),PGAD numeric (19,5),ABR DECIMAL(10,2), MaxDuration numeric(13,5), LastAttempt datetime, AttemptPercentage numeric(13,5),DurationPercentage numeric(13,5),SuccesfulAttempts int, ZoneName nvarchar(250) )
	SET NOCOUNT ON
	-- Customer, No Supplier
	IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
		INSERT INTO @Results (AttemptDateTime ,OurZoneID,Attempts, DurationsInMinutes, CeiledDuration ,ASR,ACD, deliveredAttempts,DeliveredASR,AveragePDD,PGAD,ABR,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts, ZoneName)
			SELECT 
			    DATEADD(day,0,datediff(day,0, ts.FirstCDRAttempt)) AS AttemptDateTime,
				--min(ts.FirstCDRAttempt) AS AttemptDateTime,
				TS.OurZoneID,          
				Sum(attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				SUM(ceiledDuration)/60.0 AS CeiledDuration,
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) AS deliveredAttempts, 
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				isnull(AVG(ts.PGAD),0) AS PGAD,
				case when (Sum(TS.Attempts)-isnull(sum(TS.ReleaseSourceS),0)) > 0  
                                Then isnull(CONVERT(DECIMAL(10,2),(Sum(TS.SuccessfulAttempts ))*100.00 / (Sum(TS.Attempts)-isnull(sum(TS.ReleaseSourceS),0))),0)
                                ELSE 0 END ABR,
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
				,oz.Name as ZoneName
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst)) 
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			 LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				WHERE 
					FirstCDRAttempt >= @FromDateTime AND FirstCDRAttempt<@toDateTime
							AND  TS.CustomerID IN(SELECT ParsedString FROM @ParsedStringResults) 
				AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				
			Group By 
				DATEADD(day,0,datediff(day,0, ts.FirstCDRAttempt)), 
				TS.OurZoneID , 	OZ.Name
				order by OZ.Name asc

	-- No Customer, Supplier
	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
		INSERT INTO @Results (AttemptDateTime, OurZoneID, Attempts, DurationsInMinutes, CeiledDuration ,ASR,ACD,DeliveredAttempts,DeliveredASR,AveragePDD,PGAD,ABR,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts ,ZoneName)
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
				AVG(ts.PGAD) AS PGAD, 
				case when (Sum(TS.Attempts)-isnull(sum(TS.ReleaseSourceS),0)) > 0  
                                Then CONVERT(DECIMAL(10,2),(Sum(TS.SuccessfulAttempts ))*100.00 / (Sum(TS.Attempts)-isnull(sum(TS.ReleaseSourceS),0)))
                                ELSE 0 END ABR,
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
				,oz.Name as ZoneName
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier)) 
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			 LEFT JOIN Zone AS OZ WITH (nolock) ON TS.SupplierZoneID = OZ.ZoneID
			
				WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND TS.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) 
				--AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			Group By 
			    --dbo.DateOf(ts.FirstCDRAttempt),
				TS.SupplierZoneID,oz.Name

	--Percentage For Attempts-----
	Declare @TotalAttempts bigint
	SELECT  @TotalAttempts = SUM(Attempts) FROM @Results
	Update  @Results SET AttemptPercentage= (Attempts * 100. / @TotalAttempts)

	SELECT * from @Results Order By Attempts DESC ,DurationsInMinutes DESC

END