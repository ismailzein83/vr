

CREATE  PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitorSupplierZoneDetails]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10)=null,
	@SupplierID   varchar(10)=null,
    @OurZoneID 	  INT,
	@SwitchID	  tinyInt = NULL,
	@CodeGroup VARCHAR(20) = NULL,
	@Port VARCHAR(1)=null,
    @PortInValue VARCHAR(21)=null,
    @PortOutValue VARCHAR(21)=null
	
AS
	SET NOCOUNT ON
	
	
if @CustomerID is null and @SupplierID IS NULL
    select 
		Ts.SupplierZoneID as SupplierZoneID,
		TS.SupplierID AS SupplierID,
		Sum(attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
		  
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR,
		  
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
	   Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK)
    LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
	WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND ts.CustomerID NOT in  (SELECT grasc.CID AS CarrierAccountID
                    FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
                    --		AND ts.supplierid NOT IN  (SELECT grasc.CID AS CarrierAccountID
                    --FROM dbo.GetRepresentedAsSwitchCarriers() grasc) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND ( @Port IS null 
		OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
		Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
		Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		TS.SupplierZoneID,
		Ts.SupplierID
	ORDER by Attempts desc

if @CustomerID is NOT null and @SupplierID IS NULL
    select 
		Ts.SupplierZoneID as SupplierZoneID,
		TS.SupplierID AS SupplierID,
		Sum(NumberOfCalls) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end  as DeliveredASR,
		 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
	   Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_Customer))
    LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
	WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
                    		AND ts.supplierid NOT IN  (SELECT grasc.CID AS CarrierAccountID
                    FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
		AND (TS.CustomerID = @CustomerID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND ( @Port IS null 
		OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
		Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
		Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		 TS.SupplierZoneID,
		 Ts.SupplierID
	ORDER by Attempts desc

if @CustomerID is null and @SupplierID IS NOT NULL
    select 
		Ts.SupplierZoneID as SupplierZoneID,
		TS.SupplierID AS SupplierID,
		Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
	   Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_Supplier))
    LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
	WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
			AND ts.CustomerID NOT IN  (SELECT grasc.CID AS CarrierAccountID
                    FROM dbo.GetRepresentedAsSwitchCarriers() grasc)
           
		AND (TS.SupplierID = @SupplierID) 
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		--AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND ( @Port IS null 
		OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
		Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
		Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		TS.SupplierZoneID,
		Ts.SupplierID
	ORDER by Attempts desc

if @CustomerID is NOT null and @SupplierID IS NOT NULL
    select 
		Ts.SupplierZoneID as SupplierZoneID,
		TS.SupplierID AS SupplierID,
		Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds/60.0)/Sum(SuccessfulAttempts) else NULL end as ACD,
		case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR, 
		Avg(PDDinSeconds) as AveragePDD ,MAX(MaxDurationInSeconds/60.) as MaxDuration,
        Max(LastCDRAttempt) as LastAttempts,
        1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
	   Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
    LEFT JOIN Zone OZ ON TS.OurZoneID = OZ.ZoneID	
	WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		AND (TS.CustomerID =@CustomerID)
		AND (TS.SupplierID = @SupplierID)
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND ( @Port IS null 
		OR  ((@Port='I') AND  (ts.Port_IN=@PortInValue))
		Or (( @Port='O') AND( ts.Port_OUT=@PortOutValue))
		Or  (( @Port='B') AND( ts.Port_IN=@PortInValue AND  ts.Port_OUT=@PortOutValue)))
    Group by 
		Ts.SupplierZoneID,
		TS.SupplierID
	ORDER by Attempts desc