

CREATE PROCEDURE [FraudAnalysis].[sp_NumberProfile_CreateTempByAccountNumber]
(
	@TempTableName VARCHAR(200),
	@FromDate DateTime,
	@ToDate DateTime,
	@AccountNumber VARCHAR(100) 
)
AS
BEGIN
	SET NOCOUNT ON
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
    BEGIN
		SELECT AccountNumber,
			FromDate,
			ToDate,
			AggregateValues,
			'Non Strategy Related' as StrategyName
			
        INTO #Result
        
		FROM FraudAnalysis.NumberProfile np
		
		WHERE np.AccountNumber=@AccountNumber and (@FromDate is null or np.FromDate >=@FromDate )and  (@ToDate is null or np.ToDate<=@ToDate)
		
		DECLARE @sql VARCHAR(1000)
		SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
		EXEC(@sql)
	END
	
	SET NOCOUNT OFF
END