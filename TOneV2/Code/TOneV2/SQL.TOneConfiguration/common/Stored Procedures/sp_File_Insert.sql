
CREATE PROCEDURE [common].[sp_File_Insert] 
	@Name Nvarchar(255),
	@Extension Nvarchar(50),
	@Content varbinary(max),
	@ModuleType VARCHAR(255),
	@UserID INT,
	@CreatedTime datetime,
	@Id bigint out
AS
BEGIN
	Insert into common.[File] ([Name], [Extension], [Content], [ModuleType], [UserID], [CreatedTime])
	values(@Name, @Extension, @Content, @ModuleType, @UserID, @CreatedTime)
	SET @Id = @@IDENTITY
END