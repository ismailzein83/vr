Create PROCEDURE [common].[sp_VRComponentType_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(Max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM common.VRComponentType WHERE Name = @Name)
	BEGIN
		INSERT INTO common.VRComponentType (ID,Name,Settings)
		VALUES (@ID, @Name,@Settings)
	END
END