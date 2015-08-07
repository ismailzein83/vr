
CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_CreateTempForFilteredStrategies]
(
	@TempTableName varchar(200),	
	@Name Nvarchar(255),
	@Description Nvarchar(255), 
	@PeriodsList Nvarchar(255)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	    
	    CREATE TABLE #Period(Id int, Description varchar(20))
	    
			IF(@PeriodsList = '')
				begin
					EXEC('INSERT INTO #Period SELECT Id,Description FROM [FraudAnalysis].Period ')
				end
			else
				begin
					EXEC('INSERT INTO #Period SELECT Id,Description FROM [FraudAnalysis].Period p WHERE p.Id IN ('+@PeriodsList+')')
				end
	    
	    		
			SELECT FraudAnalysis.[Strategy].[Id],FraudAnalysis.[Strategy].[Name], FraudAnalysis.[Strategy].[StrategyContent] , FraudAnalysis.[Strategy].[PeriodId]
			into #Result
			FROM FraudAnalysis.[Strategy]
			inner join #Period  on #Period.Id=FraudAnalysis.[Strategy].PeriodId
			WHERE (@Name IS NULL OR FraudAnalysis.[Strategy].Name  LIKE '%' + @Name + '%' )
			AND (@Description IS NULL OR FraudAnalysis.[Strategy].Description  LIKE '%' + @Description + '%' )
						
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END