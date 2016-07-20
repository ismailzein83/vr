CREATE PROCEDURE [Retail].[sp_StatusDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
    @Settings nvarchar(MAX),
	@EntityType int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Retail_BE.StatusDefinition WHERE Name = @Name and EntityType = @EntityType)
	BEGIN
	INSERT INTO Retail_BE.StatusDefinition (ID,Name,Settings,EntityType)
	VALUES (@ID, @Name,@Settings,@EntityType)
	END
END