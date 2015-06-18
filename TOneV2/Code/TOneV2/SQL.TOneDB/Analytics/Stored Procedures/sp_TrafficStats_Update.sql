
CREATE PROCEDURE [Analytics].[sp_TrafficStats_Update]
	@TrafficStats Analytics.TrafficStatsType READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE [TrafficStats]
	SET 
		TrafficStats.FirstCDRAttempt = CASE  WHEN TrafficStats.FirstCDRAttempt < tStat.FirstCDRAttempt THEN TrafficStats.FirstCDRAttempt ELSE tStat.FirstCDRAttempt END,
		TrafficStats.LastCDRAttempt = CASE  WHEN TrafficStats.LastCDRAttempt > tStat.LastCDRAttempt THEN TrafficStats.LastCDRAttempt ELSE tStat.LastCDRAttempt END,
		TrafficStats.Attempts =  TrafficStats.Attempts + tStat.Attempts,
		TrafficStats.DeliveredAttempts =  TrafficStats.DeliveredAttempts + tStat.DeliveredAttempts,
		TrafficStats.SuccessfulAttempts = TrafficStats.SuccessfulAttempts +  tStat.SuccessfulAttempts,
		TrafficStats.DurationsInSeconds = TrafficStats.DurationsInSeconds + tStat.DurationsInSeconds,
		TrafficStats.PDDInSeconds  = ((TrafficStats.SuccessfulAttempts * TrafficStats.PDDInSeconds) +  (tStat.SuccessfulAttempts * tStat.PDDInSeconds))/( TrafficStats.SuccessfulAttempts+tStat.SuccessfulAttempts ) ,
		TrafficStats.MaxDurationInSeconds = CASE  WHEN TrafficStats.MaxDurationInSeconds > tStat.MaxDurationInSeconds THEN TrafficStats.MaxDurationInSeconds ELSE tStat.MaxDurationInSeconds END,
		TrafficStats.UtilizationInSeconds = TrafficStats.UtilizationInSeconds +  tStat.UtilizationInSeconds,
		TrafficStats.NumberOfCalls = TrafficStats.NumberOfCalls +  tStat.NumberOfCalls,
		TrafficStats.DeliveredNumberOfCalls = TrafficStats.DeliveredNumberOfCalls +  tStat.DeliveredNumberOfCalls,
		TrafficStats.PGAD = ((TrafficStats.SuccessfulAttempts * TrafficStats.PGAD) +  (tStat.SuccessfulAttempts * tStat.PGAD))/( TrafficStats.SuccessfulAttempts+tStat.SuccessfulAttempts ) ,
		TrafficStats.CeiledDuration = TrafficStats.CeiledDuration +  tStat.CeiledDuration,
		TrafficStats.ReleaseSourceAParty =TrafficStats.ReleaseSourceAParty + tStat.ReleaseSourceAParty,
		TrafficStats.ReleaseSourceS =TrafficStats.ReleaseSourceS + tStat.ReleaseSourceS
	FROM [TrafficStats]  inner join @TrafficStats as tStat ON  [TrafficStats].ID = tStat.ID
	
	
END