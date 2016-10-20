CREATE PROCEDURE [reprocess].[sp_ReprocessDefinition_Update]
	@ID uniqueidentifier,
	@Name Nvarchar(255),
	@Settings nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM [reprocess].ReprocessDefinition WHERE ID != @ID AND Name = @Name)
	BEGIN
		UPDATE [reprocess].ReprocessDefinition
		SET Name = @Name,
			[Settings] = @Settings
		WHERE ID = @ID
	END
END