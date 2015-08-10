
CREATE PROCEDURE [common].[sp_File_Insert] 
	
	@Name Nvarchar(255),
	@Extension Nvarchar(50),
	@Content varbinary(max),
	@CreatedTime datetime,
	@Id bigint out
AS
BEGIN
	Insert into common.[File] ([Name],[Extension],[Content], [CreatedTime])
	values(@Name, @Extension, @Content ,@CreatedTime)	
	SET @Id = @@IDENTITY
END