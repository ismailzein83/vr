


CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetFilteredStrategies]
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
		SELECT FraudAnalysis.[Strategy].[Id], FraudAnalysis.[Strategy].[StrategyContent], ROW_NUMBER()  
		OVER ( ORDER BY  FraudAnalysis.[Strategy].[Id] ASC) AS RowNumber 
			FROM FraudAnalysis.[Strategy] 

				WHERE (@Name IS NULL OR FraudAnalysis.[Strategy].Name  LIKE '%' + @Name + '%' )
				AND (@Description IS NULL OR FraudAnalysis.[Strategy].Description  LIKE '%' + @Description + '%' )	
	)
	SELECT Id, StrategyContent, RowNumber
	FROM Strategies_CTE WHERE RowNumber between @FromRow AND @ToRow                           

SET NOCOUNT OFF

END