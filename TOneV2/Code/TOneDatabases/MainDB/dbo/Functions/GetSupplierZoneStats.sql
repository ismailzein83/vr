-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetSupplierZoneStats]
(
	@FromDate datetime,
	@TillDate DATETIME,
	@ZoneID INT =null  
)
RETURNS @Stats TABLE 
(
	SupplierID VARCHAR(10)
	,OurZoneID INT
	,DurationsInMinutes  NUMERIC(13,5)
	,ASR NUMERIC(13,5)
	,ACD NUMERIC(13,5)
	PRIMARY KEY (SupplierID,OurZoneID)
)
AS
BEGIN
IF @ZoneID IS NULL 
	INSERT INTO @Stats
	SELECT 
		ISNULL(TS.SupplierID,''), 
		ISNULL(TS.OurZoneID,0), 
        SUM(DurationsInSeconds/60.) as DurationsInMinutes, 
	    (SUM(TS.SuccessfulAttempts)*100.0 / SUM(TS.Attempts)) AS ASR,
		CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(TS.DurationsInSeconds)/(60.0 * SUM(TS.SuccessfulAttempts)) ELSE 0 END AS ACD
	FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
         WHERE 
			TS.FirstCDRAttempt BETWEEN @FromDate AND @TillDate
	GROUP BY TS.SupplierID, TS.OurZoneID
ELSE
	INSERT INTO @Stats
	SELECT 
		ISNULL(TS.SupplierID,''), 
		ISNULL(TS.OurZoneID,0), 
        SUM(DurationsInSeconds/60.) as DurationsInMinutes, 
	    (SUM(TS.SuccessfulAttempts)*100.0 / SUM(TS.Attempts)) AS ASR,
		CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(TS.DurationsInSeconds)/(60.0 * SUM(TS.SuccessfulAttempts)) ELSE 0 END AS ACD
	FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
         WHERE 
			TS.FirstCDRAttempt BETWEEN @FromDate AND @TillDate
			AND TS.OurZoneID = @ZoneID
	GROUP BY TS.SupplierID, TS.OurZoneID
	RETURN 
END