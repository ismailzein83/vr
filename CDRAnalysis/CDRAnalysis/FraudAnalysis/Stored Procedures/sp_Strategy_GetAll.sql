

CREATE PROCEDURE [FraudAnalysis].[sp_Strategy_GetAll] 
(
	@PeriodId int
)
AS
BEGIN
SELECT [Id]
      ,[Description]
      ,[UserId]
      ,[CreationDate]
      ,[Name]
      ,[IsDefault]
      ,[PeriodId]
      ,[StrategyContent]
  FROM FraudAnalysis.Strategy
  Where (@PeriodId =0 or PeriodId = @PeriodId)
END