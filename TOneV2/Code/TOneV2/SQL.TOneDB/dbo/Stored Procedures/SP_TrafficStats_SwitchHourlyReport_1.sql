--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE  PROCEDURE [dbo].[SP_TrafficStats_SwitchHourlyReport] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@SwitchID		tinyInt = NULL  
AS
BEGIN	
	SET NOCOUNT ON
	SET @FromDateTime = DATEADD(day,0,datediff(day,0, @FromDateTime))
	SET @ToDateTime = DATEADD(s,-1,DATEADD(day,1,datediff(day,0, @ToDateTime)))
	DECLARE @IPPattern VARCHAR(10) 
	SET @IPPattern = '%.%.%.%'
	
	SELECT
        datepart(hour,LastCDRAttempt) AS [Hour],
        dateadd(dd,0, datediff(dd,0,LastCDRAttempt)) AS [Date],
        Ts.SwitchId AS SwitchID,
        SUM(CASE WHEN ts.Port_IN not LIKE @IPPattern THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS TDM_InDuration,
        SUM(CASE WHEN ts.Port_IN not LIKE @IPPattern AND ts.Port_Out not LIKE @IPPattern  THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS TDM_TDMOutDuration,
        SUM(CASE WHEN ts.Port_IN LIKE @IPPattern THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS IP_InDuration,
        SUM(CASE WHEN ts.Port_IN LIKE @IPPattern AND ts.Port_Out not LIKE @IPPattern  THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS IP_TDMOutDuration,
        SUM(CASE WHEN ts.Port_Out not LIKE @IPPattern THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS TDM_OutDuration,
        SUM(CASE WHEN ts.Port_Out LIKE @IPPattern THEN ts.DurationsInSeconds ELSE 0 END )/60.0 AS IP_OutDuration,
        
        SUM(CASE WHEN ts.Port_IN not LIKE @IPPattern THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS TDM_InUtilDuration,
        SUM(CASE WHEN ts.Port_IN not LIKE @IPPattern AND ts.Port_Out not LIKE @IPPattern  THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS TDM_TDMOutUtilDuration,
        SUM(CASE WHEN ts.Port_IN LIKE @IPPattern THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS IP_InUtilDuration,
        SUM(CASE WHEN ts.Port_IN LIKE @IPPattern AND ts.Port_Out not LIKE @IPPattern  THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS IP_TDMOutUtilDuration,
        SUM(CASE WHEN ts.Port_Out not LIKE @IPPattern THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS TDM_OutUtilDuration,
        SUM(CASE WHEN ts.Port_Out LIKE @IPPattern THEN ts.UtilizationInSeconds ELSE 0 END )/60.0 AS IP_OutUtilDuration
         
   FROM TrafficStats AS TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst) )                                     
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	GROUP BY datepart(hour,LastCDRAttempt),dateadd(dd,0, datediff(dd,0,LastCDRAttempt)),TS.SwitchId
	ORDER BY [Hour] ,[Date]

END