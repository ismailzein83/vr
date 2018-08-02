
Create PROCEDURE [common].[sp_File_GetInfoByUniqueId]
	@fileUniqueId uniqueidentifier
AS
BEGIN
	SELECT [Id], [Name], [Extension], [IsUsed], [ModuleName], [UserID], IsTemp, ConfigID, Settings, [CreatedTime], [FileUniqueId]
	FROM [common].[File] WITH(NOLOCK) 
	WHERE FileUniqueId = @fileUniqueId
END