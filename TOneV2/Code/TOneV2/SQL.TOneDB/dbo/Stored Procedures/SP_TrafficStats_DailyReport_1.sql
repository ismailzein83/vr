--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE   PROCEDURE [dbo].[SP_TrafficStats_DailyReport] 
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
	SELECT
        DATEADD(day,0,datediff(day,0, LastCDRAttempt)) AS [Day],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID) 
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
   
	GROUP BY  DATEADD(day,0,datediff(day,0, LastCDRAttempt))
	ORDER BY [Day] 

END