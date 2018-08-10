
Create PROCEDURE [common].[sp_File_GetByUniqueId]
	@fileUniqueId uniqueidentifier
AS
BEGIN
	SELECT [Id], [Name], [Extension], [Content], [IsUsed], [ModuleName], [UserID], IsTemp, ConfigID, Settings, [CreatedTime], [FileUniqueId]
	FROM [common].[File] WITH(NOLOCK) 
	WHERE FileUniqueId = @fileUniqueId
END