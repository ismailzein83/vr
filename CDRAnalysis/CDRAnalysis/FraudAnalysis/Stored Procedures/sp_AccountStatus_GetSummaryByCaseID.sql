
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_GetSummaryByCaseID]
	@CaseID VARCHAR(50),
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	SELECT        MAX(sed.SuspicionLevelID) AS SuspicionLevelID, CASE WHEN ac.[Status] IN (1, 2) THEN SUM(CASE WHEN sed.SuspicionOccuranceStatus = 1 THEN 1 ELSE 0 END) 
                         ELSE COUNT(*) END AS NumberOfOccurances, MAX(se.ExecutionDate) AS LastOccurance, ac.ID AS CaseID, ac.AccountNumber, ac.Status
	FROM            FraudAnalysis.StrategyExecutionItem AS sed INNER JOIN
                         FraudAnalysis.AccountCase AS ac ON sed.CaseID = ac.ID LEFT OUTER JOIN
                         FraudAnalysis.StrategyExecution AS se ON se.ID = sed.StrategyExecutionID
	WHERE ac.ID = @CaseID
	GROUP BY ac.ID, ac.AccountNumber, ac.Status
	
	SET NOCOUNT OFF;
END