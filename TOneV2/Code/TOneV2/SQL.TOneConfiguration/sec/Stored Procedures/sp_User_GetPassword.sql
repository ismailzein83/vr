Create PROCEDURE [sec].[sp_User_GetPassword] 
	@ID int
AS
BEGIN
	Select [Password] FROM sec.[User]
	WHERE ID = @ID
END