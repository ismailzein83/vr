




CREATE PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitorDetails_Enhanced]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10)=null,
	@SupplierID   varchar(10)=null,
	@GroupingField varchar(10)='CUSTOMERS',
    @OurZoneID 	  INT,
	@SwitchID	  tinyInt = NULL,
	@CodeGroup VARCHAR(20) = NULL,
	@Port VARCHAR(1)=null,
    @PortInValue VARCHAR(21)=null,
    @PortOutValue VARCHAR(21)=null
	
AS
	SET NOCOUNT ON
	
	IF @CustomerID IS NOT NULL SET @GroupingField = 'SUPPLIERS'
	IF @SupplierID IS NOT NULL SET @GroupingField = 'CUSTOMERS'
	

 
if @CustomerID is null and @SupplierID IS NULL
    select 
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(deliveredAttempts)*100.0 / Sum(Attempts) END as DeliveredASR,

 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls  - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK)
         LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
		 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
		 LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL ) OR (TS.SwitchID = @SwitchID AND CA.RepresentsASwitch='N' ))
		AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
	AND ( @Port IS null 
	OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
	Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
	Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
	
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc

if @CustomerID is NOT null and @SupplierID IS NULL
    select 
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(deliveredAttempts)*100.0 / Sum(Attempts) END as DeliveredASR,


		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK)
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.CustomerID = @CustomerID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
	AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
	AND ( @Port IS null 
	OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
	Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
	Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc

if @CustomerID is null and @SupplierID IS NOT NULL
    select 
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID ELSE TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		

		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(deliveredAttempts)*100.0 / Sum(Attempts) END as DeliveredASR,

		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK)
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
		AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
	AND ( @Port IS null 
	OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
	Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
	Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE Ts.SupplierID END)
	ORDER by Attempts desc

if @CustomerID is NOT null and @SupplierID IS NOT NULL
    select 
		CASE WHEN @GroupingField = 'CUSTOMERS' then CustomerID else TS.SupplierID end as CarrierAccountID,
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls) ELSE SUM(Attempts) END as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) END as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		

		CASE WHEN @GroupingField = 'CUSTOMERS' THEN case when Sum(NumberOfCalls) > 0 
		then Sum(deliveredAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 END 
		ELSE Sum(deliveredAttempts)*100.0 / Sum(Attempts) END as DeliveredASR,

		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK)
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
			 LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID		
WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.SupplierID = @SupplierID) 
		AND (TS.OurZoneID = @OurZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
	AND ( @Port IS null 
	OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
	Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
	Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc