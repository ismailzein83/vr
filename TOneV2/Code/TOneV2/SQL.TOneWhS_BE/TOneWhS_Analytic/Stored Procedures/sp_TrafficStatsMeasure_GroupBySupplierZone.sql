-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Analytic].[sp_TrafficStatsMeasure_GroupBySupplierZone]
@TimeSpan int
AS
BEGIN
	SELECT [SupplierZoneID]
      ,Sum([Attempts]) as TotalNumberOfAttempts
      ,Sum([DurationInSeconds]) as TotalDurationInSeconds
      ,Sum([DeliveredAttempts]) as TotalDeliveredAttempts
      ,Sum([SuccessfulAttempts]) as TotalSuccesfulAttempts

  FROM [TOneV2_Dev].[TOneWhS_Analytic].[TrafficStats]
    where DATEDIFF(ss, FirstCDRAttempt, GETDATE()) < @TimeSpan
group by [SupplierZoneID]
     
END