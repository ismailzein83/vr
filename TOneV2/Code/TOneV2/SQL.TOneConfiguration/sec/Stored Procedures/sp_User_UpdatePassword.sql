CREATE PROCEDURE [sec].[sp_User_UpdatePassword] 
    @ID int,
	@NewPassword Nvarchar(255),
	@LastModifiedBy int
AS
BEGIN

	UPDATE sec.[User]
	SET Password = @NewPassword, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE(), PasswordChangeTime = GETDATE()
	WHERE ID = @ID

END