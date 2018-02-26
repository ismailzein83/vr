CREATE Procedure [VRNotification].[sp_VRNotification_UpdateStatus]
	@VRNotificationId bigint,
	@Status tinyint,
	@RollbackEventPayload nvarchar(max),
	@ExecuteBPInstanceID bigint,
	@ClearBPInstanceID bigint
AS
BEGIN
	
UPDATE [VRNotification].[VRNotification]
set [Status] = @Status, 
    RollbackEventPayload = ISNULL(@RollbackEventPayload, RollbackEventPayload),
	ExecuteBPInstanceID = ISNULL(@ExecuteBPInstanceID, ExecuteBPInstanceID), 
	ClearBPInstanceID = ISNULL(@ClearBPInstanceID, ClearBPInstanceID)
where 	ID = @VRNotificationId

END