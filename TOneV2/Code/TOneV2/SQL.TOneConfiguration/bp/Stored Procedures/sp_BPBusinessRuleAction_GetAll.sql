CREATE PROCEDURE  [bp].[sp_BPBusinessRuleAction_GetAll]
AS
BEGIN
   SELECT [ID]
      ,[BusinessRuleDefinitionId]
      ,[Settings]
  FROM [bp].[BPBusinessRuleAction] WITH(NOLOCK)
END