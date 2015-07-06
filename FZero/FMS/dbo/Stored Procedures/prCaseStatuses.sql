


CREATE PROCEDURE [dbo].[prCaseStatuses]
(
 @StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
)
AS
BEGIN    

SELECT     COUNT(distinct dbo.GeneratedCalls.ID) AS CountGenerated,  dbo.Statuses.Name
FROM         dbo.Statuses LEFT OUTER JOIN
                      dbo.GeneratedCalls ON dbo.Statuses.ID = dbo.GeneratedCalls.StatusID
WHERE      (AttemptDateTime BETWEEN @StartDate AND @EndDate)
GROUP BY dbo.Statuses.Name


union 

SELECT     COUNT(distinct rc.CLI) AS CountGenerated,  'Distinct Fraud' Name
FROM         dbo.Statuses LEFT OUTER JOIN
                      dbo.GeneratedCalls gc ON dbo.Statuses.ID = gc.StatusID
                      inner join RecievedCalls rc on gc.ID=rc.GeneratedCallID
WHERE      (gc.AttemptDateTime BETWEEN @StartDate AND @EndDate) and StatusID=2
GROUP BY dbo.Statuses.Name


END