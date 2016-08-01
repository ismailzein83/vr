CREATE PROCEDURE [TOneWhS_Analytic].[sp_TrafficStatsMeasure_GroupBySaleZone_Supplier]
@TimeSpan int
AS
BEGIN
	SELECT [SupplierID], [SaleZoneId]
      ,Sum([Attempts]) as TotalNumberOfAttempts
      ,Sum([DurationInSeconds]) as TotalDurationInSeconds
      ,Sum([DeliveredAttempts]) as TotalDeliveredAttempts
      ,Sum([SuccessfulAttempts]) as TotalSuccesfulAttempts

  FROM [TOneWhS_Analytic].[TrafficStats]
  where DATEDIFF(ss, FirstCDRAttempt, GETDATE()) < @TimeSpan
group by [SupplierID], [SaleZoneId]
     
END