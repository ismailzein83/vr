Create PROCEDURE [FraudAnalysis].[sp_Strategy_GetFilteredStrategies]
(
	@FromRow int ,
	@ToRow int,
	@Name Nvarchar(255),
	@Description Nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT ON

;WITH Strategies_CTE (Id, StrategyContent, RowNumber) AS 
	(
		SELECT dbo.[Strategy].[Id], dbo.[Strategy].[StrategyContent], ROW_NUMBER()  
		OVER ( ORDER BY  dbo.[Strategy].[Id] ASC) AS RowNumber 
			FROM dbo.[Strategy] 

				WHERE (@Name IS NULL OR [dbo].[Strategy].Name  LIKE '%' + @Name + '%' )
				AND (@Description IS NULL OR [dbo].[Strategy].Description  LIKE '%' + @Description + '%' )	
	)
	SELECT Id, StrategyContent, RowNumber
	FROM Strategies_CTE WHERE RowNumber between @FromRow AND @ToRow                           

SET NOCOUNT OFF

END