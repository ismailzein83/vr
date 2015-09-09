

CREATE PROCEDURE [FraudAnalysis].[sp_NumberProfile_CreateTempForFilteredNumberProfiles]
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
		(SELECT AccountNumber,
			FromDate,
			ToDate,
			AggregateValues,
			'Non Strategy Related' as StrategyName
			
        INTO #Result
        
		FROM FraudAnalysis.NumberProfile
		
		WHERE AccountNumber=@AccountNumber
			AND FromDate >= @FromDate
			AND ToDate <= @ToDate)
			
			
			union 
			
			(
			
			SELECT sed.AccountNumber, se.FromDate, se.ToDate, sed. [AggregateValues], s.Name StrategyName
     
  FROM [FraudAnalysis].[StrategyExecutionDetails] sed
    
  inner join [FraudAnalysis].[StrategyExecution] se on sed.StrategyExecutionID =  se.ID
  inner join [FraudAnalysis].[Strategy] s on s.ID=se.StrategyID
    
  where  se.FromDate >= @FromDate and se.ToDate<=@ToDate and sed.AccountNumber=@AccountNumber
			
			)
			
			
			
			
			
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
		EXEC(@sql)
	END
	
	SET NOCOUNT OFF
END