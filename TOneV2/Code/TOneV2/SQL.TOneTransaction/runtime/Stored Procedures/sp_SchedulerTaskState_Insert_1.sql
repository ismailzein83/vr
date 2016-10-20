CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_Insert]	
	@TaskId uniqueidentifier
	
AS
BEGIN
	IF NOT EXISTS(Select TaskId from runtime.ScheduleTaskState WITH(NOLOCK) where TaskId = @TaskId)
	BEGIN
		Insert into runtime.ScheduleTaskState (TaskId, [Status])
		SELECT @TaskId, 0 WHERE NOT EXISTS (Select TaskId from runtime.ScheduleTaskState where TaskId = @TaskId)
	END
END