




CREATE PROCEDURE [dbo].[prfindSuspicionOccurrence]
	(
	@fromDate datetime,
	@ToDate DATETIME,
	@strategyId INT,
	@SuspectionList varCHAR(10),
	@MinimumOccurance int
	)
AS
CREATE TABLE #suspectionLevel(id int)
EXEC('INSERT INTO #suspectionLevel SELECT id FROM Suspicion_Level sl WHERE sl.Id IN ('+@SuspectionList+')')

CREATE TABLE #temp
(
 SubscriberNumber VARCHAR(50),SuspectionLevelId INT
,SuspectionLevelName VARCHAR(20)
,Occurrence_Day INT
,Occurrence_Hour INT
,LastReportNumber VARCHAR(12)
 )
INSERT INTO #temp(SubscriberNumber,SuspectionLevelId,SuspectionLevelName,Occurrence_Day, Occurrence_Hour ,LastReportNumber)
select st.SubscriberNumber,st.SuspectionLevelId,sl.[Name] AS SuspectionLevelName 
,COUNT( CASE WHEN st.PeriodId=1 then st.SuspectionLevelId end) as [Occurrence_Day]
,COUNT( CASE WHEN st.PeriodId=6 then st.SuspectionLevelId end) as [Occurrence_Hour]
,'' 
from [SubscriberThresholds] st
INNER JOIN Suspicion_Level sl ON st.SuspectionLevelId=sl.Id 
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
 where (Occurrence_Hour >= @MinimumOccurance OR Occurrence_Day > 0)
 order by 6 desc

/*

[prfindSuspicionoccurrence]
	@strategyId=34,
	@fromDate ='12/01/2014',
	@ToDate ='12/10/2014',
	@SuspectionList='3',
    @MinimumOccurance=01
*/