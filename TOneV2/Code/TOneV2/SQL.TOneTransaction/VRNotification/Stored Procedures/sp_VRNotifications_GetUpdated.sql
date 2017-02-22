
CREATE PROCEDURE [VRNotification].[sp_VRNotifications_GetUpdated]	
	@NotificationTypeID UniqueIdentifier,
	@NbOfRows INT,
	@GreaterThanID BIGINT
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
	where  TypeID = @NotificationTypeID
			AND ID >@GreaterThanID
			
        ORDER BY ID                    
END