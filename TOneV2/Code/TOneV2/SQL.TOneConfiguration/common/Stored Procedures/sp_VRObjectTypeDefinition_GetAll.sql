CREATE PROCEDURE [common].[sp_VRObjectTypeDefinition_GetAll]
AS
BEGIN
	SELECT	ID, Name, Settings
	FROM	[common].VRObjectTypeDefinition WITH(NOLOCK) 
END