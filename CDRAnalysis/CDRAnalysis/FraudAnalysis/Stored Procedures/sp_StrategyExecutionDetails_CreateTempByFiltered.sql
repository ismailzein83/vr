-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_CreateTempByFiltered]
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
		IF (@SelectedStrategyIDs IS NOT NULL)
		BEGIN
			DECLARE @StrategyIDsTable TABLE (StrategyID INT);
			INSERT INTO @StrategyIDsTable (StrategyID)
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@SelectedStrategyIDs);
		END
		
		IF (@SelectedSuspicionLevelIDs IS NOT NULL)
		BEGIN
			DECLARE @SuspicionLevelIDsTable TABLE (SuspicionLevelID INT);
			INSERT INTO @SuspicionLevelIDsTable (SuspicionLevelID)
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@SelectedSuspicionLevelIDs);
		END
		
		IF (@SelectedCaseStatusIDs IS NOT NULL)
		BEGIN
			DECLARE @CaseStatusIDsTable TABLE (CaseStatusID INT);
			INSERT INTO @CaseStatusIDsTable (CaseStatusID)
			SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@SelectedCaseStatusIDs);
		END
		
		SELECT
			sed.AccountNumber,
			MAX(sed.SuspicionLevelID) AS SuspicionLevelID,
			COUNT(*) AS NumberOfOccurances,
			MAX(se.FromDate) AS LastOccurance,
			ISNULL(MAX(accStatus.[Status]), 0) AS AccountStatusID
		
		INTO #RESULT
		
		FROM FraudAnalysis.StrategyExecutionDetails sed
		INNER JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
		LEFT JOIN FraudAnalysis.AccountStatus accStatus ON accStatus.AccountNumber = sed.AccountNumber
		
		WHERE
			--fr.[Status] = 0
			(se.FromDate >= @From AND se.ToDate <= @To)
			AND (@AccountNumber IS NULL OR sed.AccountNumber = @AccountNumber)
			AND (@SelectedSuspicionLevelIDs IS NULL OR sed.SuspicionLevelID IN (SELECT SuspicionLevelID FROM @SuspicionLevelIDsTable))
			AND (@SelectedCaseStatusIDs IS NULL OR accStatus.[Status] IN (SELECT CaseStatusID FROM @CaseStatusIDsTable))
		
		GROUP BY sed.AccountNumber
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
    END
END