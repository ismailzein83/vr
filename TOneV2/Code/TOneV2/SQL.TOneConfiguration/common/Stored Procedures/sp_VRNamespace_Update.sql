CREATE PROCEDURE [common].[sp_VRNamespace_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId uniqueidentifier
	--@Settings NVARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.VRNamespace WHERE ID != @ID and Name = @Name)
	BEGIN
		update common.VRNamespace
		set  Name = @Name,DevProjectId=@DevProjectId,LastModifiedTime = getdate()
		where  ID = @ID
	END
END