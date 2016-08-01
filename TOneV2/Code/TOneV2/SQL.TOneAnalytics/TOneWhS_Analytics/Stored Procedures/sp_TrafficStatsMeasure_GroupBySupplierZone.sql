-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Analytics].[sp_TrafficStatsMeasure_GroupBySupplierZone]
@TimeSpan int
AS
BEGIN
	SELECT [SupplierZoneID]
      ,Sum([Attempts]) as TotalNumberOfAttempts
      ,Sum([DurationInSeconds]) as TotalDurationInSeconds
      ,Sum([DeliveredAttempts]) as TotalDeliveredAttempts
      ,Sum([SuccessfulAttempts]) as TotalSuccesfulAttempts

  FROM [TOneWhS_Analytics].[TrafficStats15Min] with (nolock)
    where DATEDIFF(ss, BatchStart, GETDATE()) < @TimeSpan
group by [SupplierZoneID]
     
END