CREATE PROCEDURE [dbo].[sp_TrafficStats_CarrierPortMonitor_Enhanced]
	@FromDateTime	DATETIME,
	@ToDateTime		DATETIME,
	@InOut			varchar(3) = NULL,
	@CarrierID		varchar(10) = NULL,
	@SwitchID		tinyInt = NULL
	
WITH recompile 
AS
SET NOCOUNT ON
	IF (@InOut = 'IN') 
	BEGIN	
		-- Customer 
		IF @CarrierID IS NOT NULL
		WITH TrafficData AS (
			SELECT *
			FROM TrafficStats TS WITH(NOLOCK)
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			AND TS.Port_IN IS NOT NULL
			
			)
			, TrafficStats_IN_WithCarrier AS (
			SELECT	CustomerID As CarrierAccountID,
				Port_IN AS Port,
			Sum(NumberOfCalls) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			case when Sum(NumberOfCalls) > 0 then Sum(deliveredNumberOfCalls) * 100.0 / SUM(NumberOfCalls) ELSE 0 end as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficData TS WITH(NOLOCK)
			WHERE 
			 CustomerID = @CarrierID  AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) 
			 and SupplierID is not null AND ts.SupplierID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) 
			OR TS.SwitchID = @SwitchID)
				
			Group BY TS.CustomerID, TS.Port_IN
			)
		
			SELECT * FROM TrafficStats_IN_WithCarrier ORDER by Attempts DESC
		ELSE
			-- No Customer
			WITH TrafficData AS (
				SELECT *
			FROM TrafficStats TS WITH(NOLOCK)
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
			AND TS.Port_IN IS NOT NULL
				)
			, TrafficStats_IN_WithoutCarrier AS 
			(
			SELECT CustomerID As CarrierAccountID,
				Port_IN AS Port,
			Sum(NumberOfCalls) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			case when Sum(NumberOfCalls) > 0 then Sum(deliveredNumberOfCalls) * 100.0 / SUM(NumberOfCalls) ELSE 0 end as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficData TS WITH(NOLOCK)
			WHERE 
			 ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) 
			 and SupplierID is not null AND ts.SupplierID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) 
			OR TS.SwitchID = @SwitchID)
			Group BY TS.CustomerID, TS.Port_IN
			
			)
			select * from TrafficStats_IN_WithoutCarrier ORDER by Attempts DESC			
	END	
	
	IF (@InOut = 'OUT') 
	BEGIN 
		-- Supplier
		IF @CarrierID IS NOT NULL
		WITH TrafficData AS (
			SELECT 	*
			FROM TrafficStats TS WITH(NOLOCK)
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND SupplierID = @CarrierID
			AND TS.Port_OUT IS NOT NULL
			)
		 ,TrafficStats_OUT_WithCarrier AS (
		
			SELECT 	SupplierID As CarrierAccountID,
				Port_OUT AS Port,
			Sum(Attempts) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficData TS WITH(NOLOCK)
			WHERE 
			((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) 
			 and SupplierID is not null AND ts.SupplierID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) 
			OR TS.SwitchID = @SwitchID)
				
			Group BY TS.SupplierID, TS.Port_OUT
		)
		SELECT * FROM TrafficStats_OUT_WithCarrier ORDER by Attempts Desc
			
		ELSE
			-- No Supplier
			WITH TrafficData AS (
				SELECT *
			FROM TrafficStats TS WITH(NOLOCK)
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				)
			 ,TrafficStats_OUT_WithoutCarrier AS (
			SELECT SupplierID As CarrierAccountID,
				Port_OUT AS Port,
			Sum(Attempts) as Attempts, 
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			Avg(PDDinSeconds) as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			FROM TrafficData TS WITH(NOLOCK)
			WHERE 
			TS.Port_OUT IS NOT NULL
			and ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)
			and SupplierID is not null  AND ts.SupplierID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) 
			OR TS.SwitchID = @SwitchID)
			Group BY TS.SupplierID,TS.Port_OUT
			)
			SELECT * FROM TrafficStats_OUT_WithoutCarrier ORDER by Attempts Desc		
	END