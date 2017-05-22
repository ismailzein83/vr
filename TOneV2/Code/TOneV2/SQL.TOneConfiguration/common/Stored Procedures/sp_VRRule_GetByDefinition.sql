

Create Procedure [common].[sp_VRRule_GetByDefinition]
	@RuleDefinitionId uniqueidentifier
AS
BEGIN
	SELECT	[ID],[RuleDefinitionId],[Settings],[CreatedTime]
	FROM [common].[VRRule] with(nolock)
	where [RuleDefinitionID] = @RuleDefinitionId
END