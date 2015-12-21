-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_Analytic].[sp_TrafficStatsByCode_Update]
	@TrafficStatsByCode [TOneWhS_Analytic].TrafficStatsType READONLY
AS
BEGIN

	
	UPDATE [TOneWhS_Analytic].[TrafficStatsByCode]
	SET 
		[TrafficStatsByCode].Attempts = [TrafficStatsByCode].Attempts + tStat.Attempts,
		[TrafficStatsByCode].DurationInSeconds = [TrafficStatsByCode].DurationInSeconds + tStat.TotalDurationInSeconds,
		[TrafficStatsByCode].FirstCDRAttempt = CASE  WHEN [TrafficStatsByCode].FirstCDRAttempt < tStat.FirstCDRAttempt THEN [TrafficStatsByCode].FirstCDRAttempt ELSE tStat.FirstCDRAttempt END,
		[TrafficStatsByCode].LastCDRAttempt = CASE  WHEN [TrafficStatsByCode].LastCDRAttempt > tStat.LastCDRAttempt THEN [TrafficStatsByCode].LastCDRAttempt ELSE tStat.LastCDRAttempt END,
		[TrafficStatsByCode].DeliveredAttempts =  [TrafficStatsByCode].DeliveredAttempts + tStat.DeliveredAttempts,
		[TrafficStatsByCode].SuccessfulAttempts = [TrafficStatsByCode].SuccessfulAttempts +  tStat.SuccessfulAttempts,
		[TrafficStatsByCode].SumOfPDDInSeconds  =  [TrafficStatsByCode].SumOfPDDInSeconds +  tStat.PDDInSeconds ,
		[TrafficStatsByCode].MaxDurationInSeconds = CASE  WHEN [TrafficStatsByCode].MaxDurationInSeconds > tStat.MaxDurationInSeconds THEN [TrafficStatsByCode].MaxDurationInSeconds ELSE tStat.MaxDurationInSeconds END,
		[TrafficStatsByCode].NumberOfCalls = [TrafficStatsByCode].NumberOfCalls +  tStat.NumberOfCalls,
		[TrafficStatsByCode].SumOfPGAD = [TrafficStatsByCode].SumOfPGAD +    tStat.SumOfPGAD,
		[TrafficStatsByCode].DeliveredNumberOfCalls = [TrafficStatsByCode].DeliveredNumberOfCalls +  tStat.DeliveredNumberOfCalls,
		[TrafficStatsByCode].CeiledDuration = [TrafficStatsByCode].CeiledDuration +  tStat.CeiledDuration
	--	TrafficStats.UtilizationInSeconds = TrafficStats.UtilizationInSeconds +  tStat.UtilizationInSeconds

	FROM [TOneWhS_Analytic].[TrafficStatsByCode]  inner join @TrafficStatsByCode as tStat ON  [TrafficStatsByCode].ID = tStat.ID
	
	
END