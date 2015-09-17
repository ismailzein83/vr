-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountStatus_GetSummaryByAccountNumber]
	@AccountNumber VARCHAR(50),
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
		accStatus.AccountNumber,
		MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
		CASE WHEN accStatus.[Status] IN (1, 2) THEN SUM (CASE WHEN sed.SuspicionOccuranceStatus = 1 THEN 1 ELSE 0 END) ELSE COUNT(*) END AS NumberOfOccurances,
		MAX(se.ExecutionDate) AS LastOccurance,
		accStatus.[Status] AS AccountStatusID
	
	FROM FraudAnalysis.AccountStatus accStatus
	LEFT JOIN FraudAnalysis.StrategyExecutionDetails sed ON accStatus.AccountNumber = sed.AccountNumber
	LEFT JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
	
	WHERE accStatus.AccountNumber = @AccountNumber
	
	GROUP BY accStatus.AccountNumber, accStatus.[Status]
	
	SET NOCOUNT OFF;
END