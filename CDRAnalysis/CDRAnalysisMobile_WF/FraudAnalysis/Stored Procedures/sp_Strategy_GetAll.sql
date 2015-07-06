



CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetAll] 
AS
BEGIN
SELECT [Id]
      ,[Description]
      ,[UserId]
      ,[CreationDate]
      ,[Name]
      ,[IsDefault]
      ,[StrategyContent]
  FROM FraudAnalysis.Strategy
END