CREATE PROCEDURE [common].[sp_StyleDefinition_GetAll]
AS
BEGIN
	SELECT	ID, Name, Settings
	FROM	[common].StyleDefinition WITH(NOLOCK) 
END