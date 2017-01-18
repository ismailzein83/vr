create PROCEDURE [Common].[sp_StatusDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@BusinessEntityDefinitionId uniqueidentifier,
    @Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Common].StatusDefinition WHERE Name = @Name and BusinessEntityDefinitionID = @BusinessEntityDefinitionId)
	BEGIN
	INSERT INTO [Common].StatusDefinition (ID,Name,BusinessEntityDefinitionID,Settings)
	VALUES (@ID, @Name,@BusinessEntityDefinitionId,@Settings)
	END
END