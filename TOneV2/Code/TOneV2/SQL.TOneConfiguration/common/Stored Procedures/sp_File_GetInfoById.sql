
CREATE PROCEDURE [common].[sp_File_GetInfoById]
	@Id INT
AS
BEGIN
	SELECT [Id], [Name], [CreatedTime],[Extension],[IsUsed] FROM [common].[File]
	WHERE Id = @Id
END