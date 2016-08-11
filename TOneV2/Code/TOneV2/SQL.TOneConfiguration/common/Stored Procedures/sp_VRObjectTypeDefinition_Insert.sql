create PROCEDURE [common].[sp_VRObjectTypeDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(Max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.VRObjectTypeDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO common.VRObjectTypeDefinition (ID,Name,Settings)
		VALUES (@ID, @Name,@Settings)
	END
END