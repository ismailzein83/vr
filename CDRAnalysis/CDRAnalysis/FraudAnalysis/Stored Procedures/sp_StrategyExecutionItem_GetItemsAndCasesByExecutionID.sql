-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionItem_GetItemsAndCasesByExecutionID]
	@ExecutionID BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  SELECT exItem.[ID]
      ,exItem.[StrategyExecutionID]
      ,exItem.[AccountNumber]
      ,exItem.[SuspicionLevelID]
      ,exItem.[FilterValues]
      ,exItem.[AggregateValues]
      ,exItem.[CaseID]
      ,exItem.[SuspicionOccuranceStatus]
      ,exItem.[IMEIs]
  FROM [FraudAnalysis].[StrategyExecutionItem] exItem
  WHERE exItem.StrategyExecutionID = @ExecutionID

--retrieve match cases and related execution items
  SELECT distinct ac.[ID] CaseID
      ,ac.[AccountNumber]
      ,ac.[UserID]
      ,ac.[Status] StatusID
      ,ac.[StatusUpdatedTime]
      ,ac.[ValidTill]
      ,ac.[CreatedTime]
      ,ac.[Reason]
      INTO #AccountCases
  FROM [FraudAnalysis].[AccountCase] ac
  JOIN StrategyExecutionItem exItem ON ac.ID = exItem.CaseID
  WHERE exItem.StrategyExecutionID = @ExecutionID
    
  SELECT * FROM #AccountCases
  
  SELECT exItem.[ID]
      ,exItem.[StrategyExecutionID]
      ,exItem.[AccountNumber]
      ,exItem.[SuspicionLevelID]
      ,exItem.[FilterValues]
      ,exItem.[AggregateValues]
      ,exItem.[CaseID]
      ,exItem.[SuspicionOccuranceStatus]
      ,exItem.[IMEIs]
  FROM [FraudAnalysis].[StrategyExecutionItem] exItem
  JOIN #AccountCases ac on exItem.CaseID = ac.CaseID  
  
END