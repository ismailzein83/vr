


CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_GetSubscriberThresholds]
(
	@FromRow int ,
	@ToRow int,
	@FromDate DATETIME,
	@ToDate DATETIME,
	@MSISDN varCHAR(100)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		CREATE TABLE #SuspicionLevel(Id int, Name varchar(20))
		CREATE TABLE #Strategy(Id int, Name varchar(20))
		
		EXEC('INSERT INTO #SuspicionLevel SELECT Id,Name FROM [FraudAnalysis].Suspicion_Level')
		EXEC('INSERT INTO #Strategy SELECT Id,Name FROM [FraudAnalysis].Strategy ')
		
		
		;WITH SubscriberThresholds_CTE ( DateDay  ,SubscriberNumber  ,Criteria1  ,Criteria2  ,Criteria3  ,Criteria4  ,Criteria5  ,Criteria6 ,Criteria7  ,Criteria8  ,Criteria9  ,Criteria10  ,Criteria11  ,Criteria12  ,Criteria13  ,Criteria14 ,Criteria15  ,Criteria16 ,Criteria17  ,Criteria18  , SuspicionLevelName  , StrategyName ,  RowNumber) AS 
			(
				SELECT [DateDay]  ,[SubscriberNumber]  ,[Criteria1]  ,[Criteria2]  ,[Criteria3]  ,[Criteria4]  ,[Criteria5]  ,[Criteria6]  ,[Criteria7]  ,[Criteria8]  ,[Criteria9]  ,[Criteria10]  ,[Criteria11]  ,[Criteria12]  ,[Criteria13]  ,[Criteria14] ,[Criteria15]  ,[Criteria16] ,[Criteria17]  ,[Criteria18]   ,#SuspicionLevel.Name as SuspicionLevelName  ,#Strategy.Name as StrategyName , ROW_NUMBER() OVER ( ORDER BY  DateDay ASC) AS RowNumber FROM [FraudAnalysis].[SubscriberThreshold] st
				inner join #SuspicionLevel ON st.SuspicionLevelId=#SuspicionLevel.Id 
				inner join #Strategy ON #Strategy.Id=st.StrategyId 
				WHERE  SubscriberNumber=@MSISDN and SuspicionLevelId <> 0 and dateday between @FromDate and @ToDate  
			)
			
		SELECT DateDay  ,SubscriberNumber  ,Criteria1  ,Criteria2  ,Criteria3  ,Criteria4  ,Criteria5  ,Criteria6 ,Criteria7  ,Criteria8  ,Criteria9  ,Criteria10  ,Criteria11  ,Criteria12  ,Criteria13  ,Criteria14 ,Criteria15  ,Criteria16 ,Criteria17  ,Criteria18   , SuspicionLevelName  , StrategyName ,  RowNumber
		FROM SubscriberThresholds_CTE WHERE RowNumber between @FromRow AND @ToRow  

		SET NOCOUNT OFF
		/*

		exec [FraudAnalysis].[sp_FraudResult_GetSubscriberThresholds]
			@FromRow =1 ,
			@ToRow =1000,
			@FromDate ='2015-03-14 03:10:05.000',
			@ToDate ='2015-03-14 03:10:15.000',
			@MSISDN='202010904977227'
		*/
	END