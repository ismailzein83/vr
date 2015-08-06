

CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_CreateTempForFilteredSubscriberThresholds]
(
	@TempTableName varchar(200),	
	@FromDate DATETIME,
	@ToDate DATETIME,
	@MSISDN varCHAR(100)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
		
		
			CREATE TABLE #SuspicionLevel(Id int, Name varchar(20))
			CREATE TABLE #Strategy(Id int, Name varchar(20))
			
			EXEC('INSERT INTO #SuspicionLevel SELECT Id,Name FROM [FraudAnalysis].Suspicion_Level')
			EXEC('INSERT INTO #Strategy SELECT Id,Name FROM [FraudAnalysis].Strategy ')
			
			
			
			SELECT [DateDay]  ,[SubscriberNumber]  ,CriteriaValues   ,#SuspicionLevel.Name as SuspicionLevelName  ,#Strategy.Name as StrategyName 
			into #Result
			FROM [FraudAnalysis].[SubscriberThreshold] st
			inner join #SuspicionLevel ON st.SuspicionLevelId=#SuspicionLevel.Id 
			inner join #Strategy ON #Strategy.Id=st.StrategyId 
			WHERE  SubscriberNumber=@MSISDN and SuspicionLevelId <> 0 and dateday between @FromDate and @ToDate  
			
		
			
			
			declare @sql varchar(1000)
			set @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #Result';
			exec(@sql)
			
		END
		
		SET NOCOUNT OFF
	END