create PROCEDURE [Common].[sp_StatusDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@BusinessEntityDefinitionId uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Common].StatusDefinition WHERE ID != @ID and Name = @Name and BusinessEntityDefinitionID = @BusinessEntityDefinitionId)
	BEGIN
		update [Common].StatusDefinition 
		set  Name = @Name ,Settings= @Settings, BusinessEntityDefinitionID = @BusinessEntityDefinitionId
		where  ID = @ID
	END
END