CREATE Procedure [VRNotification].[sp_VRNotification_UpdateStatus]
	@VRNotificationId bigint,
	@Status tinyint,
	@ExecuteBPInstanceID bigint,
	@ClearBPInstanceID bigint
AS
BEGIN
	
UPDATE [VRNotification].[VRNotification]
set [Status] = @Status, ExecuteBPInstanceID = ISNULL(@ExecuteBPInstanceID, ExecuteBPInstanceID), ClearBPInstanceID = ISNULL(@ClearBPInstanceID, ClearBPInstanceID)
where 	ID = @VRNotificationId

END