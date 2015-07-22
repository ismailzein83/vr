CREATE PROCEDURE [sec].[sp_OrgChart_GetById]
	@Id INT
AS
BEGIN
	SELECT [Id], [Name], [Hierarchy] FROM [sec].[OrgChart]
	WHERE Id = @Id
END