
-- Zone Monitor Stored Procedure
CREATE PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitor_Enhanced_New]
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
				--AND cg.CarrierGroupID = ca.CarrierGroupID
				AND cg.[Path] LIKE (@CarrierGroupPath + '%')
		
		

	Declare @Results TABLE (OurZoneID int , GateWayIn varchar(50),GateWayOut varchar(50), Port_out varchar(50), Port_in varchar(50), SupplierID Varchar (15) ,Attempts int, FailedAttempts int, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), MaxDuration numeric(13,5), LastAttempt datetime, AttemptPercentage numeric(13,5),DurationPercentage numeric(13,5),SuccesfulAttempts int )
	SET NOCOUNT ON

	DECLARE @SwitchConnectivity TABLE (
				CarrierAccount VARCHAR(10),
				SwitchID TINYINT,
				Details NVARCHAR(MAX),
				BeginEffectiveDate SMALLDATETIME,
				EndEffectiveDate SMALLDATETIME,
				GateWayName NVARCHAR(50)
			)

	SET NOCOUNT ON

	INSERT INTO @SwitchConnectivity
	  (
		CarrierAccount,
		SwitchID,
		Details,
		BeginEffectiveDate,
		EndEffectiveDate,
		GateWayName
	  )
	SELECT csc.CarrierAccountID,csc.SwitchID,csc.Details ,csc.BeginEffectiveDate,csc.EndEffectiveDate,csc.[Name]
	FROM   CarrierSwitchConnectivity csc WITH(NOLOCK, INDEX(IX_CSC_CarrierAccount))
	
	-- No Customer, No Supplier
	IF @CustomerID IS NULL AND @SupplierID IS NULL
		INSERT INTO @Results (OurZoneID, GateWayIn,GateWayOut, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,        
				CASE WHEN @GroupByGateWay IN('B','I') THEN cscIn.GateWayName ELSE NULL END AS GateWayIN,
				CASE WHEN @GroupByGateWay IN('B','O') THEN cscOut.GateWayName ELSE NULL END AS GateWayOut,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay !='N')  
					THEN Sum(Attempts)else Sum(NumberOfCalls) END as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay !='N')  THEN
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) 
				else
				case when Sum(NumberOfCalls) > 0 
				then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) 
				ELSE 0 END END as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay !='N')  THEN
				Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
				else
				case when Sum(NumberOfCalls) > 0 
				then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) 
				ELSE 0 END END as DeliveredASR,
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts - SuccessfulAttempts)else Sum(NumberOfCalls - SuccessfulAttempts) END as FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK) 
			    Left JOIN @SwitchConnectivity cscOut
			     ON ( @GroupByGateWay IN('B','O')
					 AND (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%')
					 AND(( @SwitchID IS NULL OR (@SwitchID=cscOut.SwitchID )) AND  TS.SwitchID = cscOut.SwitchID)
					 AND ts.SupplierID  =cscOut.CarrierAccount		
					 AND((cscOut.BeginEffectiveDate >=@FromDateTime AND cscOut.EndEffectiveDate<= @ToDateTime) OR cscOut.EndEffectiveDate IS null))
			    Left JOIN @SwitchConnectivity cscIn 
			     ON ( @GroupByGateWay IN('B','I')
					 AND (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' )
					 AND(( @SwitchID IS NULL   OR (@SwitchID=cscIn.SwitchID)) AND  TS.SwitchID = cscIn.SwitchID)
					 AND ts.CustomerID =cscIn.CarrierAccount		
					 AND((cscIn.BeginEffectiveDate >=@FromDateTime AND cscIn.EndEffectiveDate<= @ToDateTime) OR cscIn.EndEffectiveDate IS null))
				
				LEFT JOIN Zone AS OZ WITH (NOLOCK) ON TS.OurZoneID = OZ.ZoneID
				WHERE			
				FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime
				AND ((@SwitchID IS NULL  AND ts.CustomerID NOT IN (SELECT grasc.CID
				                                                     FROM dbo.GetRepresentedAsSwitchCarriers() grasc )) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
				
			Group By  
				TS.OurZoneID
				, CASE WHEN @GroupByGateWay IN ('B','I')  THEN cscIn.GateWayName ELSE NULL END
				, CASE WHEN @GroupByGateWay IN ('B','O') THEN cscOut.GateWayName  ELSE NULL END
				, CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END
				, CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END
				, CASE WHEN @ShowSupplier = 'Y' THEN TS.SupplierID  ELSE NULL END
	-- Customer, No Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
		INSERT INTO @Results (OurZoneID, GateWayIn,GateWayOut, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,          
				CASE WHEN @GroupByGateWay IN('B','I') THEN cscIn.GateWayName ELSE NULL END AS GateWayIN,
				CASE WHEN @GroupByGateWay IN('B','O') THEN cscOut.GateWayName ELSE NULL END AS GateWayOut,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay != 'N')  THEN Sum(Attempts)else Sum(NumberOfCalls) END as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay != 'N')  THEN
				Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) 
				else
				case when Sum(NumberOfCalls) > 0 
				then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) 
				ELSE 0 END END as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay != 'N')  THEN
				Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
				else
				case when Sum(NumberOfCalls) > 0 
				then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) 
				ELSE 0 END END as DeliveredASR,

				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts - SuccessfulAttempts)else Sum(NumberOfCalls - SuccessfulAttempts) END as FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK) 
				 Left JOIN @SwitchConnectivity cscOut
			     ON ( @GroupByGateWay IN('B','O')
					 AND (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%')
					 AND(( @SwitchID IS NULL OR (@SwitchID=cscOut.SwitchID )) AND  TS.SwitchID = cscOut.SwitchID)
					 AND ts.SupplierID  =cscOut.CarrierAccount		
					 AND((cscOut.BeginEffectiveDate >=@FromDateTime AND cscOut.EndEffectiveDate<= @ToDateTime) OR cscOut.EndEffectiveDate IS null))
			    Left JOIN @SwitchConnectivity cscIn 
			     ON ( @GroupByGateWay IN('B','I')
					 AND (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' )
					 AND(( @SwitchID IS NULL   OR (@SwitchID=cscIn.SwitchID)) AND  TS.SwitchID = cscIn.SwitchID)
					 AND ts.CustomerID =cscIn.CarrierAccount		
					 AND((cscIn.BeginEffectiveDate >=@FromDateTime AND cscIn.EndEffectiveDate<= @ToDateTime) OR cscIn.EndEffectiveDate IS null))
				 
				 LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				 WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND TS.CustomerID = @CustomerID 
				AND ((@SwitchID IS NULL  AND ts.CustomerID NOT IN (SELECT grasc.CID
				                                                     FROM dbo.GetRepresentedAsSwitchCarriers() grasc )) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
				
			Group By 
				TS.OurZoneID , 
				CASE WHEN @GroupByGateWay IN ('B','I')  THEN cscIn.GateWayName ELSE NULL END,
				CASE WHEN @GroupByGateWay IN ('B','O') THEN cscOut.GateWayName  ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END
	-- No Customer, Supplier
	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
		INSERT INTO @Results (OurZoneID, GateWayIn,GateWayOut, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,          
				CASE WHEN @GroupByGateWay IN('B','I') THEN cscIn.GateWayName ELSE NULL END AS GateWayIN,
				CASE WHEN @GroupByGateWay IN('B','O') THEN cscOut.GateWayName ELSE NULL END AS GateWayOut,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				case when Sum(Attempts) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 end as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				case when Sum(Attempts) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(Attempts) ELSE 0 end as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK) 
				Left JOIN @SwitchConnectivity cscOut
			     ON ( @GroupByGateWay IN('B','O')
					 AND (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%')
					 AND(( @SwitchID IS NULL OR (@SwitchID=cscOut.SwitchID )) AND  TS.SwitchID = cscOut.SwitchID)
					 AND ts.SupplierID  =cscOut.CarrierAccount		
					 AND((cscOut.BeginEffectiveDate >=@FromDateTime AND cscOut.EndEffectiveDate<= @ToDateTime) OR cscOut.EndEffectiveDate IS null))
			    Left JOIN @SwitchConnectivity cscIn 
			     ON ( @GroupByGateWay IN('B','I')
					 AND (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' )
					 AND(( @SwitchID IS NULL   OR (@SwitchID=cscIn.SwitchID)) AND  TS.SwitchID = cscIn.SwitchID)
					 AND ts.CustomerID =cscIn.CarrierAccount		
					 AND((cscIn.BeginEffectiveDate >=@FromDateTime AND cscIn.EndEffectiveDate<= @ToDateTime) OR cscIn.EndEffectiveDate IS null))
				LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
			WHERE 
					FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime  
				AND TS.SupplierID = @SupplierID 
				AND ((@SwitchID IS NULL  AND ts.CustomerID NOT IN (SELECT grasc.CID
				                                                     FROM dbo.GetRepresentedAsSwitchCarriers() grasc )) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
				
			Group By 
				TS.OurZoneID, 
				CASE WHEN @GroupByGateWay IN ('B','I')  THEN cscIn.GateWayName ELSE NULL END,
				CASE WHEN @GroupByGateWay IN ('B','O') THEN cscOut.GateWayName  ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END
	-- Customer, Supplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL
		INSERT INTO @Results (OurZoneID, GateWayIn,GateWayOut, Port_out, Port_in, SupplierID ,Attempts, DurationsInMinutes ,ASR,ACD,DeliveredASR,AveragePDD,MaxDuration,LastAttempt,AttemptPercentage,DurationPercentage,SuccesfulAttempts, FailedAttempts)
			SELECT 
				TS.OurZoneID,          
				CASE WHEN @GroupByGateWay IN('B','I') THEN cscIn.GateWayName ELSE NULL END AS GateWayIN,
				CASE WHEN @GroupByGateWay IN('B','O') THEN cscOut.GateWayName ELSE NULL END AS GateWayOut,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS PortOut,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS PortIn,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
				Sum(Attempts) as Attempts,
				Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
				case when Sum(Attempts) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) ELSE 0 end as ASR,
				case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
				case when Sum(Attempts) > 0 then Sum(deliveredAttempts) * 100.0 / SUM(Attempts) ELSE 0 end as DeliveredASR, 
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				0,0,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficStats TS WITH(NOLOCK) 
				Left JOIN @SwitchConnectivity cscOut
			     ON ( @GroupByGateWay IN('B','O')
					 AND (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%')
					 AND(( @SwitchID IS NULL OR (@SwitchID=cscOut.SwitchID )) AND  TS.SwitchID = cscOut.SwitchID)
					 AND ts.SupplierID  =cscOut.CarrierAccount		
					 AND((cscOut.BeginEffectiveDate >=@FromDateTime AND cscOut.EndEffectiveDate<= @ToDateTime) OR cscOut.EndEffectiveDate IS null))
			    Left JOIN @SwitchConnectivity cscIn 
			     ON ( @GroupByGateWay IN('B','I')
					 AND (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' )
					 AND(( @SwitchID IS NULL   OR (@SwitchID=cscIn.SwitchID)) AND  TS.SwitchID = cscIn.SwitchID)
					 AND ts.CustomerID =cscIn.CarrierAccount		
					 AND((cscIn.BeginEffectiveDate >=@FromDateTime AND cscIn.EndEffectiveDate<= @ToDateTime) OR cscIn.EndEffectiveDate IS null))
				
				LEFT JOIN Zone AS OZ WITH (nolock) ON TS.OurZoneID = OZ.ZoneID
				WHERE 
					FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime  
				AND TS.CustomerID = @CustomerID 
				AND TS.SupplierID = @SupplierID 
				AND ((@SwitchID IS NULL  AND ts.CustomerID NOT IN (SELECT grasc.CID
				                                                     FROM dbo.GetRepresentedAsSwitchCarriers() grasc )) OR TS.SwitchID = @SwitchID)
				AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
				AND TS.CustomerID IN (SELECT * FROM @FilteredCustomers)
				
			Group By 
				TS.OurZoneID ,
				CASE WHEN @GroupByGateWay IN ('B','I')  THEN cscIn.GateWayName ELSE NULL END,
				CASE WHEN @GroupByGateWay IN ('B','O') THEN cscOut.GateWayName  ELSE NULL END, 
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END,
				CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END

	--Percentage For Attempts-----
	Declare @TotalAttempts bigint
	SELECT  @TotalAttempts = SUM(Attempts) FROM @Results
	Update  @Results SET AttemptPercentage= CASE WHEN @TotalAttempts>0 THEN (Attempts * 100. / @TotalAttempts) ELSE 0.0 END 

	SELECT * from @Results Order By Attempts DESC ,DurationsInMinutes DESC
END