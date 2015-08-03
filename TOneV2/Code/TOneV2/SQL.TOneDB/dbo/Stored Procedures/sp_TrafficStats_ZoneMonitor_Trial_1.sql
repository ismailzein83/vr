



-- Zone Monitor Stored Procedure
CREATE  PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitor_Trial]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10) = NULL,
    @SwitchID	  tinyInt = NULL,   
	@ShowE1       char(1) = 'N',
	@GroupByGateWay char(1) = 'N',
	@ShowSupplier Char(1)='N',
    @CodeGroup varchar(10) = NULL,
    @CarrierGroupID INT = NULL
  
WITH RECOMPILE
AS	
BEGIN
	
	DECLARE @CarrierGroupPath VARCHAR(255)
	SELECT @CarrierGroupPath = cg.[Path] FROM CarrierGroup cg WHERE cg.CarrierGroupID = @CarrierGroupID
	
	DECLARE @FilteredCustomers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
	
	IF @CarrierGroupPath IS NULL
		INSERT INTO @FilteredCustomers SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = 'N'
	ELSE
		INSERT INTO @FilteredCustomers 
		SELECT DISTINCT ca.CarrierAccountID 
				FROM CarrierAccount ca WITH(NOLOCK)
				LEFT JOIN CarrierGroup cg  WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,','))
			WHERE
					ca.IsDeleted = 'N'
				AND cg.[Path] LIKE (@CarrierGroupPath + '%')
			--SELECT DISTINCT ca.CarrierAccountID 
			--	FROM CarrierAccount ca, CarrierGroup cg  
			--WHERE
			--		ca.IsDeleted = 'N'
			--	AND cg.CarrierGroupID = ca.CarrierGroupID
			--	AND cg.[Path] LIKE (@CarrierGroupPath + '%')

	Declare @Results TABLE (OurZoneID int , GateWay varchar(50), Port_out varchar(50), Port_in varchar(50), SupplierID Varchar (15) ,Attempts int, FailedAttempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), MaxDuration numeric(13,5), LastAttempt datetime, AttemptPercentage numeric(13,5),DurationPercentage numeric(13,5),SuccesfulAttempts int )
	SET NOCOUNT ON

	-- No Customer, No Supplier
	IF @CustomerID IS NULL AND @SupplierID IS NULL
		INSERT INTO @Results (OurZoneID, GateWay, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,        
				CASE WHEN @GroupByGateWay = 'Y' THEN csc.[Name]  ELSE NULL END AS GateWay,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  
					THEN Sum(Attempts)else Sum(NumberOfCalls) END as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) 
				else
				case when Sum(NumberOfCalls) > 0 
				then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) 
				ELSE 0 END END as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) * 100.0 / Sum(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts - SuccessfulAttempts)else Sum(NumberOfCalls - SuccessfulAttempts) END as FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst)) 
			    Left JOIN CarrierSwitchConnectivity csc  WITH(NOLOCK, INDEX(IX_CSC_CarrierAccount)) ON TS.SupplierID = csc.CarrierAccountID AND TS.SwitchID = csc.SwitchID AND  (','+csc.Details+',' LIKE '%,'+ts.Port_OUT+',%' )
				AND((csc.BeginEffectiveDate>=@FromDateTime AND csc.EndEffectiveDate<=@ToDateTime) OR csc.EndEffectiveDate IS null)
				LEFT JOIN Zone AS OZ WITH (NOLOCK) ON TS.OurZoneID = OZ.ZoneID
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
				LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
				WHERE			
				FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
				
			Group By  
				TS.OurZoneID
				, CASE WHEN @GroupByGateWay = 'Y' THEN csc.[Name] ELSE NULL END
				, CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END
				, CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END
				, CASE WHEN @ShowSupplier = 'Y' THEN TS.SupplierID  ELSE NULL END
	-- Customer, No Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
		INSERT INTO @Results (OurZoneID, GateWay, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,          
				CASE WHEN @GroupByGateWay = 'Y' THEN csc.[Name] ELSE NULL END AS GateWay,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts)else Sum(NumberOfCalls) END as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) 
				else
				case when Sum(NumberOfCalls) > 0 
				then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) 
				ELSE 0 END END as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts - SuccessfulAttempts)else Sum(NumberOfCalls - SuccessfulAttempts) END as FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer)) 
				 LEFT JOIN CarrierSwitchConnectivity csc WITH(NOLOCK, INDEX(IX_CSC_CarrierAccount)) ON TS.SupplierID = csc.CarrierAccountID AND TS.SwitchID = csc.SwitchID  AND  (','+csc.Details+',' LIKE '%,'+ts.Port_IN+',%' )
				 AND((csc.BeginEffectiveDate>=@FromDateTime AND csc.EndEffectiveDate<=@ToDateTime) OR csc.EndEffectiveDate IS null)
				 LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				 LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
				 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
				WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND TS.CustomerID = @CustomerID 
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
				
			Group By 
				TS.OurZoneID , 
				CASE WHEN @GroupByGateWay = 'Y' THEN csc.[Name] ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END
	-- No Customer, Supplier
	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
		INSERT INTO @Results (OurZoneID, GateWay, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,          
				CASE WHEN @GroupByGateWay = 'Y' THEN csc.[Name] ELSE NULL END AS GateWay,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 end as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier)) 
				LEFT JOIN CarrierSwitchConnectivity csc  WITH(NOLOCK, INDEX(IX_CSC_CarrierAccount)) ON TS.SupplierID = csc.CarrierAccountID AND TS.SwitchID = csc.SwitchID AND  (','+csc.Details+',' LIKE '%,'+ts.Port_OUT+',%' )
				AND((csc.BeginEffectiveDate>=@FromDateTime AND csc.EndEffectiveDate<=@ToDateTime) OR csc.EndEffectiveDate IS NULL)				
				LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
				LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
				WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND TS.SupplierID = @SupplierID 
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
				
			Group By 
				TS.OurZoneID, 
				CASE WHEN @GroupByGateWay = 'Y' THEN csc.[Name] ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END
	-- Customer, Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL
		INSERT INTO @Results (OurZoneID, GateWay, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,          
				CASE WHEN @GroupByGateWay = 'Y' THEN csc.[Name] ELSE NULL END AS GateWay,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 end as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier)) 
				LEFT JOIN CarrierSwitchConnectivity csc WITH(NOLOCK, INDEX(IX_CSC_CarrierAccount)) ON TS.SupplierID = csc.CarrierAccountID AND TS.SwitchID = csc.SwitchID  AND  (','+csc.Details+',' LIKE '%,'+ts.Port_OUT+',%' ) 
				AND((csc.BeginEffectiveDate>=@FromDateTime AND csc.EndEffectiveDate<=@ToDateTime) OR csc.EndEffectiveDate IS NULL)
				LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
				LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
				WHERE 
					FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime  
				AND TS.CustomerID = @CustomerID 
				AND TS.SupplierID = @SupplierID 
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
				
			Group By 
				TS.OurZoneID ,
				CASE WHEN @GroupByGateWay = 'Y' THEN csc.[Name] ELSE NULL END, 
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END

	--Percentage For Attempts-----
	Declare @TotalAttempts bigint
	SELECT  @TotalAttempts = SUM(Attempts) FROM @Results
	Update  @Results SET AttemptPercentage= CASE WHEN @TotalAttempts>0 THEN (Attempts * 100. / @TotalAttempts) ELSE 0.0 END 

	SELECT * from @Results Order By Attempts DESC ,DurationsInMinutes DESC
END