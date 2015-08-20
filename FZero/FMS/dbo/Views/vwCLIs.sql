
CREATE VIEW [dbo].[vwCLIs]
AS
SELECT	dbo.RecievedCalls.CLI, 0 ID
FROM	dbo.GeneratedCalls  with(nolock)
		LEFT JOIN	dbo.RecievedCalls with(nolock) ON dbo.GeneratedCalls.ID = dbo.RecievedCalls.GeneratedCallID
WHERE   dbo.GeneratedCalls.ReportingStatusID = 2
union 
select RelatedNumber as CLI, 0 as ID from RelatedNumbers
union 
SELECT	dbo.RecievedCalls.CLI, 0 ID
FROM    dbo.GeneratedCalls  with(nolock)
		LEFT JOIN	dbo.RecievedCalls  with(nolock) ON dbo.GeneratedCalls.ID = dbo.RecievedCalls.GeneratedCallID
WHERE   dbo.GeneratedCalls.ReportingStatusID = 4