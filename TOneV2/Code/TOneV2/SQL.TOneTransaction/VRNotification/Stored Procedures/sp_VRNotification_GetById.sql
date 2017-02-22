


CREATE Procedure [VRNotification].[sp_VRNotification_GetById]
	@NotificationID bigint
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
	where (vr.ID = @NotificationID)
END