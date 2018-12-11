CREATE PROCEDURE [bp].[sp_BPTask_GetUpdated]
	@AfterLastUpdateTime datetime,
	@AfterId bigint,
	@NbOfRows INT,
	@ProcessInstanceId BIGINT,
	@UserId int
AS
BEGIN

IF (@AfterLastUpdateTime IS NULL)
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
            INTO #temp_table
            FROM [BP].[BPTask]  WITH(NOLOCK) 
            
            WHERE (@ProcessInstanceId is null or ProcessInstanceID = @ProcessInstanceId)
            AND (@UserId is null or ',' + AssignedUsers + ',' like '%,' +CONVERT(varchar(100), @UserId)  +',%')
            ORDER BY ID DESC
            
            SELECT * FROM #temp_table
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
            INTO #temp2_table
            FROM [BP].[BPTask]  WITH(NOLOCK) 
            
            WHERE (@ProcessInstanceId is null or ProcessInstanceID = @ProcessInstanceId)
            AND (@UserId is null or ',' + AssignedUsers + ',' like '%,' +CONVERT(varchar(100), @UserId)  +',%')
            AND (LastUpdatedTime > @AfterLastUpdateTime OR (LastUpdatedTime = @AfterLastUpdateTime AND ID > @AfterId)) --ONLY Updated records
            ORDER BY [LastUpdatedTime], [ID]
            
            SELECT * FROM #temp2_table
	END
END