CREATE PROCEDURE [sec].[sp_OrgChart_Update] 
	@Id INT,
	@Name VARCHAR(100),
	@Hierarchy VARCHAR(1000)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM sec.[OrgChart] WHERE Id != @Id AND Name = @Name)
	BEGIN
		UPDATE [sec].[OrgChart]
		SET Name = @Name,
			Hierarchy = @Hierarchy
		WHERE Id = @Id
	END
END