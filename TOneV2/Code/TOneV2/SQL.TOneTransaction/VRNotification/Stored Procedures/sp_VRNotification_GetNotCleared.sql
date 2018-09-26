CREATE Procedure [VRNotification].[sp_VRNotification_GetNotCleared]
	@NotificationTypeID uniqueidentifier,
	@ParentType1 varchar(255) = null,
	@ParentType2 varchar(255) = null,
	@EventKeysTable [VRNotification].[EventKeysTable] READONLY,
	@NotificationCreatedAfter datetime = null
AS
BEGIN
	SELECT	 [ID]
			,[UserID]
			,[TypeID]
			,[ParentType1]
			,[ParentType2]
			,vrNotificationTable.[EventKey]
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
	FROM [VRNotification].[VRNotification] vrNotificationTable WITH(NOLOCK)
	JOIN @EventKeysTable eventKeyTable on vrNotificationTable.EventKey = eventKeyTable.EventKey
	where TypeID = @NotificationTypeID
		  and (@ParentType1 is null or ParentType1 = @ParentType1)
		  and (@ParentType2 is null or ParentType2 = @ParentType2)
		  and (@NotificationCreatedAfter is null or CreationTime > @NotificationCreatedAfter)
END