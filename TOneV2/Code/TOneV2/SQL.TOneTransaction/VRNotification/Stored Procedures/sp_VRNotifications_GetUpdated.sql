CREATE PROCEDURE [VRNotification].[sp_VRNotifications_GetUpdated]
	@NotificationTypeID UniqueIdentifier,
	@NbOfRows BIGINT,
    @TimestampAfter timestamp,
    @Description varchar(max),
    @AlertLevelIds varchar(max),
	@From datetime,
	@To datetime
AS
BEGIN
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
	  ,[timestamp]
	  ,[EventPayload]
	INTO #temptable2_VRNotifications_GetUpdated
	FROM [VRNotification].[VRNotification] WITH(NOLOCK) 
	WHERE  TypeID = @NotificationTypeID 
		   AND (@Description is null or [Description] like '%' + @Description + '%')
		   AND (@AlertLevelIds is null or [AlertLevelId] in (SELECT AlertLevelId FROM @NotificationAlertLevelIDsTable)) 
		   AND (@From is null or CreationTime >= @From)
		   AND (@To is null or CreationTime < @To)
		   AND ([timestamp] > @TimestampAfter) 
	ORDER BY [timestamp]
	
	SELECT [ID],[UserID],[TypeID],[ParentType1],[ParentType2],[EventKey],[ExecuteBPInstanceID]
			,[ClearBPInstanceID],[Status],[AlertLevelID],[Description],[ErrorMessage],[Data],[CreationTime] ,[EventPayload]
	FROM #temptable2_VRNotifications_GetUpdated
	  
	IF((SELECT COUNT(*) FROM #temptable2_VRNotifications_GetUpdated) = 0)
		SELECT @TimestampAfter AS MaxTimestamp
	ELSE
		SELECT MAX([timestamp]) AS MaxTimestamp FROM #temptable2_VRNotifications_GetUpdated
END