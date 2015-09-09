-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_CreateTempByAccountNumberForSummaries]
	@TempTableName VARCHAR(200),
	@AccountNumber VARCHAR(50) = NULL,
	@From DATETIME,
	@To DATETIME,
	@SelectedStrategyIDs VARCHAR(MAX) = NULL,
	@SelectedSuspicionLevelIDs VARCHAR(MAX) = NULL,
	@SelectedCaseStatusIDs VARCHAR(MAX) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		WITH OpenAccounts AS
		(
			SELECT
				sed.AccountNumber,
				MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
				COUNT(*) AS NumberOfOccurances,
				MAX(se.ExecutionDate) AS LastOccurance,
				CASE WHEN ISNULL(accStatus.[Status], 0) IN (0, 2) THEN ISNULL(accStatus.[Status], 0) ELSE 0 END AS AccountStatusID
			  
			FROM FraudAnalysis.StrategyExecutionDetails sed
			INNER JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
			LEFT JOIN FraudAnalysis.AccountStatus accStatus ON accStatus.AccountNumber = sed.AccountNumber

			WHERE sed.SuspicionOccuranceStatus = 0
				AND se.FromDate >= @From
				AND se.FromDate <= @To
			GROUP BY sed.AccountNumber, accStatus.[Status]
		),
		      
		ClosedAccounts AS
		(
			SELECT
				sed.AccountNumber,
				MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
				COUNT(*) AS NumberOfOccurances,
				MAX(se.ExecutionDate) AS LastOccurance,
				accStatus.[Status] AS AccountStatusID

			FROM FraudAnalysis.AccountStatus accStatus 
			LEFT JOIN FraudAnalysis.StrategyExecutionDetails sed ON accStatus.AccountNumber = sed.AccountNumber
			LEFT JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
			LEFT JOIN OpenAccounts openAcc ON openAcc.AccountNumber = sed.AccountNumber 

			WHERE openAcc.AccountNumber IS NULL AND accStatus.Status IN (3, 4) --3: Fraud, 4: WhiteList
				AND se.FromDate >= @From
				AND se.FromDate <= @To
			GROUP BY sed.AccountNumber, accStatus.[Status]
		)

		SELECT * INTO #RESULT
		FROM (SELECT * FROM OpenAccounts UNION SELECT * FROM ClosedAccounts) AllAccounts
		WHERE AllAccounts.AccountStatusID = 0

		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
    END
END