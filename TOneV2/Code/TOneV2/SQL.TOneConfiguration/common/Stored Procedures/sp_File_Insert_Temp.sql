
CREATE PROCEDURE [common].[sp_File_Insert_Temp] 
	@Name Nvarchar(255),
	@Extension Nvarchar(50),
	@Content varbinary(max),
	@ModuleName VARCHAR(255),
	@UserID INT,
	@CreatedTime datetime,
	@Id bigint out
AS
BEGIN
	Insert into common.[File_Temp] ([Name], [Extension], [Content], [ModuleName], [UserID], [CreatedTime])
	values(@Name, @Extension, @Content, @ModuleName, @UserID, @CreatedTime)
	SET @Id = SCOPE_IDENTITY()
END