CREATE PROCEDURE  [bp].[sp_BPBusinessRuleActionType_GetAll]
AS
BEGIN
   SELECT [ID]
      ,[Description]
      ,[Settings]
  FROM [bp].[BPBusinessRuleActionType] WITH(NOLOCK)
END