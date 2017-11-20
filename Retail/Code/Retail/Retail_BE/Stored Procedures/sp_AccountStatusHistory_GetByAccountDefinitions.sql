
CREATE PROCEDURE [Retail_BE].[sp_AccountStatusHistory_GetByAccountDefinitions]
@AccountDefinitions [Retail_BE].[AccountDefinitionsType] Readonly
AS
BEGIN

	SELECT [ID],accountStatusHistory.[AccountBEDefinitionID],accountStatusHistory.[AccountID],[StatusID],[PreviousStatusID],[StatusChangedDate]
	FROM [Retail_BE].[AccountStatusHistory] accountStatusHistory with(nolock)
	Join @AccountDefinitions accountDefinition on accountDefinition.AccountBEDefinitionID = accountStatusHistory.AccountBEDefinitionID 
	And accountDefinition.AccountID >= accountStatusHistory.AccountID 
	AND ISNULL(accountStatusHistory.IsDeleted,0) = 0
END