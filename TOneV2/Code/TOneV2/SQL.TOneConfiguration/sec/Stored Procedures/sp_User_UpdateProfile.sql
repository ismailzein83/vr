CREATE PROCEDURE [sec].[sp_User_UpdateProfile] 
	@ID int,
	@Name Nvarchar(255),
	@Settings nvarchar(max),
	@LastModifiedBy int
AS
BEGIN
	UPDATE	[sec].[User]
	SET		[Name] = @Name , [Settings] = @Settings, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE	ID = @ID
END