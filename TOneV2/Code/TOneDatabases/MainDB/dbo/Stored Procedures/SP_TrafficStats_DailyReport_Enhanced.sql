--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE  PROCEDURE [dbo].[SP_TrafficStats_DailyReport_Enhanced] 
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
	;WITH TraficBase
	AS (
		
		SELECT
        dbo.DateOf(LastCDRAttempt) AS [Day],
        Attempts as Attempts,
		DurationsInSeconds as DurationsInSeconds, 
		SuccessfulAttempts as SuccessfulAttempts,
		deliveredAttempts as deliveredAttempts,
	    LastCDRAttempt as LastCDRAttempt,
	    OurZoneID AS OurZoneID,
	    FirstCDRAttempt AS FirstCDRAttempt,
	    Port_OUT AS Port_OUT,
	    Port_IN AS Port_IN,
	    CustomerID AS CustomerID,
	    TS.SupplierID AS SupplierID,
	    SwitchID AS SwitchID
	    --,Z.CodeGroup AS CodeGroup
       
       FROM TrafficStats AS TS WITH(NOLOCK) 
      -- LEFT OUTER JOIN
       -- Zone AS Z ON TS.OurZoneID = Z.ZoneID                                    
       WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		
		
		
		)
	SELECT
         dbo.DateOf(LastCDRAttempt) AS [Day],
        Sum(Attempts) as Attempts,
		Sum(DurationsInSeconds)/60.0 as DurationsInMinutes, 
		Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastAttempt,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt
   FROM TraficBase  TS WITH(NOLOCK)
        LEFT OUTER JOIN
       Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		--FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime and
       ( TS.CustomerID = @CustomerID OR @CustomerID IS NULL) 
		AND ( TS.SupplierID = @SupplierID OR @SupplierID IS NULL ) 
	    AND ( TS.SwitchID = @SwitchID OR @SwitchID IS NULL )
	    AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)
	    AND (@ConnectionType IS NULL OR TS.Port_OUT = @ConnectionType)      
	    AND Ts.Port_IN like (CASE @SelectPort WHEN 0 THEN @PortInOut ELSE '%%' END)    
        AND Ts.Port_out like (CASE @SelectPort WHEN 1 THEN @PortInOut ELSE '%%' END) 
        AND (@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
        --AND (@CodeGroup IS NULL OR CodeGroup = @CodeGroup)
   
	GROUP BY  dbo.DateOf(LastCDRAttempt)
	ORDER BY [Day] 

END