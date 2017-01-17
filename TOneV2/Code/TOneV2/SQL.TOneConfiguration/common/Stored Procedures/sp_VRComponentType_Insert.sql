CREATE PROCEDURE [common].[sp_VRComponentType_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@ConfigId uniqueidentifier,
	@Settings NVARCHAR(Max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.VRComponentType WHERE Name = @Name)
	BEGIN
		INSERT INTO common.VRComponentType (ID,Name,ConfigId,Settings)
		VALUES (@ID, @Name,@ConfigId,@Settings)
	END
END