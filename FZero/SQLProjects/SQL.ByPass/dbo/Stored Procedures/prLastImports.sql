




CREATE PROCEDURE [dbo].[prLastImports]

AS
BEGIN    

Select s.Name,s.DiffHour, s.VarImportDateTime from


(SELECT        dbo.Sources.Name, DATEDIFF(HOUR, 
                      max(dbo.Imports.ImportDate), GETDATE()) AS DiffHour, [dbo].[fn_DatePeriod]( max(dbo.Imports.ImportDate)) AS VarImportDateTime
FROM         dbo.Sources RIGHT OUTER JOIN
                      dbo.Imports INNER JOIN
                      dbo.GeneratedCalls ON dbo.Imports.ID = dbo.GeneratedCalls.ImportID ON dbo.Sources.ID = dbo.GeneratedCalls.SourceID 
GROUP BY  dbo.Sources.Name
UNION 
SELECT       dbo.Sources.Name, DATEDIFF(HOUR, 
                      max(dbo.Imports.ImportDate), GETDATE()) AS DiffHour, [dbo].[fn_DatePeriod]( max(dbo.Imports.ImportDate)) AS VarImportDateTime
FROM         dbo.Sources RIGHT OUTER JOIN
                      dbo.Imports INNER JOIN
                      dbo.RecievedCalls ON dbo.Imports.ID = dbo.RecievedCalls.ImportID ON dbo.Sources.ID = dbo.RecievedCalls.SourceID 
GROUP BY   dbo.Sources.Name) s

where s.Name<>'Unknown' 


END