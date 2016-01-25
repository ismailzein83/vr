



Create PROCEDURE [FraudAnalysis].[sp_StrategyExecutionItem_GetByNULLCaseID] 

AS
BEGIN

SELECT max(details.[IMEIs]) as IMEIs, details.[AccountNumber]
  FROM [FraudAnalysis].[StrategyExecutionItem] details with(nolock)
  WHERE	details.CaseID is null 
  Group by details.[AccountNumber]
  ORDER BY details.AccountNumber
  
  
  
END