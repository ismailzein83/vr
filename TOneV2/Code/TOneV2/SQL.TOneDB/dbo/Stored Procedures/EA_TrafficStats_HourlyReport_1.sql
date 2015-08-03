



--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE PROCEDURE [dbo].[EA_TrafficStats_HourlyReport] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CustomerID		varchar(10) = NULL,
	@SupplierID		varchar(10) = NULL,
    @SwitchID		tinyInt = NULL,
    @OurZoneID 		int = NULL,
    @PortInOut		varchar(50) = NULL,
	@SelectPort		int = NULL,
    @ConnectionType varchar(50) = NULL ,
    @CodeGroup varchar(10) = NULL  ,
    @AllAccounts varchar(MAX) = NULL

AS
BEGIN	
	SET NOCOUNT ON
if @CustomerID is not null and @SupplierID is NULL
begin
	
      SELECT
		--ts.SupplierID AS SupplierID,
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		SUM(ceiledDuration) / 60.0 as CeiledDuration,
		case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) as SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
   	LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND  TS.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) 
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
	SELECT
		--ts.SupplierID AS SupplierID,
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		SUM(ceiledDuration) / 60.0 as CeiledDuration,
		case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt,
        Sum(UtilizationInSeconds)/60.0 as UtilizationInMinutes
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
   LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND TS.SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) 
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