CREATE PROCEDURE  [bp].[sp_BPBusinessRuleDefinition_GetAll]
AS
BEGIN
   SELECT [ID]
	  ,[Name]
	  ,[Settings]
  FROM [bp].[BPBusinessRuleDefinition] WITH(NOLOCK)
END