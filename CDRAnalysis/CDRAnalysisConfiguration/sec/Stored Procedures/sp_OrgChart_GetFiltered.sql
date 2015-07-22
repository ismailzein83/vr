
CREATE PROC [sec].[sp_OrgChart_GetFiltered]
@FromRow INT,
@ToRow INT,
@Name VARCHAR(100) = NULL
AS
BEGIN

SET NOCOUNT ON

;WITH OrgCharts_CTE (Id, Name, Hierarchy, RowNumber) AS (
	SELECT ID, Name, Hierarchy, ROW_NUMBER() OVER (ORDER BY Id ASC) AS RowNumber 
	FROM OrgChart
	WHERE
		(Name LIKE '%' + @Name + '%' OR @Name IS NULL)
)
	
SELECT Id, Name, Hierarchy, RowNumber 
FROM OrgCharts_CTE WHERE RowNumber BETWEEN @FromRow AND @ToRow

SET NOCOUNT OFF

END