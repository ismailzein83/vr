


CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetAll] 

AS
BEGIN
	SELECT s.[ID] ,s.Name,s.[UserID] ,s.[Description],s.Settings,s.LastUpdatedOn  FROM [FraudAnalysis].[Strategy] s  WITH (NOLOCK)
END