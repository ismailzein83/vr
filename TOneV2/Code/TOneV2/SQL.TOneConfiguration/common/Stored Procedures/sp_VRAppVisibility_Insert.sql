Create PROCEDURE [common].[sp_VRAppVisibility_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(Max)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.VRAppVisibility WHERE Name = @Name)
	BEGIN
		INSERT INTO common.VRAppVisibility (ID,Name,Settings)
		VALUES (@ID, @Name,@Settings)
	END
END