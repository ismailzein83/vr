
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_GetByAccountNumber]
	@TempTableName VARCHAR(200),
	@AccountNumber VARCHAR(50),
	@From DATETIME,
	@To DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SELECT
			sed.ID AS DetailID,
			sed.AccountNumber,
			sed.SuspicionLevelId AS SuspicionLevelID,
			s.Name AS StrategyName,
			sed.SuspicionOccuranceStatus AS SuspicionOccuranceStatus,
			se.FromDate,
			se.ToDate
			
		INTO #RESULT
			
		FROM FraudAnalysis.StrategyExecutionDetails sed
		    inner join FraudAnalysis.StrategyExecution se on se.ID = sed.StrategyExecutionID
			inner join FraudAnalysis.Strategy s ON se.StrategyID = s.Id
		
		WHERE sed.AccountNumber = @AccountNumber
			AND (se.FromDate >= @From AND se.ToDate <= @To)
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END