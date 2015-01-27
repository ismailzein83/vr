CREATE  PROCEDURE [dbo].[SP_CapacityPeakMarginCheck] 
    @TheDate DATETIME
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON
DECLARE @FromDateTime DATETIME
DECLARE @ToDateTime DATETIME

   SET @FromDateTime = dbo.DateOf(@TheDate)
   SET @ToDateTime = DATEADD(D,1,@FromDateTime)
	
 
SELECT 
	        DATEPART(hour,FirstCDRAttempt) AS [Hour],
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
		FirstCDRAttempt >= @FromDateTime AND FirstCDRAttempt<@ToDateTime
	   GROUP BY 
            DATEPART(hour,FirstCDRAttempt),
	        switchid,
	        customerid,
	        supplierid,
			Port_In,
			Port_Out
			    
END