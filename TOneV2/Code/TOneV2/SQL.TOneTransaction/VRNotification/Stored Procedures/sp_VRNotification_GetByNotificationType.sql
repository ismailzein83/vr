


CREATE Procedure [VRNotification].[sp_VRNotification_GetByNotificationType]
	@NotificationTypeID uniqueidentifier,
	@ParentType1 varchar(255) = null,
	@ParentType2 varchar(255) = null,
	@EventKey nvarchar(900) = null
AS
BEGIN

	SELECT	   vr.[ID]
			  ,vr.[UserID]
			  ,vr.[TypeID]
			  ,vr.[ParentType1]
			  ,vr.[ParentType2]
			  ,vr.[EventKey]
			  ,vr.[BPProcessInstanceID]
			  ,vr.[Status]
			  ,vr.[Description]
			  ,vr.[AlertLevelID]
			  ,vr.[ErrorMessage]
			  ,vr.[Data]
	FROM	   [VRNotification].[VRNotification] vr WITH(NOLOCK)
	where (vr.[TypeID] = @NotificationTypeID)
			and (@ParentType1 is null or vr.ParentType1 = @ParentType1)
			and (@ParentType2 is null or vr.ParentType2 = @ParentType2)
			and vr.EventKey = @EventKey
END