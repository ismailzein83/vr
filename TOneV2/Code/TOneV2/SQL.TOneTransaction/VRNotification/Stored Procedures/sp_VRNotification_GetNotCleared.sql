CREATE Procedure [VRNotification].[sp_VRNotification_GetNotCleared]
	@NotificationTypeID uniqueidentifier,
	@ParentType1 varchar(255) = null,
	@ParentType2 varchar(255) = null,
	@EventKeys nvarchar(max) = null,
	@NotificationCreatedAfter datetime = null
AS
BEGIN
	DECLARE @EventKeysTable TABLE (EventKey nvarchar(900))
	INSERT INTO @EventKeysTable (EventKey)
	SELECT Convert(nvarchar(900), ParsedString) FROM [VRNotification].[ParseStringList](@EventKeys)

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
	where TypeID = @NotificationTypeID
	      and (@EventKeys is null or EventKey in (select EventKey from @EventKeysTable))
		  and (@ParentType1 is null or ParentType1 = @ParentType1)
		  and (@ParentType2 is null or ParentType2 = @ParentType2)
		  and (@NotificationCreatedAfter is null or CreationTime > @NotificationCreatedAfter)
END