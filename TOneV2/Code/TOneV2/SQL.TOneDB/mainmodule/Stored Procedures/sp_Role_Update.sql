CREATE PROCEDURE [mainmodule].[sp_Role_Update] 
	@ID int,
	@Name Nvarchar(255),
	@Description ntext
AS
BEGIN
	UPDATE [dbo].[User]
	SET Name = @Name,
		Description = @Description
	WHERE ID = @ID
END