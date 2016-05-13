CREATE PROCEDURE [sec].[sp_User_Update] 
	@ID int,
	@Name Nvarchar(255),
	@Email Nvarchar(255),
	@Status int,
	@Description ntext,
	@TenantId int
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sec.[User] WHERE ID != @ID AND Email = @Email)
	begin
		UPDATE sec.[User]
		SET Name = @Name,
			Email = @Email,
			[Status] = @Status,
			[Description] = @Description,
			TenantId = @TenantId
		WHERE ID = @ID
	end
END