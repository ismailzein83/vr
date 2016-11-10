CREATE PROCEDURE [sec].[sp_OrgChart_GetAll] 
AS
BEGIN
	SET NOCOUNT ON;
	SELECT	[Id], [Name], [Hierarchy] 
	FROM	[sec].[OrgChart] WITH(NOLOCK) 
	ORDER BY [Name]
END