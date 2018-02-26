CREATE Procedure [VRNotification].[sp_VRNotification_GetByNotificationType]
	@NotificationTypeID uniqueidentifier,
	@ParentType1 varchar(255) = null,
	@ParentType2 varchar(255) = null,
	@EventKey nvarchar(900) = null,
	@Statuses varchar(max)
AS
BEGIN
	DECLARE @StatusTable TABLE (StatusId int)
	INSERT INTO @StatusTable (StatusId)
	SELECT Convert(int, ParsedString) FROM [VRNotification].[ParseStringList](@Statuses)

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
	      and EventKey = @EventKey
		  and (@ParentType1 is null or ParentType1 = @ParentType1)
		  and (@ParentType2 is null or ParentType2 = @ParentType2)
		  and (@Statuses is null or [Status] in (select StatusId from @StatusTable))
END