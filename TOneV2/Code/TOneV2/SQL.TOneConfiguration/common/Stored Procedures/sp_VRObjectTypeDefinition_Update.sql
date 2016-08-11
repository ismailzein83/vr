create PROCEDURE [common].[sp_VRObjectTypeDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.VRObjectTypeDefinition WHERE ID != @ID and Name = @Name)
	BEGIN
		update common.VRObjectTypeDefinition
		set  Name = @Name, Settings = @Settings
		where  ID = @ID
	END
END