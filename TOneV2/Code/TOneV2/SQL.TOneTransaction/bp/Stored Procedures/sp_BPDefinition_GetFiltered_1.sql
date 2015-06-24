

CREATE PROCEDURE  [bp].[sp_BPDefinition_GetFiltered]
@FromRow int ,
@ToRow int,
@Title varchar(255)
AS
BEGIN
	SET NOCOUNT ON

;WITH BPDefinitions_CTE (ID ,Name ,Title ,FQTN ,Config, RowNumber) AS 
	(
		SELECT [ID] ,Name ,Title ,[FQTN] ,[Config], ROW_NUMBER() OVER ( ORDER BY  [bp].[BPDefinition].[ID] ASC) AS RowNumber 
		FROM [bp].[BPDefinition] WITH(NOLOCK)
		Where  (Title like '%'+@Title +'%' or @Title is null)
	)
	SELECT ID ,Name ,Title ,FQTN ,Config, RowNumber
	FROM BPDefinitions_CTE WHERE RowNumber between @FromRow AND @ToRow                           

SET NOCOUNT OFF

END