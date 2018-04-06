CREATE PROCEDURE [sec].[sp_User_ActivatePassword] 
    @Email Nvarchar(255),
	@NewPassword Nvarchar(255),
	@LastModifiedBy int
AS
BEGIN

	UPDATE sec.[User]
	SET Password = @NewPassword, TempPassword = null, TempPasswordValidTill = null, LastModifiedBy = @LastModifiedBy, LastModifiedTime = GETDATE()
	WHERE Email = @Email

END