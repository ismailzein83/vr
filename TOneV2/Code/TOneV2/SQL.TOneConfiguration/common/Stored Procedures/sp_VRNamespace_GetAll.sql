CREATE PROCEDURE [common].[sp_VRNamespace_GetAll]
AS
BEGIN
	SELECT	ID, Name,DevProjectID
	FROM	[common].VRNamespace WITH(NOLOCK) 
	ORDER BY [Name]
END