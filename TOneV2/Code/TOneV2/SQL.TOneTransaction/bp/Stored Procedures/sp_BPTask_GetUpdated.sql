CREATE PROCEDURE [bp].[sp_BPTask_GetUpdated]
	@TimestampAfter timestamp,
	@NbOfRows INT,
	@ProcessInstanceId BIGINT,
	@UserId int
AS
BEGIN

IF (@TimestampAfter IS NULL)
	BEGIN
	SELECT TOP(@NbOfRows)[ID]
			, [ProcessInstanceID]
			, [TypeID] 
			, [Title] 
			, [AssignedUsers]
			, [AssignedUsersDescription]
			, [ExecutedBy]
			, [Status] 
			, [TaskData]
			, [TaskExecutionInformation]
			, [Notes]
			, [Decision]
			, [CreatedTime] 
			, [LastUpdatedTime]
			, [timestamp]
            INTO #temp_table
            FROM [BP].[BPTask]  WITH(NOLOCK) 
            
            WHERE (@ProcessInstanceId is null or ProcessInstanceID = @ProcessInstanceId)
            AND (@UserId is null or ',' + AssignedUsers + ',' like '%,' +CONVERT(varchar(100), @UserId)  +',%')
            ORDER BY ID DESC
            
            SELECT * FROM #temp_table
      
            SELECT MAX([timestamp]) MaxTimestamp FROM #temp_table
	END
	
ELSE
	BEGIN
	SELECT TOP(@NbOfRows)[ID]
			, [ProcessInstanceID]
			, [TypeID] 
			, [Title] 
			, [AssignedUsers]
			, [AssignedUsersDescription]
			, [ExecutedBy]
			, [Status] 
			, [TaskData]
			, [TaskExecutionInformation]
			, [Notes]
			, [Decision]
			, [CreatedTime] 
			, [LastUpdatedTime]
			, [timestamp]
            INTO #temp2_table
            FROM [BP].[BPTask]  WITH(NOLOCK) 
            
            WHERE (@ProcessInstanceId is null or ProcessInstanceID = @ProcessInstanceId)
            AND (@UserId is null or ',' + AssignedUsers + ',' like '%,' +CONVERT(varchar(100), @UserId)  +',%')
            AND ([timestamp] > @TimestampAfter) --ONLY Updated records
            ORDER BY [timestamp]
            
            SELECT * FROM #temp2_table
      
			IF((SELECT COUNT(*) FROM #temp2_table) = 0)
				SELECT @TimestampAfter AS MaxTimestamp
			ELSE
				SELECT MAX([timestamp]) MaxTimestamp FROM #temp2_table

	END
END