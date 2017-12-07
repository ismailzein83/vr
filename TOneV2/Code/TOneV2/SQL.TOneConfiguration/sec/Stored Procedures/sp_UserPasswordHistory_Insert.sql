
CREATE PROCEDURE [sec].sp_UserPasswordHistory_Insert 
	
	@UserID int,
	@Password Nvarchar(255),
	@IsChangedByAdmin bit,
	@Id int out
AS
BEGIN

		INSERT INTO [sec].[UserPasswordHistory]([UserID],[Password],[IsChangedByAdmin])
		VALUES (@UserID,@Password,@IsChangedByAdmin)

		SET @Id = SCOPE_IDENTITY()
END