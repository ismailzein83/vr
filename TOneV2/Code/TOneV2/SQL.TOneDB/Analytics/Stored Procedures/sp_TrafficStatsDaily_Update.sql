
CREATE PROCEDURE [Analytics].[sp_TrafficStatsDaily_Update]
	@TrafficStats Analytics.TrafficStatsDailyType READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE [TrafficStatsDaily]
	SET 
		[TrafficStatsDaily].Attempts =  [TrafficStatsDaily].Attempts + tStat.Attempts,
		[TrafficStatsDaily].DeliveredAttempts =  [TrafficStatsDaily].DeliveredAttempts + tStat.DeliveredAttempts,
		[TrafficStatsDaily].SuccessfulAttempts = [TrafficStatsDaily].SuccessfulAttempts +  tStat.SuccessfulAttempts,
		[TrafficStatsDaily].DurationsInSeconds = [TrafficStatsDaily].DurationsInSeconds + tStat.DurationsInSeconds,
		[TrafficStatsDaily].PDDInSeconds  = (([TrafficStatsDaily].SuccessfulAttempts * [TrafficStatsDaily].PDDInSeconds) +  (tStat.SuccessfulAttempts * tStat.PDDInSeconds))/( [TrafficStatsDaily].SuccessfulAttempts+tStat.SuccessfulAttempts ) ,
		[TrafficStatsDaily].MaxDurationInSeconds = CASE  WHEN [TrafficStatsDaily].MaxDurationInSeconds > tStat.MaxDurationInSeconds THEN [TrafficStatsDaily].MaxDurationInSeconds ELSE tStat.MaxDurationInSeconds END,
		[TrafficStatsDaily].UtilizationInSeconds = [TrafficStatsDaily].UtilizationInSeconds +  tStat.UtilizationInSeconds,
		[TrafficStatsDaily].NumberOfCalls = [TrafficStatsDaily].NumberOfCalls +  tStat.NumberOfCalls,
		[TrafficStatsDaily].DeliveredNumberOfCalls = [TrafficStatsDaily].DeliveredNumberOfCalls +  tStat.DeliveredNumberOfCalls,
		[TrafficStatsDaily].PGAD = (([TrafficStatsDaily].SuccessfulAttempts * [TrafficStatsDaily].PGAD) +  (tStat.SuccessfulAttempts * tStat.PGAD))/( [TrafficStatsDaily].SuccessfulAttempts+tStat.SuccessfulAttempts ) ,
		[TrafficStatsDaily].CeiledDuration = [TrafficStatsDaily].CeiledDuration +  tStat.CeiledDuration,
		[TrafficStatsDaily].ReleaseSourceAParty =[TrafficStatsDaily].ReleaseSourceAParty + tStat.ReleaseSourceAParty,
		[TrafficStatsDaily].ReleaseSourceS =[TrafficStatsDaily].ReleaseSourceS + tStat.ReleaseSourceS
	FROM [TrafficStatsDaily] inner join @TrafficStats as tStat ON  [TrafficStatsDaily].ID = tStat.ID
	
	
END