
CREATE Procedure [VRNotification].[sp_VRAlertRule_SetEnable]
	@ID int
AS
BEGIN
	update [VRNotification].VRAlertRule
	set IsDisabled = 0
	where ID = @ID
END