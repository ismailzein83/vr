
CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_CreateTempForFilteredStrategies]
(
	@TempTableName varchar(200),	
	@Name Nvarchar(255),
	@Description Nvarchar(255), 
	@PeriodsList Nvarchar(255),
	@UsersList Nvarchar(255),
	@IsDefault bit,
	@IsEnabled bit,
	@FromDate datetime,
	@ToDate datetime
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	    		
			SELECT s.[Id],s.[Name], s.[StrategyContent] , s.[UserId], s.[PeriodId], p.Description as StrategyType
			into #Result
			FROM FraudAnalysis.[Strategy] s
			inner join FraudAnalysis.Period p on p.Id=s.PeriodId
			WHERE (@Name IS NULL OR s.Name  LIKE '%' + @Name + '%' )
			AND (@Description IS NULL OR s.Description  LIKE '%' + @Description + '%' )
			and (@IsDefault is null or s.IsDefault=@IsDefault)
			and (@IsEnabled is null or s.IsEnabled=@IsEnabled)
			and 
			(( @FromDate is null and @ToDate is null )
			or ( @FromDate is not null and @ToDate is null and  s.LastUpdatedOn >= @FromDate)
			or ( @FromDate is null and @ToDate is not null and  s.LastUpdatedOn <= @ToDate)
			or ( @FromDate is not null and @ToDate is not null and  s.LastUpdatedOn between @FromDate and @ToDate))
			
			and (@PeriodsList ='' or  s.PeriodId in (SELECT * FROM dbo.CSVToTable(@PeriodsList)))
			and (@UsersList ='' or  s.UserId in (SELECT * FROM dbo.CSVToTable(@UsersList)))
						
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END