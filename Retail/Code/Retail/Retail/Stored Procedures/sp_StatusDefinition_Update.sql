CREATE PROCEDURE [Retail].[sp_StatusDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings nvarchar(MAX),
	@EntityType int
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM Retail_BE.StatusDefinition WHERE ID != @ID and Name = @Name and EntityType=@EntityType)
	BEGIN
		update Retail_BE.StatusDefinition 
		set  Name = @Name ,Settings= @Settings,EntityType = @EntityType
		where  ID = @ID
	END
END