
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_GetDetailsByID]
	@CaseID int
AS
BEGIN
	
	SELECT     ac.ID AS CaseID, ac.AccountNumber, ac.[Status], 
               COUNT(sed.ID) AS NumberOfOccurances,
               MAX(sed.SuspicionLevelID) AS SuspicionLevelID, 
               MAX(se.ExecutionDate) AS LastOccurance,
			   Max(sed.StrategyExecutionID) as StrategyExecutionID
	FROM FraudAnalysis.AccountCase ac WITH (NOLOCK)
    LEFT JOIN FraudAnalysis.StrategyExecutionItem AS sed WITH (NOLOCK) ON sed.CaseID = ac.ID
    LEFT JOIN FraudAnalysis.StrategyExecution AS se WITH (NOLOCK) ON se.ID = sed.StrategyExecutionID
	WHERE ac.ID = @CaseID
	GROUP BY ac.ID, ac.AccountNumber, ac.Status
END