CREATE PROCEDURE [common].[sp_StyleDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(Max)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.StyleDefinition WHERE Name = @Name)
	BEGIN
		INSERT INTO common.StyleDefinition (ID,Name,Settings)
		VALUES (@ID, @Name,@Settings)
	END
END