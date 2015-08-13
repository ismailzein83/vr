

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetStrategy] 
	@Id int
AS
BEGIN


	SELECT s.[Id],s.[Name], s.[StrategyContent] , s.[PeriodId], s.[UserId]
  FROM FraudAnalysis.[Strategy] s
  
	WHERE s.Id = @Id
END