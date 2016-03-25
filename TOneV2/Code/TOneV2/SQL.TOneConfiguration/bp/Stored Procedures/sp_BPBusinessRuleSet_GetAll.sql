
CREATE PROCEDURE  [bp].[sp_BPBusinessRuleSet_GetAll]
AS
BEGIN
   SELECT [ID]
	  ,Name
	  ,ParentID
	  ,Details
	  ,BPDefinitionId
  FROM [bp].[BPBusinessRuleSet] WITH(NOLOCK)
END