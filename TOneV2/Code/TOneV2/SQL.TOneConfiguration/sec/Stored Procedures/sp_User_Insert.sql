CREATE PROCEDURE [sec].[sp_User_Insert] 
	@SecurityProviderId uniqueidentifier,
	@Name Nvarchar(255),
	@TempPassword Nvarchar(255),
	@Email Nvarchar(255),
	@Description Nvarchar(1000),
	@TenantId int,
	@EnabledTill datetime,
	@ExtendedSettings nvarchar(max),	
	@Settings nvarchar(max),
	@CreatedBy int,
	@LastModifiedBy int,
	@Id int out
AS
BEGIN
IF NOT EXISTS(select null from sec.[User] where Email = @Email)
	BEGIN
		Insert into sec.[User] ([SecurityProviderId], [Name],[TempPassword],[Email], [Description], [TenantId], [EnabledTill], [ExtendedSettings],[Settings], [CreatedBy],[LastModifiedBy], [LastModifiedTime]) 
		values(@SecurityProviderId, @Name, @TempPassword, @Email, @Description, @TenantId, @EnabledTill, @ExtendedSettings,@Settings, @CreatedBy, @LastModifiedBy, GETDATE())
		
		SET @Id = SCOPE_IDENTITY()
	END
END