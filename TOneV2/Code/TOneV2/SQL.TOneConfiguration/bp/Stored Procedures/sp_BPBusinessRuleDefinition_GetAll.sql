CREATE PROCEDURE  [bp].[sp_BPBusinessRuleDefinition_GetAll]
AS
BEGIN
	select [ID], [Name], [BPDefintionId], [Settings], [Rank]
	from [bp].[BPBusinessRuleDefinition] with(nolock)
	order by [Rank]
END