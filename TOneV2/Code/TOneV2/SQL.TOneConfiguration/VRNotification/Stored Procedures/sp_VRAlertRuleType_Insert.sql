--Insert
CREATE Procedure [VRNotification].[sp_VRAlertRuleType_Insert]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId uniqueidentifier,
	@Settings NVARCHAR(Max)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM VRNotification.VRAlertRuleType WHERE Name = @Name)
	BEGIN
		INSERT INTO VRNotification.VRAlertRuleType (ID,Name,Settings,DevProjectId)
		VALUES (@ID, @Name,@Settings,@DevProjectId)
	END
END