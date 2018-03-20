CREATE PROCEDURE [sec].[sp_User_UpdateSetting]
	@Settings nvarchar(max),
	@UserID int,
	@LastModifiedBy int
AS
BEGIN
		UPDATE sec.[User]
		SET Settings = @Settings, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
		WHERE ID = @UserID
END