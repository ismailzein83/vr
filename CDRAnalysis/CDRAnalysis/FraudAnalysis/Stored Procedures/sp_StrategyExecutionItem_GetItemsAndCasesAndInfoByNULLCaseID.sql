-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionItem_GetItemsAndCasesAndInfoByNULLCaseID]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;



  -- Select UnAssigned Info from Items into Temp Table
  SELECT max(items.[IMEIs]) as IMEIs, items.[AccountNumber] 
  INTO #Items
  FROM [FraudAnalysis].[StrategyExecutionItem] items with(nolock)
  WHERE	items.CaseID is null 
  Group by items.[AccountNumber]
  ORDER BY items.AccountNumber

  
  -- Select from Temp table the UnAssigned Items
   SELECT items.[AccountNumber]
      ,items.[IMEIs]as IMEIs
  FROM #Items items



  -- Select Cases of Account Numbers in Items with Null Cases
  SELECT cases.[ID] CaseID
      ,cases.[AccountNumber]
      ,cases.[UserID]
      ,cases.[Status] StatusID
      ,cases.[StatusUpdatedTime]
      ,cases.[ValidTill]
      ,cases.[CreatedTime]
      ,cases.[Reason]
  FROM [FraudAnalysis].[AccountCase] cases  WITH (NOLOCK)
  JOIN #Items items
  on cases.AccountNumber = items.AccountNumber


   -- Select Info of Account Numbers in Items with Null Cases
  SELECT info.[AccountNumber]
      ,info.[InfoDetail]
  FROM [FraudAnalysis].[AccountInfo] info  WITH (NOLOCK)
  JOIN #Items items
  on info.AccountNumber = items.AccountNumber
 
 
  
END