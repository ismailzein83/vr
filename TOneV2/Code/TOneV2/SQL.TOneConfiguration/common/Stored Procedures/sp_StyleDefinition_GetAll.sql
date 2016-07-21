Create PROCEDURE [common].[sp_StyleDefinition_GetAll]
AS
BEGIN
	SELECT	ID, Name, Settings
	FROM	[common].StyleDefinition
END