CREATE PROCEDURE [common].[sp_VRObjectTypeDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId uniqueidentifier,
	@Settings NVARCHAR(Max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.VRObjectTypeDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO common.VRObjectTypeDefinition (ID,Name,DevProjectId,Settings)
		VALUES (@ID, @Name,@DevProjectId,@Settings)
	END
END