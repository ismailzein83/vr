-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_GenericRuleDefinition_GetAll]
AS
BEGIN
	SELECT	ID, Name, Details, CreatedTime
	FROM	[genericdata].GenericRuleDefinition WITH(NOLOCK) 
	ORDER BY [Name]
END