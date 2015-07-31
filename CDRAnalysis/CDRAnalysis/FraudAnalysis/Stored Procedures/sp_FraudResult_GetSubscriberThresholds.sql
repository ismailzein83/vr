


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
		
		
		;WITH SubscriberThresholds_CTE ( DateDay  ,SubscriberNumber  ,CriteriaValues  , SuspicionLevelName  , StrategyName ,  RowNumber) AS 
			(
				SELECT [DateDay]  ,[SubscriberNumber]  ,CriteriaValues   ,#SuspicionLevel.Name as SuspicionLevelName  ,#Strategy.Name as StrategyName , ROW_NUMBER() OVER ( ORDER BY  DateDay ASC) AS RowNumber FROM [FraudAnalysis].[SubscriberThreshold] st
				inner join #SuspicionLevel ON st.SuspicionLevelId=#SuspicionLevel.Id 
				inner join #Strategy ON #Strategy.Id=st.StrategyId 
				WHERE  SubscriberNumber=@MSISDN and SuspicionLevelId <> 0 and dateday between @FromDate and @ToDate  
			)
			
		SELECT DateDay  ,SubscriberNumber  ,CriteriaValues   , SuspicionLevelName  , StrategyName ,  RowNumber
		FROM SubscriberThresholds_CTE WHERE RowNumber between @FromRow AND @ToRow  

		SET NOCOUNT OFF
		/*

		exec [FraudAnalysis].[sp_FraudResult_GetSubscriberThresholds]
			@FromRow =1 ,
			@ToRow =1000,
			@FromDate ='2015-01-14 03:10:05.000',
			@ToDate ='2015-12-14 03:10:15.000',
			@MSISDN='202059052924790'
		*/
	END