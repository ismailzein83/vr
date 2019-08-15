CREATE PROCEDURE [common].[sp_VRNamespace_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId uniqueidentifier
	--@Settings NVARCHAR(Max)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.VRNamespace WHERE Name = @Name)
	BEGIN
		INSERT INTO common.VRNamespace (ID,Name,DevProjectId)
		VALUES (@ID, @Name,@DevProjectId)
	END
END