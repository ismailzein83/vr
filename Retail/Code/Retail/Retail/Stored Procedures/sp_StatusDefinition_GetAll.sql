CREATE PROCEDURE [Retail].[sp_StatusDefinition_GetAll]
AS
BEGIN
	SELECT ID, Name, Settings, EntityType
	FROM Retail_BE.StatusDefinition  with(nolock)
END