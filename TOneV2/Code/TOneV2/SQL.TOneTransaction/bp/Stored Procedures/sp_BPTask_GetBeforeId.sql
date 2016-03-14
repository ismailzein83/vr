CREATE PROCEDURE [bp].[sp_BPTask_GetBeforeId]
	@LessThanID BIGINT,
	@NbOfRows INT,
	@ProcessInstanceId BIGINT,
	@UserId int
AS
BEGIN


SELECT TOP(@NbOfRows)[ID]
			, [ProcessInstanceID]
			, [TypeID] 
			, [Title] 
			, [AssignedUsers]
			, [ExecutedBy]
			, [Status] 
			, [TaskInformation]
			, [TaskExecutionInformation]
			, [CreatedTime] 
			, [LastUpdatedTime]
			, [timestamp]
	FROM [BP].[BPTask]  
	WHERE ID < @LessThanID 
		  AND (@ProcessInstanceId is null or ProcessInstanceID = @ProcessInstanceId)
            AND (@UserId is null or ',' + AssignedUsers + ',' like '%,' +CONVERT(varchar(100), @UserId)  +',%')
	
	
	ORDER BY ID DESC
END