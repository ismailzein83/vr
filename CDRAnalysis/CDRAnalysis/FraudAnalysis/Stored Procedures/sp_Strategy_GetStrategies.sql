

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetStrategies] 
(
	@PeriodId int,
	@IsEnabled bit
)
AS
BEGIN
SELECT s.[Id],s.[Name], s.[StrategyContent] , s.[PeriodId], s.[UserId], p.Description as StrategyType
  FROM FraudAnalysis.Strategy s
  inner join FraudAnalysis.Period p on p.Id=s.PeriodId
  Where (@PeriodId =0 or PeriodId = @PeriodId) and (@IsEnabled is null or IsEnabled=@IsEnabled)
END