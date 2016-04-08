
CREATE PROCEDURE [common].[sp_File_GetFileById]
	@Id INT
AS
BEGIN
	SELECT [Id], [Name], [Extension], [Content], [IsUsed], [ModuleType], [UserID], [CreatedTime]
	FROM [common].[File]
	WHERE Id = @Id
END