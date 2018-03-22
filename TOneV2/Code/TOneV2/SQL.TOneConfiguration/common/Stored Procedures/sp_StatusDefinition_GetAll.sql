CREATE PROCEDURE [common].[sp_StatusDefinition_GetAll]
AS
BEGIN
	SELECT ID, Name, Settings, BusinessEntityDefinitionID, CreatedTime, CreatedBy, LastModifiedBy, LastModifiedTime
	FROM [Common].StatusDefinition  with(nolock)
END