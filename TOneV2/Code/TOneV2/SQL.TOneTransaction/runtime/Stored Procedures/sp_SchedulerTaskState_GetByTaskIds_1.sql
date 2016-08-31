CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_GetByTaskIds]
	@TaskIds varchar(max)
AS
BEGIN	
	DECLARE @TaskIdsTable TABLE (TaskId int)
	INSERT INTO @TaskIdsTable (TaskId)
	select Convert(int, ParsedString) from [runtime].[ParseStringList](@TaskIds)
            
            
	SELECT [TaskId]
      ,[Status]
      ,[LastRunTime]
      ,[NextRunTime]
      ,[ExecutionInfo]
	FROM [runtime].[ScheduleTaskState] WITH(NOLOCK) 
	WHERE TaskId in (select TaskId from @TaskIdsTable)
END