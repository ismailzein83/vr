-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Analytic].[sp_TrafficStats_Update]
	@TrafficStats [TOneWhS_Analytic].TrafficStatsType READONLY
AS
BEGIN

	
	UPDATE [TOneWhS_Analytic].[TrafficStats]
	SET 
		TrafficStats.Attempts = TrafficStats.Attempts + tStat.Attempts,
		TrafficStats.DurationInSeconds = TrafficStats.DurationInSeconds + tStat.TotalDurationInSeconds,
		TrafficStats.FirstCDRAttempt = CASE  WHEN TrafficStats.FirstCDRAttempt < tStat.FirstCDRAttempt THEN TrafficStats.FirstCDRAttempt ELSE tStat.FirstCDRAttempt END,
		TrafficStats.LastCDRAttempt = CASE  WHEN TrafficStats.LastCDRAttempt > tStat.LastCDRAttempt THEN TrafficStats.LastCDRAttempt ELSE tStat.LastCDRAttempt END,
		TrafficStats.DeliveredAttempts =  TrafficStats.DeliveredAttempts + tStat.DeliveredAttempts,
		TrafficStats.SuccessfulAttempts = TrafficStats.SuccessfulAttempts +  tStat.SuccessfulAttempts,
		TrafficStats.SumOfPDDInSeconds  =  TrafficStats.SumOfPDDInSeconds +  tStat.PDDInSeconds ,
		TrafficStats.MaxDurationInSeconds = CASE  WHEN TrafficStats.MaxDurationInSeconds > tStat.MaxDurationInSeconds THEN TrafficStats.MaxDurationInSeconds ELSE tStat.MaxDurationInSeconds END,
		TrafficStats.NumberOfCalls = TrafficStats.NumberOfCalls +  tStat.NumberOfCalls,
		TrafficStats.SumOfPGAD = TrafficStats.SumOfPGAD +    tStat.SumOfPGAD,
		TrafficStats.DeliveredNumberOfCalls = TrafficStats.DeliveredNumberOfCalls +  tStat.DeliveredNumberOfCalls,
		TrafficStats.CeiledDuration = TrafficStats.CeiledDuration +  tStat.CeiledDuration
	--	TrafficStats.UtilizationInSeconds = TrafficStats.UtilizationInSeconds +  tStat.UtilizationInSeconds

	FROM [TOneWhS_Analytic].[TrafficStats]  inner join @TrafficStats as tStat ON  [TrafficStats].ID = tStat.ID
	
	
END