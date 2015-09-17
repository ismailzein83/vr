
CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_CreateTempByFiltered]
(
	@TempTableName varchar(200),	
	@Name Nvarchar(255),
	@Description Nvarchar(255), 
	@PeriodIDs VARCHAR(MAX) = NULL,
	@UserIDs VARCHAR(MAX) = NULL,
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
			IF (@PeriodIDs IS NOT NULL)
			BEGIN
				DECLARE @PeriodIDsTable TABLE (PeriodID INT);
				INSERT INTO @PeriodIDsTable (PeriodID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@PeriodIDs);
			END
			
			IF (@UserIDs IS NOT NULL)
			BEGIN
				DECLARE @UserIDsTable TABLE (UserID INT);
				INSERT INTO @UserIDsTable (UserID)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@UserIDs);
			END
	    		
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
			
			AND (@PeriodIDs IS NULL OR s.PeriodId IN (SELECT PeriodID FROM @PeriodIDsTable))
			AND (@UserIDs IS NULL OR s.UserId IN (SELECT UserID FROM @UserIDsTable))
						
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END