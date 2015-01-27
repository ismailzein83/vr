﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_TrafficStats_OriginatingZoneMonitorDetails_Enhanced] 
	-- Add the parameters for the stored procedure here
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10)=null,
	@SupplierID   varchar(10)=null,
	@GroupingField varchar(10)='CUSTOMERS',
    @OriginatingZoneID 	  INT,
	@SwitchID	  tinyInt = NULL,
	@CodeGroup VARCHAR(20) = NULL
	
AS
	SET NOCOUNT ON
				
	IF @CustomerID IS NOT NULL SET @GroupingField = 'SUPPLIERS'
	IF @SupplierID IS NOT NULL SET @GroupingField = 'CUSTOMERS'
	
	
	
if @CustomerID is null and @SupplierID IS NULL
BEGIN
	
	;WITH		 
Traffic_Stats_Data AS (
		 select 
	    CustomerID  as CustomerID,
	    SupplierID  as SupplierID,
		NumberOfCalls AS NumberOfCalls,
		Attempts  as Attempts,
		DurationsInSeconds AS  DurationsInSeconds, 
		SuccessfulAttempts AS SuccessfulAttempts,
		deliveredAttempts AS deliveredAttempts,
		PDDinSeconds AS PDDinSeconds,
		MaxDurationInSeconds AS MaxDurationInSeconds,
		LastCDRAttempt AS LastCDRAttempt,
		DeliveredNumberOfCalls AS DeliveredNumberOfCalls,
		OriginatingZoneID AS OriginatingZoneID,
		SwitchID AS SwitchID,
		FirstCDRAttempt AS FirstCDRAttempt
     
    FROM TrafficStats TS WITH(NOLOCK)
   WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		
),

Traffic AS (

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
        --1,
       Sum(SuccessfulAttempts)AS SuccessfulAttempts,
       CASE WHEN @GroupingField = 'CUSTOMERS' THEN Sum(NumberOfCalls  - SuccessfulAttempts) ELSE SUM(Attempts - SuccessfulAttempts) END as FailedAttempts,
	   Sum(CASE when TS.SupplierID IS NULL then Attempts ELSE 0 END) as BlockedAttempts
    FROM Traffic_Stats_Data TS WITH(NOLOCK)
       
WHERE  
		 (@OriginatingZoneID IS NULL OR TS.OriginatingZoneID = @OriginatingZoneID)
	AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL ) OR (TS.SwitchID = @SwitchID AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc ) ))
		AND TS.CustomerID IS NOT NULL 
		--AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
	
	
  Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	--ORDER by Attempts desc
	
)

SELECT * FROM Traffic 
ORDER by Attempts desc
    
END

if @CustomerID is NOT null and @SupplierID IS NULL
    BEGIN
    	;WITH		 
Traffic_Stats_Data AS (
		 select 
	    CustomerID  as CustomerID,
	    SupplierID  as SupplierID,
		NumberOfCalls AS NumberOfCalls,
		Attempts  as Attempts,
		DurationsInSeconds AS  DurationsInSeconds, 
		SuccessfulAttempts AS SuccessfulAttempts,
		deliveredAttempts AS deliveredAttempts,
		PDDinSeconds AS PDDinSeconds,
		MaxDurationInSeconds AS MaxDurationInSeconds,
		LastCDRAttempt AS LastCDRAttempt,
		DeliveredNumberOfCalls AS DeliveredNumberOfCalls,
		OriginatingZoneID AS OriginatingZoneID,
		SwitchID AS SwitchID,
		FirstCDRAttempt AS FirstCDRAttempt
     
    FROM TrafficStats TS WITH(NOLOCK)
   WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		
)
    	
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
    FROM Traffic_Stats_Data TS WITH(NOLOCK)
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OriginatingZoneID = OZ.ZoneID	
WHERE   (TS.CustomerID = @CustomerID) 
		AND (@OriginatingZoneID IS NULL OR TS.OriginatingZoneID = @OriginatingZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
	AND ((@SwitchID IS NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc ) ) OR TS.SwitchID = @SwitchID)
	
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
	ORDER by Attempts desc
    END
    
    
if @CustomerID is null and @SupplierID IS NOT NULL
 BEGIN
    	;WITH		 
Traffic_Stats_Data AS (
		 select 
	    CustomerID  as CustomerID,
	    SupplierID  as SupplierID,
		NumberOfCalls AS NumberOfCalls,
		Attempts  as Attempts,
		DurationsInSeconds AS  DurationsInSeconds, 
		SuccessfulAttempts AS SuccessfulAttempts,
		deliveredAttempts AS deliveredAttempts,
		PDDinSeconds AS PDDinSeconds,
		MaxDurationInSeconds AS MaxDurationInSeconds,
		LastCDRAttempt AS LastCDRAttempt,
		DeliveredNumberOfCalls AS DeliveredNumberOfCalls,
		OriginatingZoneID AS OriginatingZoneID,
		SwitchID AS SwitchID,
		FirstCDRAttempt AS FirstCDRAttempt
     
    FROM TrafficStats TS WITH(NOLOCK)
   WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		
)
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
    FROM Traffic_Stats_Data TS WITH(NOLOCK)
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID	
LEFT JOIN Zone OZ ON TS.OriginatingZoneID = OZ.ZoneID	
WHERE   (TS.SupplierID = @SupplierID) 
		AND (@OriginatingZoneID IS NULL OR TS.OriginatingZoneID = @OriginatingZoneID)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
		AND TS.CustomerID IS NOT NULL 
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
	
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE Ts.SupplierID END)
	ORDER by Attempts desc
 END
 
if @CustomerID is NOT null and @SupplierID IS NOT NULL
 BEGIN
    	;WITH		 
Traffic_Stats_Data AS (
		 select 
	    CustomerID  as CustomerID,
	    SupplierID  as SupplierID,
		NumberOfCalls AS NumberOfCalls,
		Attempts  as Attempts,
		DurationsInSeconds AS  DurationsInSeconds, 
		SuccessfulAttempts AS SuccessfulAttempts,
		deliveredAttempts AS deliveredAttempts,
		PDDinSeconds AS PDDinSeconds,
		MaxDurationInSeconds AS MaxDurationInSeconds,
		LastCDRAttempt AS LastCDRAttempt,
		DeliveredNumberOfCalls AS DeliveredNumberOfCalls,
		OriginatingZoneID AS OriginatingZoneID,
		SwitchID AS SwitchID,
		FirstCDRAttempt AS FirstCDRAttempt
     
    FROM TrafficStats TS WITH(NOLOCK)
   WHERE  FirstCDRAttempt >= @FromDateTime  AND   FirstCDRAttempt <= @ToDateTime
		
)
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
    FROM Traffic_Stats_Data TS WITH(NOLOCK)
LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			 LEFT JOIN CarrierAccount AS CAS WITH (NOLOCK) ON TS.SupplierID = CAS.CarrierAccountID
			 LEFT JOIN Zone OZ ON TS.OriginatingZoneID = OZ.ZoneID		
WHERE   (TS.SupplierID = @SupplierID) 
		AND (TS.OriginatingZoneID = @OriginatingZoneID)
		AND (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc )) OR TS.SwitchID = @SwitchID)
	
    Group by 
		(CASE WHEN @GroupingField = 'CUSTOMERS' THEN CustomerID ELSE TS.SupplierID END)
		
	ORDER by Attempts desc
end