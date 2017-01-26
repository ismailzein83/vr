Create PROCEDURE [common].[sp_VRAppVisibility_GetAll]
AS
BEGIN
	SELECT	ID, Name, Settings
	FROM	[common].VRAppVisibility WITH(NOLOCK) 
	ORDER BY [Name]
END