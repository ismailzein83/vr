


CREATE PROCEDURE [FraudAnalysis].[sp_Dashboard_GetFraudCasesPerStrategy]
(
	@FromDate datetime,
	@ToDate datetime
)
	AS
	BEGIN
		
		select Sum(unionTable.CountCases)as CountCases, unionTable.StrategyName from(

			select count(distinct sc.AccountNumber) CountCases , s.Name as StrategyName from FraudAnalysis.Strategy s
			inner join FraudAnalysis.AccountCase sc on  sc.StrategyId=s.Id 
			where sc.LogDate between @FromDate and @ToDate   and sc.StatusId=3   
			group by  s.Name
			
				union 	
				
			select 0 as CountCases, s.Name as StrategyName from FraudAnalysis.Strategy s ) unionTable

		group by unionTable.StrategyName
		
	END