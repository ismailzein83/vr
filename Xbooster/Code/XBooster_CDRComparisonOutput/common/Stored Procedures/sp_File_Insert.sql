
CREATE PROCEDURE [common].[sp_File_Insert] 
	@Name Nvarchar(255),
	@Extension Nvarchar(50),
	@Content varbinary(max),
	@ModuleName VARCHAR(255),
	@UserID INT,
	@IsTemp BIT,
	@ConfigId uniqueidentifier,
	@Settings nvarchar(max),
	@CreatedTime datetime,
	@FileUniqueId uniqueidentifier,
	@Id bigint out
AS
BEGIN
	IF @CreatedTime IS NULL
		SET @CreatedTime = GETDATE()

	Insert into common.[File] ([Name], [Extension], [Content], [ModuleName], [UserID], [IsTemp], [ConfigID], [Settings],[CreatedTime], [FileUniqueId])
	values(@Name, @Extension, @Content, @ModuleName, @UserID, @IsTemp, @ConfigId, @Settings, @CreatedTime, @FileUniqueId)
	SET @Id = SCOPE_IDENTITY()
END