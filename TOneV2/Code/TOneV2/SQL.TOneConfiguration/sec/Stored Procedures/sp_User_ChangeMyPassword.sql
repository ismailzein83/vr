CREATE PROCEDURE [sec].[sp_User_ChangeMyPassword] 
    @ID int,
	@OldPassword Nvarchar(255),
	@NewPassword Nvarchar(255)
AS
BEGIN

 IF EXISTS ( SELECT 1 FROM [sec].[User] WHERE [ID] = @ID and [Password] = @OldPassword )
	BEGIN
		UPDATE sec.[User]
		SET Password = @NewPassword
		WHERE ID = @ID
	END
END