
create PROCEDURE [common].[sp_File_InsertWithIdentityOff] 
	@UseTemp bit,
	@Id bigint,
	@Name Nvarchar(255),
	@Extension Nvarchar(50),
	@Content varbinary(max),
	@IsUsed bit,
	@ModuleName VARCHAR(255),	
	@UserID INT,
	@CreatedTime datetime
	
AS
BEGIN

if(@UseTemp = 1)
begin
set identity_insert [common].[file_Temp] on;
	Insert into common.[File_Temp] (ID, [Name], [Extension], [Content], [ModuleName],[IsUsed], [UserID], [CreatedTime])
	values(@ID, @Name, @Extension, @Content, @ModuleName,@IsUsed, @UserID, @CreatedTime)


set identity_insert [common].[file_Temp] off;
end

else
begin
set identity_insert [common].[file] on;
	Insert into common.[File] (ID, [Name], [Extension], [Content], [ModuleName],[IsUsed], [UserID], [CreatedTime])
	values(@ID, @Name, @Extension, @Content, @ModuleName,@IsUsed, @UserID, @CreatedTime)


set identity_insert [common].[file] off;
end

END