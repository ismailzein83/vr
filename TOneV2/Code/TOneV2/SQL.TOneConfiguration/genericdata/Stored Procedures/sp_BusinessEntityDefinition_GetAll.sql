-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [genericdata].[sp_BusinessEntityDefinition_GetAll]
AS
BEGIN
	SELECT	ID, Name, Title, Settings
	FROM	[genericdata].BusinessEntityDefinition WITH(NOLOCK) 
	ORDER BY [Name]
END