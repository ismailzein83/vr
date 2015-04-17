

CREATE PROCEDURE [dbo].[prSourceGenerates]
(
 @StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
)
AS
BEGIN    

Select     COUNT(dbo.GeneratedCalls.ID) AS CountGenerated, DATEADD(dd, 0, DATEDIFF(dd, 0, dbo.GeneratedCalls.AttemptDateTime)) AS attemptdate, 
                      dbo.Sources.Name
FROM         dbo.Sources INNER JOIN
                      dbo.GeneratedCalls ON dbo.Sources.ID = dbo.GeneratedCalls.SourceID

WHERE      (AttemptDateTime BETWEEN @StartDate AND @EndDate)
GROUP BY DATEADD(dd, 0, DATEDIFF(dd, 0, dbo.GeneratedCalls.AttemptDateTime)), dbo.Sources.Name
ORDER BY attemptdate DESC

END