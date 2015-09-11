
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_CreateTempByAccountNumber]
	@TempTableName VARCHAR(200),
	@AccountNumber VARCHAR(50),
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SELECT
			sed.ID AS DetailID,
			sed.SuspicionLevelID,
			s.Name AS StrategyName,
			sed.SuspicionOccuranceStatus,
			se.FromDate,
			se.ToDate,
			sed.AggregateValues
			
		INTO #RESULT
			
		FROM FraudAnalysis.StrategyExecutionDetails sed
		    inner join FraudAnalysis.StrategyExecution se on se.ID = sed.StrategyExecutionID
			inner join FraudAnalysis.Strategy s ON se.StrategyID = s.Id
		
		WHERE sed.AccountNumber = @AccountNumber
			AND sed.SuspicionOccuranceStatus = 0
			AND se.ExecutionDate >= @FromDate AND se.ExecutionDate <= @ToDate
		
		ORDER BY se.ExecutionDate DESC
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END