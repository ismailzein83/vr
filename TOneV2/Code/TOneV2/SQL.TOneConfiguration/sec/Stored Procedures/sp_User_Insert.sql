CREATE PROCEDURE [sec].[sp_User_Insert] 
	
	@Name Nvarchar(255),
	@TempPassword Nvarchar(255),
	@Email Nvarchar(255),
	@Status int,
	@Description ntext,
	@TenantId int,
	@Id int out
AS
BEGIN
IF NOT EXISTS(select null from sec.[User] where Email = @Email)
	BEGIN
		Insert into sec.[User] ([Name],[TempPassword],[Email], [Status], [Description], [TenantId]) 
		values(@Name, @TempPassword, @Email, @Status, @Description, @TenantId)
		
		SET @Id = SCOPE_IDENTITY()
	END
END