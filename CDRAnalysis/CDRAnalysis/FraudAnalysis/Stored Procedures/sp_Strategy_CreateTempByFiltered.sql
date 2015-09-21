
CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_CreateTempByFiltered]
(
	@TempTableName varchar(200),	
	@Name NVARCHAR(255) = NULL,
	@Description NVARCHAR(255) = NULL, 
	@PeriodIDs VARCHAR(MAX) = NULL,
	@UserIDs VARCHAR(MAX) = NULL,
	@IsDefault VARCHAR(MAX) = NULL,
	@IsEnabled VARCHAR(MAX) = NULL,
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
			
			IF (@IsDefault IS NOT NULL)
			BEGIN
				DECLARE @IsDefaultTable TABLE (Value INT);
				INSERT INTO @IsDefaultTable (Value)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@IsDefault);
			END
			
			IF (@IsEnabled IS NOT NULL)
			BEGIN
				DECLARE @IsEnabledTable TABLE (Value INT);
				INSERT INTO @IsEnabledTable (Value)
				SELECT CONVERT(INT, ParsedString) FROM [FraudAnalysis].[ParseStringList](@IsEnabled);
			END
	    		
			SELECT s.[Id],
				s.[Name],
				s.[StrategyContent],
				s.[UserId],
				s.[PeriodId],
				p.[Description] AS StrategyType,
				s.IsEnabled,
				s.LastUpdatedOn
				
			into #Result
			
			FROM FraudAnalysis.[Strategy] s
			INNER JOIN FraudAnalysis.Period p ON p.ID = s.PeriodID
			
			WHERE (@Name IS NULL OR s.Name  LIKE '%' + @Name + '%' )
			AND (@Description IS NULL OR s.Description  LIKE '%' + @Description + '%' )
			
			AND (@IsDefault IS NULL OR s.IsDefault IN (SELECT Value FROM @IsDefaultTable))
			AND (@IsEnabled IS NULL OR s.IsEnabled IN (SELECT Value FROM @IsEnabledTable))
			
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