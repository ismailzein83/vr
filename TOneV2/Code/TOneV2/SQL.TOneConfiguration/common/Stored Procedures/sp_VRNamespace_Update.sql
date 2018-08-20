CREATE PROCEDURE [common].[sp_VRNamespace_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.VRNamespace WHERE ID != @ID and Name = @Name)
	BEGIN
		update common.VRNamespace
		set  Name = @Name, Settings = @Settings
		where  ID = @ID
	END
END