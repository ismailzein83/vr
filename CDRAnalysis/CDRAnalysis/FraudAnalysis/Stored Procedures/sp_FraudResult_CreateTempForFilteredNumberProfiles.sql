

CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_CreateTempForFilteredNumberProfiles]
(
	@TempTableName VARCHAR(200),	
	@FromDate DATETIME,
	@ToDate DATETIME,
	@AccountNumber VARCHAR(100)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
			SELECT FromDate,
				np.ToDate,
				np.StrategyId,
				s.Name AS StrategyName,
				np.AccountNumber,
				np.AggregateValues
				
            INTO #Result
            
			FROM FraudAnalysis.NumberProfile np
				LEFT JOIN FraudAnalysis.Strategy s ON s.Id = np.StrategyId
			
			WHERE AccountNumber=@AccountNumber
				AND FromDate >= @FromDate
				AND ToDate <= @ToDate
			
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			EXEC(@sql)
			
		END
		
		SET NOCOUNT OFF
	END