
CREATE   PROCEDURE [dbo].[Sp_GetSupplierZoneStatsByZone]
    @FromDate    datetime,
    @ToDate        datetime,
    @ZoneId int
WITH RECOMPILE
AS
BEGIN  
    SET NOCOUNT ON
 SELECT
        ISNULL(TS.SupplierID,''),
        ISNULL(TS.OurZoneID,0),
        ISNULL(TS.SupplierZoneID,0),
        SUM(DurationsInSeconds/60.) as DurationsInMinutes,
        (SUM(TS.SuccessfulAttempts)*100.0 / SUM(TS.Attempts)) AS ASR,
        CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(TS.DurationsInSeconds)/(60.0 * SUM(TS.SuccessfulAttempts)) ELSE 0 END AS ACD
    FROM TrafficStatsDaily TS 
    -- inner join Zone z on z.ZoneID=ts.OurZoneID
  WHERE TS.Calldate BETWEEN @FromDate AND @ToDate AND TS.CustomerID IS NOT NULL and ts.OurZoneID=@ZoneId
    GROUP BY
        ISNULL(TS.SupplierID,''),
        ISNULL(TS.OurZoneID,0),
        ISNULL(TS.SupplierZoneID,0)

END