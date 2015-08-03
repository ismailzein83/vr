

-- Zone Monitor Stored Procedure
CREATE    PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitor_Enhanced]
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
	SELECT @CarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = @CarrierGroupID
	
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

	-- get all traffic to a cte on predefined criterias 
	;WITH 
	 RepresentedAsSwitchCarriers AS 
	 (
	 	SELECT grasc.CID AS CarrierAccountID
            FROM dbo.GetRepresentedAsSwitchCarriers() grasc   
	 )
	 , FilteredCustomers AS 
	 (
	 	SELECT  CarrierAccountID FROM @FilteredCustomers
	 	)
	 ,Traffic_Stats_Data AS (
		SELECT 
		         ts.SwitchId AS SwitchID 
		       , ts.Port_IN AS Port_In
		       , ts.Port_OUT AS Port_Out
		       , ts.CustomerID AS CustomerID
		       , ts.OurZoneID AS OurZoneID
		       , ts.SupplierID AS SupplierID
		       , ts.SupplierZoneID AS SupplierZoneID
		       , ts.FirstCDRAttempt AS FirstCDRAttempt
		       , ts.Attempts AS Attempts
		       , ts.DeliveredAttempts AS DeliveredAttempts 
		       , ts.DeliveredNumberOfCalls AS DeliveredNumberOfCalls
		       , ts.SuccessfulAttempts AS SuccessfulAttempts
		       , ts.DurationsInSeconds AS DurationsInSeconds
		       , ts.PDDInSeconds AS PDDInSeconds
		       , ts.MaxDurationInSeconds AS MaxDurationInSeconds
		       , ts.NumberOfCalls AS NumberOfCalls
		       , ts.LastCDRAttempt AS LastCDRAttempt 
		FROM TrafficStats ts WITH(NOLOCK)  
		WHERE 
			FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime
			AND (@CustomerID IS NULL OR ts.CustomerID = @CustomerID)
			AND (@SupplierID IS NULL OR ts.SupplierID = @SupplierID)
	 )
	, Traffic_Stats AS 
	
	(
		SELECT 
		         ts.SwitchId AS SwitchID 
		       , ts.Port_IN AS Port_In
		       , ts.Port_OUT AS Port_Out
		       , ts.CustomerID AS CustomerID
		       , ts.OurZoneID AS OurZoneID
		       , ts.SupplierID AS SupplierID
		       , ts.SupplierZoneID AS SupplierZoneID
		       , Sum(ts.Attempts) AS Attempts
		       , Sum(ts.DeliveredAttempts) AS DeliveredAttempts 
		       , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
		       , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
		       , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
		       , AVG(ts.PDDInSeconds) AS PDDInSeconds
		       , Max(ts.MaxDurationInSeconds) AS MaxDurationInSeconds
		       , Sum(ts.NumberOfCalls) AS NumberOfCalls
		       , Max(ts.LastCDRAttempt) AS LastCDRAttempt 
		 FROM Traffic_Stats_Data ts WITH(NOLOCK)

		GROUP BY ts.SwitchId 
		       , ts.Port_IN 
		       , ts.Port_OUT 
		       , ts.CustomerID 
		       , ts.OurZoneID 
		       , ts.SupplierID 
		       , ts.SupplierZoneID 
	)
	, 
	Traffic AS 
	(      SELECT 
		        ts.SwitchId AS SwitchID 
		       , ts.Port_IN AS Port_In
		       , ts.Port_OUT AS Port_Out
		       , ts.CustomerID AS CustomerID
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
		       ,ts.LastCDRAttempt AS LastCDRAttempt 
		 FROM Traffic_Stats ts WITH(NOLOCK)	  
     WHERE  
 ((@SwitchID IS NULL AND  NOT EXISTS (SELECT * FROM RepresentedAsSwitchCarriers grasc WHERE grasc.CarrierAccountID = TS.CustomerID )) OR TS.SwitchID = @SwitchID)
AND EXISTS  (SELECT * FROM FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID )

	)
		--SELECT COUNT(*) FROM Traffic

	,OurZOnes AS 
	(
		SELECT z.ZoneID AS ZoneID 
		     , z.Name AS ZoneName 
		     , z.CodeGroup AS CodeGroup 
		FROM Zone z WITH(NOLOCK) 
		WHERE z.SupplierID='SYS' 
		AND (z.BeginEffectiveDate <= @FromDateTime OR z.BeginEffectiveDate BETWEEN @FromDateTime AND @ToDateTime) 
	)
	-- Get Carrier Switch connectivity 
	, SwitchConnectivity AS 
	(
		SELECT csc.CarrierAccountID AS  CarrierAccount 
			  ,csc.SwitchID AS SwitchID 
			  ,csc.Details AS Details
			  ,csc.BeginEffectiveDate AS BeginEffectiveDate
			  ,csc.EndEffectiveDate AS EndEffectiveDate
			  ,csc.[Name] AS GateWayName
	     FROM   CarrierSwitchConnectivity csc WITH(NOLOCK)--, INDEX(IX_CSC_CarrierAccount))
		WHERE ((csc.BeginEffectiveDate >=@FromDateTime AND csc.EndEffectiveDate<= @ToDateTime) OR csc.EndEffectiveDate IS null)
				
	
	)
	--,final AS 
	--(
	--	SELECT ourzoneid ,SUM(attempts) AS s
	--	FROM traffic GROUP BY ourzoneid 
	--	)
	, FinalTrafficStats AS 
	(
		SELECT 
			    TS.OurZoneID AS OurZoneID,        
				CASE WHEN @GroupByGateWay IN('B','I') THEN cscIn.GateWayName ELSE NULL END AS GateWayIn,
				CASE WHEN @GroupByGateWay IN('B','O') THEN cscOut.GateWayName ELSE NULL END AS GateWayOut,
				CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS Port_Out,
				CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS Port_In,
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
				case when Sum(NumberOfCalls) > 0 AND @SupplierID IS NOT NULL then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls)
				    when Sum(NumberOfCalls) > 0 AND @SupplierID IS NULL then Sum(DeliveredNumberOfCalls)*100.0 / Sum(NumberOfCalls) 
				ELSE 0 END END as DeliveredASR,
				Avg(PDDinSeconds) as AveragePDD, 
				Max (MaxDurationInSeconds)/60.0 as MaxDuration,
				Max(LastCDRAttempt) as LastAttempt,
				--0 AS ,0,
				Sum(SuccessfulAttempts)AS SuccesfulAttempts,
				CASE WHEN (@ShowSupplier='Y' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts - SuccessfulAttempts)else Sum(NumberOfCalls - SuccessfulAttempts) END as FailedAttempts
			FROM Traffic TS WITH(NOLOCK)-- ,INDEX(IX_TrafficStats_DateTimeFirst)) 
			    Left JOIN SwitchConnectivity cscOut
			     ON ( @GroupByGateWay IN('B','O')
					 AND (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%')
					 AND(( @SwitchID IS NULL OR (@SwitchID=cscOut.SwitchID )) AND  TS.SwitchID = cscOut.SwitchID)
					 AND ts.SupplierID  =cscOut.CarrierAccount		)
			   Left JOIN SwitchConnectivity cscIn 
			     ON ( @GroupByGateWay IN('B','I')
					 AND (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' )
					 AND(( @SwitchID IS NULL   OR (@SwitchID=cscIn.SwitchID)) AND  TS.SwitchID = cscIn.SwitchID)
					 AND ts.CustomerID =cscIn.CarrierAccount		)
				LEFT JOIN OurZones AS OZ WITH (NOLOCK) ON TS.OurZoneID = OZ.ZoneID
	WHERE 	(@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
			Group By  
				TS.OurZoneID
				, CASE WHEN @GroupByGateWay IN ('B','I')  THEN cscIn.GateWayName ELSE NULL END
				, CASE WHEN @GroupByGateWay IN ('B','O') THEN cscOut.GateWayName  ELSE NULL END
				, CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END
				, CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END
				, CASE WHEN @ShowSupplier = 'Y' THEN TS.SupplierID  ELSE NULL END
		
	) 
	SELECT * FROM FinalTrafficStats
	
	
	END