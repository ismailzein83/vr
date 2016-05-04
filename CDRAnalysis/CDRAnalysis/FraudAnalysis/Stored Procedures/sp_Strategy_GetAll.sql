


CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetAll] 

AS
BEGIN
	SELECT s.[ID] ,s.[UserID] ,s.[StrategyContent]  FROM [FraudAnalysis].[Strategy] s  WITH (NOLOCK)
END