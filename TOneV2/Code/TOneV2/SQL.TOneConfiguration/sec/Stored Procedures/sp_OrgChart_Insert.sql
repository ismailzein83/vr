CREATE PROCEDURE [sec].[sp_OrgChart_Insert]
	@Name VARCHAR(100),
	@Hierarchy VARCHAR(1000),
	@Id INT OUTPUT
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM sec.[OrgChart] WHERE Name = @Name)
	BEGIN
		INSERT INTO sec.[OrgChart] ([Name],[Hierarchy])
		VALUES (@Name, @Hierarchy)
		
		SET @Id = SCOPE_IDENTITY()
	END
END