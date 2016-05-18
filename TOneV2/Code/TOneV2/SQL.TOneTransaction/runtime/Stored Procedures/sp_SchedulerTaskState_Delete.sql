CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_Delete]	
	@TaskId int
	
AS
BEGIN
	Delete From runtime.ScheduleTaskState  where TaskId = @TaskId
END