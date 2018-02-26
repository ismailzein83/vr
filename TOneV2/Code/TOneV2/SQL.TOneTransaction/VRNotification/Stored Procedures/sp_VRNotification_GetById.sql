
CREATE Procedure [VRNotification].[sp_VRNotification_GetById]
	@NotificationID bigint
AS
BEGIN
	SELECT	 [ID]
			,[UserID]
			,[TypeID]
			,[ParentType1]
			,[ParentType2]
			,[EventKey]
			,[ExecuteBPInstanceID]
			,[ClearBPInstanceID]
			,[Status]
			,[Description]
			,[AlertLevelID]
			,[ErrorMessage]
			,[Data]
			,[CreationTime]
			,[EventPayload]
			,RollbackEventPayload
	FROM [VRNotification].[VRNotification] WITH(NOLOCK)
	where ID = @NotificationID
END