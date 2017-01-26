Create PROCEDURE [common].[sp_VRAppVisibility_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.VRAppVisibility WHERE ID != @ID and Name = @Name)
	BEGIN
		update common.VRAppVisibility
		set  Name = @Name, Settings = @Settings
		where  ID = @ID
	END
END