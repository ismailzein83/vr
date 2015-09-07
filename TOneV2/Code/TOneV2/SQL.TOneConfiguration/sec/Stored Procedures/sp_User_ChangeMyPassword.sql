CREATE PROCEDURE [sec].[sp_User_ChangeMyPassword] 
    @ID int,
	@NewPassword Nvarchar(255)
AS
BEGIN

	UPDATE sec.[User]
	SET Password = @NewPassword
	WHERE ID = @ID

END