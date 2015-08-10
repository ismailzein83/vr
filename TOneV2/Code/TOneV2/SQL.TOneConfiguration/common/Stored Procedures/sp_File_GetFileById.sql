
CREATE PROCEDURE [common].[sp_File_GetFileById]
	@Id INT
AS
BEGIN
	SELECT [Id], [Name],[CreatedTime],[Extension],[IsUsed],[Content] FROM [common].[File]
	WHERE Id = @Id
END