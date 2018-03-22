CREATE PROCEDURE [common].[sp_StatusDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@BusinessEntityDefinitionId uniqueidentifier,
    @Settings nvarchar(MAX),
	@CreatedBy int,
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Common].StatusDefinition WHERE Name = @Name and BusinessEntityDefinitionID = @BusinessEntityDefinitionId)
	BEGIN
	INSERT INTO [Common].StatusDefinition (ID,Name,BusinessEntityDefinitionID,Settings, CreatedBy, LastModifiedBy, LastModifiedTime)
	VALUES (@ID, @Name, @BusinessEntityDefinitionId,@Settings, @CreatedBy, @LastModifiedBy, GETDATE())
	END
END