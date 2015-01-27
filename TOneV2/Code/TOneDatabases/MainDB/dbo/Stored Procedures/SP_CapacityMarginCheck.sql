CREATE  PROCEDURE [dbo].[SP_CapacityMarginCheck] 
    @FromDateTime	datetime,
	@ToDateTime		DATETIME
WITH RECOMPILE
AS
BEGIN	

SET NOCOUNT ON
        
SELECT 
	        switchid,
	        customerid,
	        supplierid,
			Port_In,
			Port_Out,
			SUM(NumberOfCalls) AS Attempts,
			SUM(SuccessfulAttempts) AS  SuccesfulAttempts,
			SUM(SuccessfulAttempts) * 100.0 /
			Cast(NULLIF(SUM(NumberOfCalls) + 0.0, 0) AS NUMERIC) AS ASR,
			SUM(DurationsInSeconds)/60.0   AS DurationsInMinutes,
			SUM(UtilizationInSeconds)/60.0   AS UtilizationsInMinutes		

	FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
	WHERE   
	FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	GROUP BY 
	        switchid,
	        customerid,
	        supplierid,
			Port_In,
			Port_Out
			    
END