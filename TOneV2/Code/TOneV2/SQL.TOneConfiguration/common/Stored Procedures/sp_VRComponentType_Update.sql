CREATE PROCEDURE [common].[sp_VRComponentType_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@ConfigId uniqueidentifier,
	@Settings NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.VRComponentType WHERE ID != @ID and Name = @Name)
	BEGIN
		update common.VRComponentType
		set  Name = @Name, Settings = @Settings,configId = @ConfigId
		where  ID = @ID
	END
END