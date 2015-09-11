



CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionDetails_Load] 

AS
BEGIN

SELECT distinct details.[AccountNumber]
  FROM [FraudAnalysis].[StrategyExecutionDetails] details with(nolock)
  WHERE	details.CaseID is null 
  ORDER BY details.AccountNumber
END