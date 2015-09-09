


CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_GetFraudCasesPerStrategy]
(
	@FromDate datetime,
	@ToDate datetime
)
	AS
	BEGIN
		
		select Sum(unionTable.CountCases)as CountCases, unionTable.StrategyName from(

			select count(distinct ac.AccountNumber) CountCases , s.Name as StrategyName from FraudAnalysis.Strategy s
			inner join FraudAnalysis.StrategyExecution se on se.StrategyID= s.ID
			inner join FraudAnalysis.StrategyExecutionDetails sed on se.ID=sed.StrategyExecutionID
			inner join FraudAnalysis.AccountCase ac on sed.CaseId= ac.ID 
			
			where ac.CreatedTime between @FromDate and @ToDate   and ac.Status=3   
			group by  s.Name
			
				union 	
				
			select 0 as CountCases, s.Name as StrategyName from FraudAnalysis.Strategy s ) unionTable

		group by unionTable.StrategyName
		
	END