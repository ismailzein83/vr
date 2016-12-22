Create Procedure [VRNotification].[sp_VRNotification_UpdateStatus]
	@VRNotificationId uniqueIdentifier,
	@Status tinyint
AS
BEGIN
	
UPDATE [VRNotification].[VRNotification]
set [Status] = @Status
where 	ID = @VRNotificationId

END