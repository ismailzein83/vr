

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetAllEnabledByPeriodID] 
(
	@PeriodId int,
	@IsEnabled bit
)
AS
BEGIN
	SELECT s.[Id],
	s.[Name],
	s.[StrategyContent],
	s.[PeriodId],
	p.[Description] AS StrategyType,
	s.[UserId]
	
	FROM FraudAnalysis.Strategy s
	INNER JOIN FraudAnalysis.Period p ON p.ID = s.PeriodID
	
	Where (@PeriodId =0 or PeriodId = @PeriodId) and (@IsEnabled is null or IsEnabled=@IsEnabled)
END