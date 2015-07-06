


CREATE PROCEDURE [dbo].[prImports]
(
 @StartDate DATETIME = NULL
,@EndDate DATETIME = NULL
)
AS
BEGIN    


SELECT     s.CountImportedCalls,  s.ImportID, s.Name, s.VarImportDateTime    from 


(SELECT     COUNT(dbo.GeneratedCalls.ID) AS CountImportedCalls, dbo.Imports.ID AS ImportID,  dbo.Sources.Name, [dbo].[fn_DatePeriod](dbo.Imports.ImportDate) AS VarImportDateTime
FROM         dbo.Sources RIGHT OUTER JOIN
                      dbo.Imports INNER JOIN
                      dbo.GeneratedCalls ON dbo.Imports.ID = dbo.GeneratedCalls.ImportID ON dbo.Sources.ID = dbo.GeneratedCalls.SourceID 
WHERE     (Imports.ImportDate BETWEEN @StartDate AND @EndDate)
GROUP BY dbo.Imports.ID, dbo.Imports.ImportDate, dbo.Sources.Name
UNION 
SELECT     COUNT(dbo.RecievedCalls.ID) AS CountImportedCalls, dbo.Imports.ID AS ImportID,  dbo.Sources.Name,  [dbo].[fn_DatePeriod](dbo.Imports.ImportDate) AS VarImportDateTime
FROM         dbo.Sources RIGHT OUTER JOIN
                      dbo.Imports INNER JOIN
                      dbo.RecievedCalls ON dbo.Imports.ID = dbo.RecievedCalls.ImportID ON dbo.Sources.ID = dbo.RecievedCalls.SourceID
WHERE     (Imports.ImportDate BETWEEN @StartDate AND @EndDate)
GROUP BY dbo.Imports.ID, dbo.Imports.ImportDate, dbo.Sources.Name) s

order by ImportID desc





END