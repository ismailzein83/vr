--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE   PROCEDURE [dbo].[SP_TrafficStats_DailyTimeShift] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CustomerID		varchar(10) = NULL,
	@SupplierID		varchar(10) = NULL
AS
BEGIN	
	SET NOCOUNT ON
	SELECT
         DATEADD(day,0,datediff(day,0, LastCDRAttempt))AS [Day],
        Sum(Attempts) as Attempts,
        Sum(SuccessfulAttempts) AS SuccessfulAttempt,
        Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
		Sum(DurationsInSeconds)/60.0 as [Minutes], 
		Sum(SuccessfulAttempts)*100.0 / Sum(Attempts) as ASR,
		case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
		Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
	    
		--MAX(DurationsInSeconds) / 60.0 as MaxDuration,
        Max(LastCDRAttempt) as LastCall
        
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
        LEFT OUTER JOIN
        Zone AS Z ON TS.OurZoneID = Z.ZoneID                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@CustomerID IS NULL OR TS.CustomerID = @CustomerID) 
		AND (@SupplierID IS NULL OR TS.SupplierID = @SupplierID)  
	GROUP BY  DATEADD(day,0,datediff(day,0, LastCDRAttempt))
	ORDER BY [Day] 

END