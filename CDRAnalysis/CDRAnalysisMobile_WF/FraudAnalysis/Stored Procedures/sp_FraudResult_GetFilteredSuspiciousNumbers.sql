



CREATE PROCEDURE [FraudAnalysis].[sp_FraudResult_GetFilteredSuspiciousNumbers]
(
	@FromRow int ,
	@ToRow int,
	@FromDate datetime,
	@ToDate DATETIME,
	@StrategyId INT,
	@SuspicionList varCHAR(10)
)
	AS
	BEGIN
		SET NOCOUNT ON
		
		CREATE TABLE #SuspectionLevel(id int)
		EXEC('INSERT INTO #SuspectionLevel SELECT id FROM [FraudAnalysis].Suspicion_Level sl WHERE sl.Id IN ('+@SuspicionList+')')


		;WITH SuspiciousNumbers_CTE (DateDay, SubscriberNumber, SuspicionLevelName, RowNumber) AS 
			(
				SELECT st.DateDay, st.SubscriberNumber as SubscriberNumber , sl.Name AS SuspicionLevelName  , ROW_NUMBER() OVER ( ORDER BY  st.SubscriberNumber ASC) AS RowNumber 
					from [FraudAnalysis].[SubscriberThreshold] st
					INNER JOIN [FraudAnalysis].Suspicion_Level sl ON st.SuspicionLevelId=sl.Id 
					inner join #SuspectionLevel tsl on   st.SuspicionLevelId=tsl.id
					WHERE StrategyId=@strategyId AND SuspicionLevelId <> 0 and dateday between @fromDate and @ToDate
					group by SubscriberNumber,SuspicionLevelId,sl.[Name] , DateDay		
			)
			SELECT DateDay, SubscriberNumber, SuspicionLevelName, RowNumber
			FROM SuspiciousNumbers_CTE WHERE RowNumber between @FromRow AND @ToRow                           

		SET NOCOUNT OFF
		/*

		exec [FraudAnalysis].[sp_Results_GetFilteredSuspiciousNumbers]
			@FromRow =1 ,
			@ToRow =1000,
			@StrategyId=4,
			@FromDate ='12/01/2013',
			@ToDate ='12/10/2018',
			@SuspicionList='4,3,2'
		*/
	END