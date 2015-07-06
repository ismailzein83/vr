

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetStrategy] 
	@Id int
AS
BEGIN
	SELECT [Id]
      ,[Description]
      ,[UserId]
      ,[CreationDate]
      ,[Name]
      ,[IsDefault]
      ,[StrategyContent]
  FROM FraudAnalysis.[Strategy]
	WHERE Id = @Id
END