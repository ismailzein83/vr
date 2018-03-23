
CREATE Procedure [VRNotification].[sp_VRAlertRule_SetDisable]
	@ID int
AS
BEGIN
	update [VRNotification].VRAlertRule
	set IsDisabled = 1
	where ID = @ID
END