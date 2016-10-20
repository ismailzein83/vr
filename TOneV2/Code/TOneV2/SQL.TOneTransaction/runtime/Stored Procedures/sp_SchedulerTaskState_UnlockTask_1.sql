CREATE PROCEDURE [runtime].[sp_SchedulerTaskState_UnlockTask]	
	@TaskId uniqueidentifier	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE [runtime].ScheduleTaskState
    SET	LockedByProcessID = NULL
	WHERE TaskId = @TaskId
END