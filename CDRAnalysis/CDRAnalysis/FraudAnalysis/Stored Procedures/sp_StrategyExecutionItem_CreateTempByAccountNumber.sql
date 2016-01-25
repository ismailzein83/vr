
Create PROCEDURE [FraudAnalysis].[sp_StrategyExecutionItem_CreateTempByAccountNumber]
	@TempTableName VARCHAR(200),
	@CaseID int,
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
			se.ExecutionDate,
			sed.AggregateValues
			
		INTO #RESULT
			
		FROM FraudAnalysis.StrategyExecutionItem sed WITH (NOLOCK)
	    inner join FraudAnalysis.StrategyExecution se WITH (NOLOCK) on se.ID = sed.StrategyExecutionID
		inner join FraudAnalysis.Strategy s WITH (NOLOCK) ON se.StrategyID = s.Id
		
		WHERE sed.CaseID = @CaseID			
		
		ORDER BY se.ExecutionDate DESC
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END