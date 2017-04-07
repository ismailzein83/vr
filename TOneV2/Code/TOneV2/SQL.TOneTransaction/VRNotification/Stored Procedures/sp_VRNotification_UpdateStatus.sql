CREATE Procedure [VRNotification].[sp_VRNotification_UpdateStatus]
	@VRNotificationId bigint,
	@Status tinyint,
	@ExecuteBPInstanceID bigint,
	@ClearBPInstanceID bigint
AS
BEGIN
	
UPDATE [VRNotification].[VRNotification]
set [Status] = @Status
where 	ID = @VRNotificationId
		AND ExecuteBPInstanceID = ISNULL(@ExecuteBPInstanceID, ExecuteBPInstanceID)
		AND ClearBPInstanceID = ISNULL(@ClearBPInstanceID, ClearBPInstanceID)

END