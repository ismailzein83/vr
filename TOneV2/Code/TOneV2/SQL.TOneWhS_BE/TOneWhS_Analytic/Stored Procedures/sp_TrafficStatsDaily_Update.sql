-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Analytic].[sp_TrafficStatsDaily_Update]
	@TrafficStats [TOneWhS_Analytic].TrafficStatsType READONLY
AS
BEGIN

	
	UPDATE [TOneWhS_Analytic].[TrafficStatsDaily]
	SET 
		TrafficStatsDaily.Attempts = TrafficStatsDaily.Attempts + tStat.Attempts,
		TrafficStatsDaily.DurationInSeconds = TrafficStatsDaily.DurationInSeconds + tStat.TotalDurationInSeconds,
		TrafficStatsDaily.FirstCDRAttempt = CASE  WHEN TrafficStatsDaily.FirstCDRAttempt < tStat.FirstCDRAttempt THEN TrafficStatsDaily.FirstCDRAttempt ELSE tStat.FirstCDRAttempt END,
		TrafficStatsDaily.LastCDRAttempt = CASE  WHEN TrafficStatsDaily.LastCDRAttempt > tStat.LastCDRAttempt THEN TrafficStatsDaily.LastCDRAttempt ELSE tStat.LastCDRAttempt END,
	    TrafficStatsDaily.DeliveredAttempts =  TrafficStatsDaily.DeliveredAttempts + tStat.DeliveredAttempts,
		TrafficStatsDaily.SuccessfulAttempts = TrafficStatsDaily.SuccessfulAttempts +  tStat.SuccessfulAttempts,
		TrafficStatsDaily.SumOfPDDInSeconds  =  TrafficStatsDaily.SumOfPDDInSeconds +  tStat.PDDInSeconds ,
		TrafficStatsDaily.MaxDurationInSeconds = CASE  WHEN TrafficStatsDaily.MaxDurationInSeconds > tStat.MaxDurationInSeconds THEN TrafficStatsDaily.MaxDurationInSeconds ELSE tStat.MaxDurationInSeconds END,
		TrafficStatsDaily.NumberOfCalls = TrafficStatsDaily.NumberOfCalls +  tStat.NumberOfCalls,
	    TrafficStatsDaily.SumOfPGAD = TrafficStatsDaily.SumOfPGAD +    tStat.SumOfPGAD,
		TrafficStatsDaily.DeliveredNumberOfCalls = TrafficStatsDaily.DeliveredNumberOfCalls +  tStat.DeliveredNumberOfCalls,
		TrafficStatsDaily.CeiledDuration = TrafficStatsDaily.CeiledDuration +  tStat.CeiledDuration
	--	TrafficStatsDaily.UtilizationInSeconds = TrafficStatsDaily.UtilizationInSeconds +  tStat.UtilizationInSeconds

	FROM [TOneWhS_Analytic].[TrafficStatsDaily]  inner join @TrafficStats as tStat ON  [TrafficStatsDaily].ID = tStat.ID
	
END