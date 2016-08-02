CREATE PROCEDURE [TOneWhS_Analytics].[sp_TrafficStatsMeasure_GroupBySaleZone_Supplier]
@TimeSpan int
AS
BEGIN

DECLARE @FromTime datetime
SET @FromTime = DateAdd(ss, -@TimeSpan, getdate())

SELECT [SupplierID], [SaleZoneId]
      ,Sum([Attempts]) as TotalNumberOfAttempts
      ,Sum([DurationInSeconds]) as TotalDurationInSeconds
      ,Sum([DeliveredAttempts]) as TotalDeliveredAttempts
      ,Sum([SuccessfulAttempts]) as TotalSuccesfulAttempts

FROM	[TOneWhS_Analytics].[TrafficStats15Min] with (nolock)
where	BatchStart > @FromTime AND BatchStart<getdate() 
		and SupplierID is not null and SaleZoneId is not null
group by [SupplierID], [SaleZoneId]
     
END