



CREATE PROCEDURE [dbo].[db_findSuspectionOccurance]
	(
	@fromDate datetime,
	@ToDate DATETIME,
	@strategyId INT,
	@SuspectionList varCHAR(10),
	@MinimumOccurance int
	)
AS
CREATE TABLE #suspectionLevel(id int)
EXEC('INSERT INTO #suspectionLevel SELECT id FROM Suspection_Level sl WHERE sl.Id IN ('+@SuspectionList+')')

CREATE TABLE #temp
(
 SubscriberNumber VARCHAR(50),SuspectionLevelId INT
,SuspectionLevelName VARCHAR(20),[Number_of_occurance] INT
,LastReportNumber VARCHAR(12)
 )
INSERT INTO #temp(SubscriberNumber,SuspectionLevelId,SuspectionLevelName,Number_of_occurance,LastReportNumber)
select st.SubscriberNumber,st.SuspectionLevelId,sl.[Name] AS SuspectionLevelName 
,COUNT( st.SuspectionLevelId) as [Number_of_occurance]
,'' 
from [SubscriberThresholds] st
INNER JOIN Suspection_Level sl ON st.SuspectionLevelId=sl.Id 
inner join #suspectionLevel tsl on   st.SuspectionLevelId=tsl.id
WHERE StrategyId=@strategyId AND SuspectionLevelId <> 0 and dateday between @fromDate and @ToDate
group by SubscriberNumber,SuspectionLevelId,sl.[Name] 
 --having COUNT( st.SuspectionLevelId) >= @MinimumOccurance
 
 UPDATE t
 SET t.LastReportNumber = isnull(( SELECT TOP 1 r.ReportNumber
                            FROM  Report r INNER JOIN ReportDetails rd ON r.Id=rd.ReportId 
                            WHERE rd.SubscriberNumber=t.SubscriberNumber ORDER BY r.Id DESC ),'')
 FROM #temp t
 SELECT @strategyId as strategyId, t.*, s.Name as StrategyName FROM #temp t inner join Strategy s on s.Id=@strategyId
 where Number_of_occurance >= @MinimumOccurance
 order by 2 desc,4 desc

/*

[prfindSuspectionOccurance]
	@strategyId=1,
	@fromDate ='05/01/2014',
	@ToDate ='08/01/2014',
	@SuspectionList='2,3,4',
    @MinimumOccurance=01
*/