

CREATE PROCEDURE [dbo].[prComparisons]
(
 @StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
)
AS
BEGIN    


select s.* from 


(SELECT DISTINCT 
                      [dbo].[fn_DatePeriod](LevelOneComparisonDateTime) AS VarCompareDateTime, LevelOneComparisonDateTime AS CompareDateTime, 'Level One Compare' AS Type, 
                      COUNT(ID) AS CountCalls
FROM         GeneratedCalls
WHERE     levelonecomparisondatetime IS NOT NULL and (AttemptDateTime BETWEEN @StartDate AND @EndDate)
GROUP BY LevelOneComparisonDateTime
UNION ALL
SELECT DISTINCT 
                      [dbo].[fn_DatePeriod](LevelTwoComparisonDateTime) AS VarCompareDateTime, LevelTwoComparisonDateTime AS CompareDateTime, 'Level Two Compare' AS Type, 
                      COUNT(ID) AS CountCalls
FROM         GeneratedCalls AS GeneratedCalls_1
WHERE     leveltwocomparisondatetime IS NOT NULL and (AttemptDateTime BETWEEN @StartDate AND @EndDate)
GROUP BY LevelTwoComparisonDateTime) s


order by s.CompareDateTime desc







END