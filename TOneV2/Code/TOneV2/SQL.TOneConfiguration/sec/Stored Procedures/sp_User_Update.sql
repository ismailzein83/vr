CREATE PROCEDURE [sec].[sp_User_Update] 
	@ID int,
	@Name Nvarchar(255),
	@Email Nvarchar(255),
	@Status int,
	@Description ntext
AS
BEGIN
	UPDATE sec.[User]
	SET Name = @Name,
		Email = @Email,
		[Status] = @Status,
		Description = @Description
	WHERE ID = @ID
END