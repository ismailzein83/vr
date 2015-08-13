

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetStrategies] 
(
	@PeriodId int,
	@IsEnabled bit
)
AS
BEGIN
SELECT s.[Id],s.[Name], s.[StrategyContent] , s.[PeriodId], s.[UserId]
  FROM FraudAnalysis.Strategy s
  Where (@PeriodId =0 or PeriodId = @PeriodId) and (@IsEnabled is null or IsEnabled=@IsEnabled)
END