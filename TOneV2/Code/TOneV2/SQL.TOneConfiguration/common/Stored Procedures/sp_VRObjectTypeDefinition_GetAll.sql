CREATE PROCEDURE [common].[sp_VRObjectTypeDefinition_GetAll]
AS
BEGIN
	SELECT	ID,DevProjectID, Name, Settings
	FROM	[common].VRObjectTypeDefinition WITH(NOLOCK) 
	ORDER BY [Name]
END