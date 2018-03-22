CREATE PROCEDURE [common].[sp_StatusDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@BusinessEntityDefinitionId uniqueidentifier,
	@Settings nvarchar(MAX),
	@LastModifiedBy int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM [Common].StatusDefinition WHERE ID != @ID and Name = @Name and BusinessEntityDefinitionID = @BusinessEntityDefinitionId)
	BEGIN
		update [Common].StatusDefinition 
		set  Name = @Name ,Settings= @Settings, BusinessEntityDefinitionID = @BusinessEntityDefinitionId, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
		where  ID = @ID
	END
END