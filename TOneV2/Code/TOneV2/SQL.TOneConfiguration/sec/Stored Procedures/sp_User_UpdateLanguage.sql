CREATE PROCEDURE [sec].[sp_User_UpdateLanguage]
	@Settings nvarchar(max),
	@UserID int
AS
BEGIN
		UPDATE sec.[User]
		SET Settings = @Settings
		WHERE ID = @UserID
END