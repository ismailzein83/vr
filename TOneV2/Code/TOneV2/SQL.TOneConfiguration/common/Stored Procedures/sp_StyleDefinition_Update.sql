CREATE PROCEDURE [common].[sp_StyleDefinition_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM common.StyleDefinition WHERE ID != @ID and Name = @Name)
	BEGIN
		update common.StyleDefinition
		set  Name = @Name, Settings = @Settings, LastModifiedTime = getdate()
		where  ID = @ID
	END
END