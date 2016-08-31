CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_GetByTaskId]
	@TaskId int
AS
BEGIN	
	SELECT [TaskId]
      ,[Status]
      ,[LastRunTime]
      ,[NextRunTime]
      ,[ExecutionInfo]
	FROM [runtime].[ScheduleTaskState] WITH(NOLOCK) 
	WHERE TaskId = @TaskId
END