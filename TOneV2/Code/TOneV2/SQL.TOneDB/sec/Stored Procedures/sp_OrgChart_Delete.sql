CREATE PROCEDURE [sec].[sp_OrgChart_Delete] 
	@Id INT
AS
BEGIN
	DELETE FROM sec.[OrgChart]
    WHERE Id = @Id 
END