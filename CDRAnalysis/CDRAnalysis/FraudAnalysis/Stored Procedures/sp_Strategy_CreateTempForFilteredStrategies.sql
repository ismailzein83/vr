
CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_CreateTempForFilteredStrategies]
(
	@TempTableName varchar(200),	
	@Name Nvarchar(255),
	@Description Nvarchar(255)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
		
			SELECT FraudAnalysis.[Strategy].[Id],FraudAnalysis.[Strategy].[Name], FraudAnalysis.[Strategy].[StrategyContent]
			into #Result
			FROM FraudAnalysis.[Strategy] WHERE (@Name IS NULL OR FraudAnalysis.[Strategy].Name  LIKE '%' + @Name + '%' )
			AND (@Description IS NULL OR FraudAnalysis.[Strategy].Description  LIKE '%' + @Description + '%' )
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END