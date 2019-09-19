
CREATE PROCEDURE [VR_BEBridge].[sp_BEReceiveDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId uniqueidentifier,
	@Settings NVARCHAR(MAX)

AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM VR_BEBridge.BEReceiveDefinition WHERE Name = @Name )
	BEGIN
		INSERT INTO VR_BEBridge.BEReceiveDefinition (ID,Name,DevProjectID, Settings)
		VALUES (@ID,@Name,@DevProjectId, @Settings)
	END
END