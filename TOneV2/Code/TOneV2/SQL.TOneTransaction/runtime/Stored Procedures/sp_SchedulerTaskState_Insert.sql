CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_Insert]	
	@TaskId int
	
AS
BEGIN
IF NOT EXISTS(Select TaskId from runtime.ScheduleTaskState where TaskId = @TaskId)
BEGIN
	Insert into runtime.ScheduleTaskState (TaskId, [Status]) values (@TaskId, 0)
	END
END