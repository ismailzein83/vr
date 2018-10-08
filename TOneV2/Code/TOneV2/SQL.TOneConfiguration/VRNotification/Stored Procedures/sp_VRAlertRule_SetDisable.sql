
CREATE Procedure [VRNotification].[sp_VRAlertRule_SetDisable]
	@ID int,
	@LastModifiedBy int
AS
BEGIN
	update [VRNotification].VRAlertRule
	set IsDisabled = 1 , LastModifiedBy = @LastModifiedBy
	where ID = @ID
END