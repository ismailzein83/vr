

CREATE Procedure [Retail].[sp_Account_GetByDefinition]
	@DefinitionId uniqueidentifier
AS
BEGIN
	SELECT	a.ID, a.Name, a.TypeID, a.Settings, a.StatusID, a.ParentID, a.SourceID, a.ExecutedActionsData,a.ExtendedSettings,a.CreatedTime, a.CreatedBy, a.LastModifiedBy, a.LastModifiedTime
	FROM [Retail].[Account] a with(nolock)
	Join [Retail_BE].[AccountType] t with(nolock)
	on a.[TypeID] = t.[ID]
	where t.[AccountBEDefinitionID] = @DefinitionId
END