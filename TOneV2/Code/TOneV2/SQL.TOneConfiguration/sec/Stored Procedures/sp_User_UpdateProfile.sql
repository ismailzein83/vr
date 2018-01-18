CREATE PROCEDURE [sec].[sp_User_UpdateProfile] 
	@ID int,
	@Name Nvarchar(255),
	@Settings nvarchar(max)
AS
BEGIN
	UPDATE	[sec].[User]
	SET		[Name] = @Name , [Settings] = @Settings
	WHERE	ID = @ID
END