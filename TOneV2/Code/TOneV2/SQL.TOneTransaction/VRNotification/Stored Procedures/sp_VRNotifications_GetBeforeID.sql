

CREATE PROCEDURE [VRNotification].[sp_VRNotifications_GetBeforeID]
	@NotificationTypeID UniqueIdentifier,
	@NbOfRows INT,
	@LessThanID BIGINT
	
AS
BEGIN	            
	
	SELECT TOP(@NbOfRows) 
		[ID]
      ,[UserID]
      ,[TypeID]
      ,[ParentType1]
      ,[ParentType2]
      ,[EventKey]
      ,[BPProcessInstanceID]
      ,[Status]
      ,[AlertLevelID]
      ,[Description]
      ,[ErrorMessage]
      ,[Data]
	FROM [VRNotification].[VRNotification] WITH(NOLOCK) 
	WHERE ID < @LessThanID 
	AND  TypeID = @NotificationTypeID
	ORDER BY ID DESC
END