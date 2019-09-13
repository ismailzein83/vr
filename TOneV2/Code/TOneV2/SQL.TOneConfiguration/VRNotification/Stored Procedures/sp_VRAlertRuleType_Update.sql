--Update
CREATE Procedure [VRNotification].[sp_VRAlertRuleType_Update]
	@ID uniqueidentifier,
	@Name NVARCHAR(255),
	@DevProjectId uniqueidentifier,
	@Settings NVARCHAR(MAX)
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM VRNotification.VRAlertRuleType WHERE ID != @ID and Name = @Name)
	BEGIN
		update VRNotification.VRAlertRuleType
		set  Name = @Name, DevProjectId=@DevProjectId, Settings = @Settings, LastModifiedTime = GETDATE()
		where  ID = @ID
	END
END