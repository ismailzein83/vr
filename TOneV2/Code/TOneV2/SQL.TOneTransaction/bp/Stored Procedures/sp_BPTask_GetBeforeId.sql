CREATE PROCEDURE [bp].[sp_BPTask_GetBeforeId]
	@LessThanID BIGINT,
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
			, [TakenBy]
	FROM	[BP].[BPTask]   WITH(NOLOCK) 
	WHERE	ID < @LessThanID 
			AND (@ProcessInstanceId is null or ProcessInstanceID = @ProcessInstanceId)
            AND (@UserId is null or ',' + AssignedUsers + ',' like '%,' +CONVERT(varchar(100), @UserId)  +',%')
			AND (@TaskTypeIds is null or [TypeID] in (select TypeId from @TaskTypeIdsTable))
			AND (@Title is null or Title like '%' + @Title + '%')
			AND (@BpTaskStatuses is null or [Status] not in (select [Status] from @BpTaskStatusesTable))
	
	ORDER BY ID DESC
END