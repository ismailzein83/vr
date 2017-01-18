create PROCEDURE [Common].[sp_StatusDefinition_GetAll]
AS
BEGIN
	SELECT ID, Name, Settings, BusinessEntityDefinitionID
	FROM [Common].StatusDefinition  with(nolock)
END