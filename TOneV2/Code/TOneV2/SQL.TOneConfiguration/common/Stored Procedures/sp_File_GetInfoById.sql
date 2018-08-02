
CREATE PROCEDURE [common].[sp_File_GetInfoById]
	@Id bigint
AS
BEGIN
	SELECT [Id], [Name], [Extension], [IsUsed], [ModuleName], [UserID], IsTemp, ConfigID, Settings, [CreatedTime],FileUniqueId
	FROM [common].[File] WITH(NOLOCK) 
	WHERE Id = @Id
END