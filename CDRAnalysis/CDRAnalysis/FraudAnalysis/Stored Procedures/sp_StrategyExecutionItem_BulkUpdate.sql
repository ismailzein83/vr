﻿




-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_StrategyExecutionItem_BulkUpdate]
	@StrategyExecutionItem [FraudAnalysis].StrategyExecutionItemType READONLY
AS
BEGIN

	
	UPDATE [FraudAnalysis].[StrategyExecutionItem] 
	SET 
		StrategyExecutionItem.CaseID = item.CaseID

	FROM [FraudAnalysis].[StrategyExecutionItem]  inner join @StrategyExecutionItem as item ON  StrategyExecutionItem.AccountNumber = item.AccountNumber and StrategyExecutionItem.CaseID is null
	
	
END