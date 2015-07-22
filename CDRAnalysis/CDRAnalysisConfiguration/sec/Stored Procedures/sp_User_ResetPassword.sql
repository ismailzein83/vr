CREATE PROCEDURE [sec].[sp_User_ResetPassword] 
	@ID int,
	@Password Nvarchar(255)
AS
BEGIN
	UPDATE sec.[User]
	SET Password = @Password
	WHERE ID = @ID
END