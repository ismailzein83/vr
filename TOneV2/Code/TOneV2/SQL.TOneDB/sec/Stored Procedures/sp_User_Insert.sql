CREATE PROCEDURE [sec].[sp_User_Insert] 
	
	@Name Nvarchar(255),
	@Password Nvarchar(255),
	@Email Nvarchar(255),
	@Status int,
	@Description ntext,
	@Id int out
AS
BEGIN
	Insert into sec.[User] ([Name],[Password],[Email], [Status], [Description])
	values(@Name, @Password, @Email, @Status, @Description)
	
	SET @Id = @@IDENTITY
END