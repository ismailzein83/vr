CREATE PROCEDURE [sec].[sp_User_EditUserProfile] 
	@ID int,
	@Name Nvarchar(255)
AS
BEGIN
	UPDATE sec.[User]
	SET Name = @Name
	WHERE ID = @ID
END