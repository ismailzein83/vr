

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetStrategy] 
	@Id int
AS
BEGIN


	SELECT s.[Id],s.[Name], s.[StrategyContent] , s.[PeriodId], s.[UserId], p.Description as StrategyType
  FROM FraudAnalysis.[Strategy] s
  inner join FraudAnalysis.Period p on p.Id=s.PeriodId
  
	WHERE s.Id = @Id
END