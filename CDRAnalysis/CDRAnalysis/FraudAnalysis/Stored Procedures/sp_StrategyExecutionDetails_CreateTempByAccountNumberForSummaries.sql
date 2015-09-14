-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_CreateTempByAccountNumberForSummaries]
	@TempTableName VARCHAR(200),
	@AccountNumber VARCHAR(50) = NULL,
	@FromDate DATETIME,
	@ToDate DATETIME,
	@StrategyIDs VARCHAR(MAX) = NULL,
	@AccountStatusIDs VARCHAR(MAX) = NULL,
	@SuspicionLevelIDs VARCHAR(MAX) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		IF (@StrategyIDs IS NOT NULL)
		BEGIN
			DECLARE @StrategyIDsTable TABLE (StrategyID INT);
			INSERT INTO @StrategyIDsTable (StrategyID)
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@StrategyIDs);
		END
		
		IF (@AccountStatusIDs IS NOT NULL)
		BEGIN
			DECLARE @AccountStatusIDsTable TABLE (AccountStatusID INT);
			INSERT INTO @AccountStatusIDsTable (AccountStatusID)
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@AccountStatusIDs);
		END
		
		IF (@SuspicionLevelIDs IS NOT NULL)
		BEGIN
			DECLARE @SuspicionLevelIDsTable TABLE (SuspicionLevelID INT);
			INSERT INTO @SuspicionLevelIDsTable (SuspicionLevelID)
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@SuspicionLevelIDs);
		END
		
		;WITH OpenAccounts AS
		(
			SELECT
				sed.AccountNumber,
				MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
				COUNT(*) AS NumberOfOccurances,
				MAX(se.ExecutionDate) AS LastOccurance,
				CASE WHEN ISNULL(accStatus.[Status], 1) IN (1, 2) THEN ISNULL(accStatus.[Status], 1) ELSE 1 END AS AccountStatusID

			FROM FraudAnalysis.StrategyExecutionDetails sed
			INNER JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
			LEFT JOIN FraudAnalysis.AccountStatus accStatus ON accStatus.AccountNumber = sed.AccountNumber

			WHERE sed.SuspicionOccuranceStatus = 1
				AND se.ExecutionDate >= @FromDate AND se.ExecutionDate <= @ToDate
				AND (@AccountNumber IS NULL OR sed.AccountNumber = @AccountNumber)
				AND (@StrategyIDs IS NULL OR se.StrategyID IN (SELECT StrategyID FROM @StrategyIDsTable))
				AND (@SuspicionLevelIDs IS NULL OR sed.SuspicionLevelID IN (SELECT SuspicionLevelID FROM @SuspicionLevelIDsTable))

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
				AND se.ExecutionDate >= @FromDate AND se.ExecutionDate <= @ToDate
				AND (@AccountNumber IS NULL OR sed.AccountNumber = @AccountNumber)
				AND (@StrategyIDs IS NULL OR se.StrategyID IN (SELECT StrategyID FROM @StrategyIDsTable))
				AND (@SuspicionLevelIDs IS NULL OR sed.SuspicionLevelID IN (SELECT SuspicionLevelID FROM @SuspicionLevelIDsTable))

			GROUP BY sed.AccountNumber, accStatus.[Status]
		)

		SELECT * INTO #RESULT
		FROM (SELECT * FROM OpenAccounts UNION SELECT * FROM ClosedAccounts) AllAccounts
		
		WHERE (@AccountStatusIDs IS NULL OR AllAccounts.AccountStatusID IN (SELECT AccountStatusID FROM @AccountStatusIDsTable))

		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
    END
END