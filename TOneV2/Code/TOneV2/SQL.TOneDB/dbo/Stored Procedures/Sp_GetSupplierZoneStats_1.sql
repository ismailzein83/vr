
CREATE   PROCEDURE [dbo].[Sp_GetSupplierZoneStats]
    @FromDate    datetime,
    @ToDate        datetime

WITH RECOMPILE
AS
BEGIN   
    SET NOCOUNT ON
  
 SELECT 
		ISNULL(TS.SupplierID,''), 
		ISNULL(TS.OurZoneID,0), 
        SUM(DurationsInSeconds/60.) as DurationsInMinutes, 
	    (SUM(TS.SuccessfulAttempts)*100.0 / SUM(TS.Attempts)) AS ASR,
		CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(TS.DurationsInSeconds)/(60.0 * SUM(TS.SuccessfulAttempts)) ELSE 0 END AS ACD
	FROM TrafficStatsDaily TS  with (nolock,index=IX_TrafficStatsDaily_DateTimeFirst)
 WHERE ( TS.Calldate BETWEEN @FromDate AND @ToDate) AND TS.CustomerID IS NOT NULL  
	GROUP BY 
		ISNULL(TS.SupplierID,''), 
		ISNULL(TS.OurZoneID,0)

   
   
END