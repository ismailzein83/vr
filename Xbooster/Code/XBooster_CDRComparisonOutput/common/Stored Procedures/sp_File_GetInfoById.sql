CREATE PROCEDURE [common].[sp_File_GetInfoById]
	@Id INT
AS
BEGIN
	SELECT [Id], [Name], [Extension], [IsUsed], [ModuleName], [UserID], [CreatedTime]
	FROM [common].[File] WITH(NOLOCK) 
	WHERE Id = @Id
END