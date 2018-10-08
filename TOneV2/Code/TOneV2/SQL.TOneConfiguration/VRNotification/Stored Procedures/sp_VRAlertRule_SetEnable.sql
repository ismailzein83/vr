
CREATE Procedure [VRNotification].[sp_VRAlertRule_SetEnable]
	@ID int,
	@LastModifiedBy int
AS
BEGIN
	update [VRNotification].VRAlertRule
	set IsDisabled = 0 , LastModifiedBy = @LastModifiedBy
	where ID = @ID
END