CREATE PROCEDURE [sec].[sp_User_Update] 
	@ID int,
	@Name Nvarchar(255),
	@Email Nvarchar(255),
	@Description Nvarchar(1000),
	@TenantId int,
	@EnabledTill datetime,
	@Settings nvarchar(max),
	@LastModifiedBy int
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM sec.[User] WHERE ID != @ID AND Email = @Email)
	begin
		UPDATE sec.[User]
		SET Name = @Name,
			Email = @Email,
			[Description] = @Description,
			TenantId = @TenantId,
			EnabledTill = @EnabledTill,
			Settings = @Settings,
			LastModifiedBy = @LastModifiedBy,
			LastModifiedTime = GETDATE()
		WHERE ID = @ID
	end
END