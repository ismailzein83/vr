CREATE PROCEDURE [TOneWhS_Analytics].[sp_TrafficStatsMeasure_GroupBySaleZone_Supplier]
@TimeSpan int
AS
BEGIN
	SELECT [SupplierID], [SaleZoneId]
      ,Sum([Attempts]) as TotalNumberOfAttempts
      ,Sum([DurationInSeconds]) as TotalDurationInSeconds
      ,Sum([DeliveredAttempts]) as TotalDeliveredAttempts
      ,Sum([SuccessfulAttempts]) as TotalSuccesfulAttempts

  FROM [TOneWhS_Analytics].[TrafficStats15Min] with (nolock)
  where DATEDIFF(ss, BatchStart, GETDATE()) < @TimeSpan
group by [SupplierID], [SaleZoneId]
     
END