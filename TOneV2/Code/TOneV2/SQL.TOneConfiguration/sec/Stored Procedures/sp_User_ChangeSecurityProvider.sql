CREATE PROCEDURE [sec].[sp_User_ChangeSecurityProvider] 
	@ID int,
	@SecurityProviderId uniqueidentifier,
	@Email nvarchar(255),
	@Password nvarchar(255),
	@Settings nvarchar(max),
	@LastModifiedBy int
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sec.[User] WHERE ID != @ID AND Email = @Email)
	begin
		UPDATE sec.[User]
		SET SecurityProviderId=@SecurityProviderId,
			Email = @Email,
			Password = @Password,
			Settings = @Settings,
			LastModifiedBy = @LastModifiedBy,
			LastModifiedTime = GETDATE()
		WHERE ID = @ID
	end
END