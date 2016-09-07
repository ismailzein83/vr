--Update
Create Procedure [VRNotification].[sp_VRAlertRuleType_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@Settings NVARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM VRNotification.VRAlertRuleType WHERE ID != @ID and Name = @Name)
	BEGIN
		update VRNotification.VRAlertRuleType
		set  Name = @Name, Settings = @Settings
		where  ID = @ID
	END
END