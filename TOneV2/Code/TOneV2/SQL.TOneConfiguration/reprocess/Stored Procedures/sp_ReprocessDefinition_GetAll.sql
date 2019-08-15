
CREATE PROCEDURE [reprocess].[sp_ReprocessDefinition_GetAll]
AS
BEGIN
	SELECT	ID, Name,DevProjectID, Settings
	FROM	[reprocess].ReprocessDefinition WITH(NOLOCK) 
	ORDER BY [Name]
END