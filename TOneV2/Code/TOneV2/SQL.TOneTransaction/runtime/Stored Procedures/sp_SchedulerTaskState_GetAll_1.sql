CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_GetAll]
	
AS
BEGIN	
	SELECT [TaskId]
      ,[Status]
      ,[LastRunTime]
      ,[NextRunTime]
      ,[ExecutionInfo]
	FROM [runtime].[ScheduleTaskState]
END