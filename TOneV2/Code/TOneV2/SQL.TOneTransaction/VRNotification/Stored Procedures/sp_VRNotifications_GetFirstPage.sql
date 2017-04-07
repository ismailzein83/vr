﻿CREATE PROCEDURE [VRNotification].[sp_VRNotifications_GetFirstPage]
	@NotificationTypeID UniqueIdentifier,
	@NbOfRows bigint,
    @Description varchar(max),
    @StatusIds varchar(max),
    @AlertLevelIds varchar(max)
AS
BEGIN
	DECLARE @NotificationStatusIDsTable TABLE (StatusId INT)
	INSERT INTO @NotificationStatusIDsTable (StatusId)
	SELECT Convert(INT, ParsedString) FROM [VRNotification].[ParseStringList](@StatusIds)
	
	DECLARE @NotificationAlertLevelIDsTable TABLE (AlertLevelId uniqueidentifier)
	INSERT INTO @NotificationAlertLevelIDsTable (AlertLevelId)
	SELECT Convert(uniqueidentifier, ParsedString) FROM [VRNotification].[ParseStringList](@AlertLevelIds)
	
	SELECT TOP(@NbOfRows) [ID]
	  ,[UserID]
	  ,[TypeID]
	  ,[ParentType1]
	  ,[ParentType2]
	  ,[EventKey]
	  ,[ExecuteBPInstanceID]
	  ,[ClearBPInstanceID]
	  ,[Status]
	  ,[AlertLevelID]
	  ,[Description]
	  ,[ErrorMessage]
	  ,[Data]	
	  ,[CreationTime]
	  ,[EventPayload]
	FROM [VRNotification].[VRNotification] WITH(NOLOCK) 
	WHERE  TypeID = @NotificationTypeID 
		   AND (@Description is null or [Description] like '%' + @Description + '%')
		   AND (@StatusIds is null or [Status] in (SELECT StatusId FROM @NotificationStatusIDsTable))
		   AND (@AlertLevelIds is null or [AlertLevelId] in (SELECT AlertLevelId FROM @NotificationAlertLevelIDsTable))
	ORDER BY ID DESC
				
	SELECT MAX([timestamp]) AS MaxTimestamp 
	FROM [VRNotification].[VRNotification] WITH(NOLOCK)
END