-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BELookupRuleDefinition_GetAll]
AS
BEGIN
	SELECT	[ID], [Name], [Details]
	FROM	[genericdata].BELookupRuleDefinition WITH(NOLOCK) 
END