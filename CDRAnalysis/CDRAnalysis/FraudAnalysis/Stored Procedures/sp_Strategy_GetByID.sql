
CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetByID] 
	@Id int
AS
BEGIN
	SELECT s.[Id],
		s.[Name],
		s.[StrategyContent],
		s.[PeriodId],
		p.[Description] AS StrategyType,
		s.[UserId]

	FROM FraudAnalysis.[Strategy] s
	INNER JOIN FraudAnalysis.Period p ON p.ID = s.PeriodID

	WHERE s.Id = @Id
END