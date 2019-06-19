CREATE PROCEDURE [bp].[sp_BPTask_GetUpdated]
	@AfterLastUpdateTime datetime,
	@AfterId bigint,
	@NbOfRows INT,
	@ProcessInstanceId BIGINT,
	@UserId int , 
	@BpTaskStatuses varchar(max),
	@TaskTypeIds varchar(max),
	@Title nvarchar(255)
	 
AS
BEGIN

DECLARE @TaskTypeIdsTable TABLE (TypeId uniqueidentifier)
	INSERT INTO @TaskTypeIdsTable (TypeId)
	select Convert(uniqueidentifier, ParsedString) from [bp].[ParseStringList](@TaskTypeIds)

DECLARE @BpTaskStatusesTable TABLE ([Status] int)
	INSERT INTO @BpTaskStatusesTable ([Status])
	select Convert(int, ParsedString) from [bp].[ParseStringList](@BpTaskStatuses)

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
			, [TakenBy]
            INTO #temp_table
            FROM [BP].[BPTask]  WITH(NOLOCK) 
            
            WHERE (@ProcessInstanceId is null or ProcessInstanceID = @ProcessInstanceId)
			AND   (@TaskTypeIds is null or [TypeID] in (select TypeId from @TaskTypeIdsTable))
			AND   (@Title is null or Title like '%' + @Title + '%')
			AND   (@BpTaskStatuses is null or [Status] not in (select [Status] from @BpTaskStatusesTable))
            AND   (@UserId is null or ',' + AssignedUsers + ',' like '%,' +CONVERT(varchar(100), @UserId)  +',%')
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
			, [TakenBy]
            INTO #temp2_table
            FROM [BP].[BPTask]  WITH(NOLOCK) 
            
            WHERE (@ProcessInstanceId is null or ProcessInstanceID = @ProcessInstanceId)
			AND  (@TaskTypeIds is null or [TypeID] in (select TypeId from @TaskTypeIdsTable))
			AND(@Title is null or Title like '%' + @Title + '%')
            AND (@UserId is null or ',' + AssignedUsers + ',' like '%,' +CONVERT(varchar(100), @UserId)  +',%')
            AND (LastUpdatedTime > @AfterLastUpdateTime OR (LastUpdatedTime = @AfterLastUpdateTime AND ID > @AfterId)) --ONLY Updated records
            ORDER BY [LastUpdatedTime], [ID]
            
            SELECT * FROM #temp2_table
	END
END