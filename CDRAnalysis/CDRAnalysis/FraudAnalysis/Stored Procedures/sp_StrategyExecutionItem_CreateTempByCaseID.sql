-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionItem_CreateTempByCaseID]
	@TempTableName VARCHAR(200),
	@CaseID INT
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	BEGIN
		SELECT d.ID AS DetailID,
			d.AccountNumber,
			d.SuspicionLevelID,
			s.Name AS StrategyName,
			d.SuspicionOccuranceStatus,
			e.FromDate,
			e.ToDate,
			e.ExecutionDate,
			d.AggregateValues,
		    d.StrategyExecutionID
		INTO #RESULT
		
		FROM FraudAnalysis.StrategyExecutionItem d WITH (NOLOCK)
		INNER JOIN FraudAnalysis.StrategyExecution e WITH (NOLOCK) ON d.StrategyExecutionID = e.ID
		INNER JOIN FraudAnalysis.Strategy s WITH (NOLOCK) ON e.StrategyID = s.Id
		
		WHERE  d.CaseID = @CaseID
	
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END