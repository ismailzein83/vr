CREATE PROCEDURE [common].[sp_VRObjectTypeDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId uniqueidentifier,
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.VRObjectTypeDefinition WHERE ID != @ID and Name = @Name)
	BEGIN
		update common.VRObjectTypeDefinition
		set  Name = @Name,DevProjectId=@DevProjectId, Settings = @Settings
		where  ID = @ID
	END
END