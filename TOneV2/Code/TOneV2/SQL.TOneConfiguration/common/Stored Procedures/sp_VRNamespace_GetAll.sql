CREATE PROCEDURE [common].[sp_VRNamespace_GetAll]
AS
BEGIN
	SELECT	ID, Name, Settings
	FROM	[common].VRNamespace WITH(NOLOCK) 
	ORDER BY [Name]
END