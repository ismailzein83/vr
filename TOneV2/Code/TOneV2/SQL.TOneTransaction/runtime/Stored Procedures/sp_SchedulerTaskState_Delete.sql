CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_Delete]	
	@TaskId uniqueidentifier
	
AS
BEGIN
	Delete From runtime.ScheduleTaskState  where TaskId = @TaskId
END