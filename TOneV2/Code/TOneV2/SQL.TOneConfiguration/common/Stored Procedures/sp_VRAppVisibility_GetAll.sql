CREATE PROCEDURE [common].[sp_VRAppVisibility_GetAll]
AS
BEGIN
	SELECT	ID, Name, IsCurrent, Settings
	FROM	[common].VRAppVisibility WITH(NOLOCK) 
	ORDER BY [Name]
END