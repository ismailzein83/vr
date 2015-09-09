-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_GetSummaryByAccountNumber]
	@AccountNumber VARCHAR(50),
	@From DATETIME,
	@To DATETIME
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
		sed.AccountNumber,
		MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
		COUNT(*) AS NumberOfOccurances,
		MAX(se.FromDate) AS LastOccurance,
		ISNULL(MAX(accStatus.[Status]), 0) AS AccountStatusID
	
	FROM FraudAnalysis.StrategyExecutionDetails sed
	INNER JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
	LEFT JOIN FraudAnalysis.AccountStatus accStatus ON accStatus.AccountNumber = sed.AccountNumber -- I think that using an INNER JOIN is better in this case...
	
	WHERE sed.AccountNumber = @AccountNumber
		AND (se.FromDate >= @From AND se.ToDate <= @To)
	
	GROUP BY sed.AccountNumber
END