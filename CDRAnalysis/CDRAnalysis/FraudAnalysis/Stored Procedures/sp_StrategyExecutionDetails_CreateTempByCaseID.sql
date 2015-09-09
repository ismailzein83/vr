-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_CreateTempByCaseID]
	@TempTableName VARCHAR(200),
	@AccountNumber VARCHAR(50),
	@CaseID INT,
	@FromDate DATETIME,
	@ToDate DATETIME
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
			e.ToDate
		
		INTO #RESULT
		
		FROM FraudAnalysis.StrategyExecutionDetails d
		INNER JOIN FraudAnalysis.StrategyExecution e ON d.StrategyExecutionID = e.ID
		INNER JOIN FraudAnalysis.Strategy s ON e.StrategyID = s.Id
		
		WHERE d.AccountNumber = @AccountNumber
			AND e.FromDate >= @FromDate
			AND e.ToDate <= @ToDate
	
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
		EXEC(@sql)
	END
END