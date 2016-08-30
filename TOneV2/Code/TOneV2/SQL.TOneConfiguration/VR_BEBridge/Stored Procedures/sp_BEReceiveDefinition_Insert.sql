
CREATE PROCEDURE [VR_BEBridge].[sp_BEReceiveDefinition_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)

AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM VR_BEBridge.BEReceiveDefinition WHERE Name = @Name )
	BEGIN
		INSERT INTO VR_BEBridge.BEReceiveDefinition (ID,Name, Settings)
		VALUES (@ID,@Name, @Settings)
	END
END