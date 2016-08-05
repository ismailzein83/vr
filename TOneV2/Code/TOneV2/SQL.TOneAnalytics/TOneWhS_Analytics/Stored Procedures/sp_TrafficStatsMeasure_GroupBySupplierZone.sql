CREATE PROCEDURE [TOneWhS_Analytics].[sp_TrafficStatsMeasure_GroupBySupplierZone]
@TimeSpan int
AS
BEGIN

DECLARE @FromTime datetime
SET @FromTime = DateAdd(ss, -@TimeSpan, getdate())

SELECT	[SupplierZoneID]
		,Sum([Attempts]) as TotalNumberOfAttempts
		,Sum([DurationInSeconds]) as TotalDurationInSeconds
		,Sum([DeliveredAttempts]) as TotalDeliveredAttempts
		,Sum([SuccessfulAttempts]) as TotalSuccesfulAttempts
FROM	[TOneWhS_Analytics].[TrafficStats15Min] with (nolock)
where	BatchStart > @FromTime AND BatchStart<getdate() 
		AND SupplierZoneId is not null
group by [SupplierZoneID]
     
END