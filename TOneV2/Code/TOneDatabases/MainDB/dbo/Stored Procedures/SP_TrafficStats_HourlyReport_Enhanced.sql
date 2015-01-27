



--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE PROCEDURE [dbo].[SP_TrafficStats_HourlyReport_Enhanced] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CustomerID		varchar(10) = NULL,
	@SupplierID		varchar(10) = NULL,
    @SwitchID		tinyInt = NULL,
    @OurZoneID 		int = NULL,
    @PortInOut		varchar(50) = NULL,
	@SelectPort		int = NULL,
    @ConnectionType varchar(50) = NULL ,
    @CodeGroup varchar(10) = NULL  
AS
BEGIN	
	SET NOCOUNT ON
if @CustomerID is not null and @SupplierID is NULL
begin
;WITH TraficBase AS 
(
	
	SELECT
	
        LastCDRAttempt AS LastCDRAttempt,
        Attempts as Attempts,
		DurationsInSeconds as DurationsInSeconds, 
		NumberOfCalls AS NumberOfCalls,
		SuccessfulAttempts AS SuccessfulAttempts,
		deliveredAttempts  as deliveredAttempts, 
        SuccessfulAttempts as SuccessfulAttempt,
        UtilizationInSeconds AS UtilizationInSeconds,
        OurZoneID AS OurZoneID,
        FirstCDRAttempt AS FirstCDRAttempt,
        CustomerID AS CustomerID,
        SupplierID AS SupplierID,
        Port_IN AS Port_IN,SwitchID AS SwitchID,Port_OUT AS Port_OUT
        FROM TrafficStats  WITH(NOLOCK)
   	                                     
    WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	
	
)	
      SELECT
		--ts.SupplierID AS SupplierID,
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) as SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TraficBase AS TS WITH(NOLOCK)
   	LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
	    (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    --AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	     AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR (@SelectPort = 0 AND (TS.Port_IN IN (@ConnectionType))) OR (@SelectPort = 1 AND (TS.Port_OUT IN (@ConnectionType))))
	    --AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    --AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        --AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
       
   
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt))--,ts.SupplierID
	ORDER BY [Hour] ,[Date]
end
if @CustomerID is null and @SupplierID is NOT NULL
BEGIN
	
	;WITH TraficBase1 AS 
(
	
	SELECT
	
        LastCDRAttempt AS LastCDRAttempt,
        Attempts as Attempts,
		DurationsInSeconds as DurationsInSeconds, 
		NumberOfCalls AS NumberOfCalls,
		SuccessfulAttempts AS SuccessfulAttempts,
		deliveredAttempts  as deliveredAttempts, 
        SuccessfulAttempts as SuccessfulAttempt,
        UtilizationInSeconds AS UtilizationInSeconds,
        OurZoneID AS OurZoneID,
        FirstCDRAttempt AS FirstCDRAttempt,
        CustomerID AS CustomerID,
        SupplierID AS SupplierID,
        Port_IN AS Port_IN,SwitchID AS SwitchID,Port_OUT AS Port_OUT
        FROM TrafficStats  WITH(NOLOCK)
   	                                     
    WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	
	
)	
	SELECT
		--ts.SupplierID AS SupplierID,
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TraficBase1 AS TS WITH(NOLOCK)
   LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
       (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    --AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	     AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR (@SelectPort = 0 AND (TS.Port_IN IN (@ConnectionType))) OR (@SelectPort = 1 AND (TS.Port_OUT IN (@ConnectionType))))
	    --AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    --AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        --AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
        
   
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt))--,ts.SupplierID
	ORDER BY [Hour] ,[Date]
	
end
if @CustomerID is not null and @SupplierID is not null
BEGIN
	
	;WITH TraficBase2 AS 
(
	
	SELECT
	
        LastCDRAttempt AS LastCDRAttempt,
        Attempts as Attempts,
		DurationsInSeconds as DurationsInSeconds, 
		NumberOfCalls AS NumberOfCalls,
		SuccessfulAttempts AS SuccessfulAttempts,
		deliveredAttempts  as deliveredAttempts, 
        SuccessfulAttempts as SuccessfulAttempt,
        UtilizationInSeconds AS UtilizationInSeconds,
        OurZoneID AS OurZoneID,
        FirstCDRAttempt AS FirstCDRAttempt,
        CustomerID AS CustomerID,
        SupplierID AS SupplierID,
        Port_IN AS Port_IN,SwitchID AS SwitchID,Port_OUT AS Port_OUT
        FROM TrafficStats  WITH(NOLOCK)
   	                                     
    WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	
	
)	
	
	SELECT
		--ts.SupplierID AS SupplierID,
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(Attempts) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(attempts) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TraficBase2 AS TS WITH(NOLOCK)
        LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
       (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    --AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	     AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR (@SelectPort = 0 AND (TS.Port_IN IN (@ConnectionType))) OR (@SelectPort = 1 AND (TS.Port_OUT IN (@ConnectionType))))
	    --AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    --AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        --AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
        
   
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt))--,ts.SupplierID
	ORDER BY [Hour] ,[Date]
end
if @CustomerID is null and @SupplierID is NULL
BEGIN
	;WITH TraficBase3 AS 
(
	
	SELECT
	
        LastCDRAttempt AS LastCDRAttempt,
        Attempts as Attempts,
		DurationsInSeconds as DurationsInSeconds, 
		NumberOfCalls AS NumberOfCalls,
		SuccessfulAttempts AS SuccessfulAttempts,
		deliveredAttempts  as deliveredAttempts, 
        SuccessfulAttempts as SuccessfulAttempt,
        UtilizationInSeconds AS UtilizationInSeconds,
        OurZoneID AS OurZoneID,
        FirstCDRAttempt AS FirstCDRAttempt,
        CustomerID AS CustomerID,
        SupplierID AS SupplierID,
        Port_IN AS Port_IN,SwitchID AS SwitchID,Port_OUT AS Port_OUT
        FROM TrafficStats  WITH(NOLOCK)
   	                                     
    WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	
	
)	
	SELECT
		--ts.SupplierID AS SupplierID,
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TraficBase3 AS TS WITH(NOLOCK)
   LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
       (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    --AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	     AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N') OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR (@SelectPort = 0 AND (TS.Port_IN IN (@ConnectionType))) OR (@SelectPort = 1 AND (TS.Port_OUT IN (@ConnectionType))))
	    --AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    --AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        --AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
        
   
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt))--,ts.SupplierID
	ORDER BY [Hour] ,[Date]
end
END